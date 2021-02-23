using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

namespace UIToolkitXRAdapter.XRAdapter {
    /// A component that ensures a <see cref="UIDocument"/> will intercept the raycasts from the
    /// <see cref="UIToolkitXRController"/>.
    /// It creates a collider that has the exact size of this component's transform, in which the UI document will be
    /// rendered in.
    /// 
    /// However, this component is not responsible for rendering or input events, see the docs for which other
    /// components are also necessary. 
    [RequireComponent(typeof(RenderTextureResizer))]
    public class XRInteractableUIDocument : BaseBehaviour {
        private BoxCollider _collider;


        internal RenderTextureResizer Resizer;


        public bool debugPointerPosition;
        private (EventCallback<PointerMoveEvent>, List<VisualElement>)? _debugPointerPositionInformation;


        [SerializeField] private InputSystemEventSystem _inputSystemEventSystem;

        private bool _hasFocus;

        internal bool IsFocused {
            get => _hasFocus;
            set {
                if (value == _hasFocus) {
                    return;
                }

                var root = Resizer.Content.rootVisualElement;
                if (value && debugPointerPosition) {
                    _debugPointerPositionInformation = root.AddPointerPositionDebugCallback();
                }
                else if (debugPointerPosition && _debugPointerPositionInformation.HasValue) {
                    root.UnregisterCallback(_debugPointerPositionInformation.Value.Item1);
                    _debugPointerPositionInformation.Value.Item2.ForEach(p => p.RemoveFromHierarchy());
                }

                _hasFocus = value;
            }
        }

        public XRTextInput xrTextInput;
        private readonly List<TextField> _textFields = new List<TextField>();

        private void Awake() {
            if (_inputSystemEventSystem.IsNull()) {
                throw new Exception($"Please add a {nameof(InputSystemEventSystem)} to the {name}.");
            }

            AssignComponent(out Resizer);
            _collider = gameObject.AddComponent<BoxCollider>();
            if (!Resizer.Initialized) {
                Resizer.Initialize();
            }

            Resizer.WorldBounds.pivot = new Vector2(0, 1);

            IsFocused = false;

            Resizer.Content.rootVisualElement.Query<TextField>().ForEach(RegisterTextField);
        }

        private void Update() {
            if (_hasFocus) {
                // TODO handle focus in Controller to prevent focus in other panels
                var panelSettings = Resizer.Content.panelSettings;
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                var panel = panelSettings.GetType().GetProperty("panel", bindingFlags)?.GetValue(panelSettings);
                _inputSystemEventSystem.GetType().GetProperty("focusedPanel", bindingFlags)
                    ?.SetValue(_inputSystemEventSystem, panel);
            }

            var rectangle = Resizer.WorldBounds.rect;
            _collider.size = rectangle.size;
            _collider.center = rectangle.center;

            var currentTextFields = Resizer.Content.rootVisualElement.Query<TextField>().Build().ToList();
            currentTextFields.Except(_textFields).ForEach(RegisterTextField);
            _textFields.Except(currentTextFields).ForEach(textField => _textFields.Remove(textField));
        }

        private void RegisterTextField(TextField textField) {
            if (xrTextInput.IsNull()) {
                throw new Exception($"To use a {textField.GetType().Name} in a {nameof(XRInteractableUIDocument)} " +
                                    $"you have to provide a {nameof(XRTextInput)} to it.");
            }

            textField.RegisterCallback<FocusInEvent>(evt => xrTextInput.Activate(textField));
            textField.RegisterCallback<FocusOutEvent>(evt => xrTextInput.Deactivate(textField));
            _textFields.Add(textField);
        }
    }
}