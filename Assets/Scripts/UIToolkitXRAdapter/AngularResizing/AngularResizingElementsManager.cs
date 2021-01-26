using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary;
using UIToolkitXRAdapter.AngularResizing.DefaultElements;
using UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing {
    [RequireComponent(typeof(UIDocument), typeof(RectTransform), typeof(RawImage))]
    public class AngularResizingElementsManager : BaseBehaviour {
        private Camera _camera;
        private UIDocument _document;
        private RectTransform _panel;
        private RectTransform _canvasRect;
        private Renderer _renderer;

        private void Awake() {
            _camera = Camera.main;
            if (_camera.IsNull()) {
                throw new Exception("Could not find main Camera.");
            }

            AssignComponent(out _document);
            AssignComponent(out _panel);
            AssignComponent(out _renderer, Search.InParents);

            AssignComponent(out Canvas canvas, Search.InParents);
            _canvasRect = canvas.As<RectTransform>();
        }

        private void Update() {
            if (!_renderer.isVisible) {
                return;
            }

            if (SizeIsCorrupted()) {
                throw new UnityException("The canvas and the UI Document have to have the same size!");
            }
            

            // TODO may refactor it towards reflection if code becomes more complex
            _document.rootVisualElement.Query<AngularFontHeightButton>().Where(CanResize).Build().ForEach(Resize);
            _document.rootVisualElement.Query<AngularFontHeightLabel>().Where(CanResize).Build().ForEach(Resize);
            _document.rootVisualElement.Query<AngularResizingVisualElement>().Where(CanResize).Build().ForEach(Resize);
        }

        // ReSharper disable once IdentifierTypo
        private static bool CanResize<T>(T cullableElement) where T : VisualElement, ICullableElement =>
            cullableElement.HasToBeCulledWhenCannotExpand || cullableElement.visible;

        private bool SizeIsCorrupted() {
            var canvasBounds = _canvasRect.rect;
            var uiBounds = _document.rootVisualElement.layout;

            if (float.IsNaN(uiBounds.height) || float.IsNaN(uiBounds.width)) {
                // the UIDocument is not initialized
                return false;
            }

            return !canvasBounds.height.IsNearly(uiBounds.height) || !canvasBounds.width.IsNearly(uiBounds.width);
        }

        private void Resize<T>(IAngularResizableElement<T> angularResizableElement) where T : VisualElement {
            var layout = angularResizableElement.AsVisualElement().layout;
            var leftDistance = new List<Vector3> {
                CalculateCornerWorldPosition(new Vector2(layout.xMin, layout.yMin)),
                CalculateCornerWorldPosition(new Vector2(layout.xMin, layout.yMax))
            }.Select(DistanceToCamera).Min();
            var rightDistance = new List<Vector3> {
                CalculateCornerWorldPosition(new Vector2(layout.xMax, layout.yMin)),
                CalculateCornerWorldPosition(new Vector2(layout.xMax, layout.yMax))
            }.Select(DistanceToCamera).Min();
            angularResizableElement.Resize(rightDistance.Max(leftDistance), _panel.lossyScale.y);
        }

        private float DistanceToCamera(Vector3 worldPosition) => 
            Vector3.Distance(_camera.transform.position, worldPosition);

        private Vector3 CalculateCornerWorldPosition(Vector2 localCornerPosition) =>
            _panel.position + _panel.rotation * (localCornerPosition * _panel.lossyScale);
    }
}