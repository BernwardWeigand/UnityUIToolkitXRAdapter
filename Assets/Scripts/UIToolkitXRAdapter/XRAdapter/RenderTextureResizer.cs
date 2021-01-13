using CoreLibrary;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter {
    [RequireComponent(typeof(RectTransform), typeof(UIDocument), typeof(RawImage))]
    public class RenderTextureResizer : BaseBehaviour {

        // We want a 16-bit depth buffer and an 8-bit stencil buffer
        private int depthBufferDepth = 24;

        // Since OnRectTransformDimensionsChange may be called before Awake,
        // it may need to execute the initialization logic.
        // This bool exists to avoid unnecessary initialization calls.
        private bool _initialized;
        
        private UIDocument _document;
        private RectTransform _rectTransform;
        private RawImage _image;

        [SerializeField][Range(0.5f, 4f)][Tooltip(
            "The scale the UI will be rendered at. 1 is default, values higher" +
            " than 1 have a higher resolution and will use downscaling." +
            "Higher values can be used to reduce aliasing of the UI"
        )]
        private float _renderScale = 1.0f;

        private float _currentRenderScale;
        
        private float _lastWidth;
        private float _lastHeight;
        
        private void Awake() {
            if(!_initialized) initialize();
        }

        private void initialize() {
            AssignComponent(out _document);
            AssignComponent(out _rectTransform);
            AssignComponent(out _image);
            _document.panelSettings.clearColor = true;
            _initialized = true;
        }

        private void Start() => updateRenderTextureSize();

        private void OnDestroy() {
            if (_document.panelSettings.targetTexture != null) {
                _image.texture = null;
                RenderTexture.ReleaseTemporary(_document.panelSettings.targetTexture);
            }
        }

        private void Update() {
            // Note: We cannot use OnRectTransformDimensionChanged event, because it is called too often.
            // Instead, we have to manually check if the dimensions changed
            if (!_renderScale.IsNearly(_currentRenderScale) 
                || !_lastHeight.IsNearly(_rectTransform.rect.height) 
                || !_lastWidth.IsNearly(_rectTransform.rect.width)
            ) updateRenderTextureSize();
        }

        private void updateRenderTextureSize() {
            if(!_initialized) initialize();
            var renderTex = RenderTexture.GetTemporary(
                (int) (_rectTransform.rect.width * _renderScale),
                (int) (_rectTransform.rect.height * _renderScale),
                depthBufferDepth
            );
            _document.panelSettings.scaleMode = PanelScaleModes.ConstantPixelSize;
            _document.panelSettings.scale = _renderScale;
            _currentRenderScale = _renderScale;
            _document.panelSettings.targetTexture = renderTex;
            _image.texture = renderTex;
            
            _lastHeight = _rectTransform.rect.height;
            _lastWidth = _rectTransform.rect.width;
        }
        
    }
}