using System.Collections.Generic;
using System.Linq;
using CoreLibrary;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter {
    /// A component that ensures a <see cref="UIDocument"/> will intercept
    /// the raycasts from the BII Input system.
    /// It creates a collider that has the exact size of this component's
    /// transform, in which the UI document will be rendered in.
    /// 
    /// However, this component is not responsible for rendering or input events,
    /// see this module's docs for which other components are also necessary. 
    [RequireComponent(typeof(UIDocument), typeof(RectTransform), typeof(RenderTextureResizer))]
    public class XRInteractableUIDocument : BaseBehaviour {
        private BoxCollider _collider;

        internal RectTransform RectTransform;
        internal RenderTextureResizer Resizer;

        private UIDocument _uiDocument;
        private readonly List<TextField> _textFields = new List<TextField>();
        // TODO check http://answers.unity.com/answers/1703263/view.html
        [SerializeField, SerializeReference]
#pragma warning disable 649
        private ITextInput _textInput;
#pragma warning restore 649

        private void Awake() {
            AssignComponent(out RectTransform);
            AssignComponent(out _uiDocument);
            AssignComponent(out Resizer);
            RectTransform.pivot = new Vector2(0, 1);
            
            _collider = gameObject.AddComponent<BoxCollider>();
            _uiDocument.rootVisualElement.Query<TextField>().ForEach(RegisterTextField);
        }

        private void Update() {
            _collider.size = RectTransform.rect.size;
            // ReSharper disable once Unity.InefficientPropertyAccess
            _collider.center = RectTransform.rect.center;
            var currentTextFields = _uiDocument.rootVisualElement.Query<TextField>().Build().ToList();
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            currentTextFields.Except(_textFields).ForEach(RegisterTextField);
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            _textFields.Except(currentTextFields).ForEach(textField => _textFields.Remove(textField));
        }

        private void RegisterTextField(TextField textField) {
            textField.RegisterCallback<ClickEvent>(evt => _textInput.RegisterAsCurrentlyActive(textField));
            _textFields.Add(textField);
        }
    }
}