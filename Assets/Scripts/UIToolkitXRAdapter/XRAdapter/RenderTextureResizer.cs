using CoreLibrary;
using UIToolkitXRAdapter.AngularResizing;
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
        internal bool Initialized;

        internal UIDocument Content;
        internal RectTransform WorldBounds;
        private RawImage _image;

        [SerializeField]
        [Range(0.5f, 4f)]
        [Tooltip(
            "The scale the UI will be rendered at. 1 is default, values higher" +
            " than 1 have a higher resolution and will use downscaling." +
            "Higher values can be used to reduce aliasing of the UI"
        )]
        private float renderScale = 1.0f;

        public float RenderScale => renderScale;

        private float _currentRenderScale;

        private float _lastWidth;
        private float _lastHeight;

        private void Awake() {
            if (!Initialized) Initialize();
        }

        internal void Initialize() {
            AssignComponent(out Content);
            AssignComponent(out WorldBounds);
            AssignComponent(out _image);
            Content.panelSettings.clearColor = true;
            Initialized = true;
        }

        private void Start() => updateRenderTextureSize();

        private void OnDestroy() {
            if (Content.panelSettings.targetTexture != null) {
                _image.texture = null;
                RenderTexture.ReleaseTemporary(Content.panelSettings.targetTexture);
            }
            
            Content.panelSettings.clearColor = false;

        }

        private void Update() {
            // Note: We cannot use OnRectTransformDimensionChanged event, because it is called too often.
            // Instead, we have to manually check if the dimensions changed
            if (!renderScale.IsNearly(_currentRenderScale)
                || !_lastHeight.IsNearly(WorldBounds.rect.height)
                || !_lastWidth.IsNearly(WorldBounds.rect.width)
            ) updateRenderTextureSize();
        }

        private void updateRenderTextureSize() {
            if (!Initialized) Initialize();
            var renderTex = RenderTexture.GetTemporary(
                (int) (WorldBounds.rect.width * renderScale),
                (int) (WorldBounds.rect.height * renderScale),
                depthBufferDepth
            );
            Content.panelSettings.scaleMode = PanelScaleModes.ConstantPixelSize;
            Content.panelSettings.scale = renderScale;
            _currentRenderScale = renderScale;
            Content.panelSettings.clearColor = true;
            Content.panelSettings.targetTexture = renderTex;
            _image.texture = renderTex;

            _lastHeight = WorldBounds.rect.height;
            _lastWidth = WorldBounds.rect.width;
        }
    }
}