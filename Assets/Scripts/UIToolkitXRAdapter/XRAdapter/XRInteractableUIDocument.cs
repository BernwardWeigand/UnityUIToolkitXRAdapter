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
    [RequireComponent(typeof(UIDocument), typeof(RectTransform))]
    public class XRInteractableUIDocument : BaseBehaviour {
        private BoxCollider _collider;

        internal RectTransform RectTransform;
        internal UIDocument UIDocument;

        private void Awake() {
            AssignComponent(out RectTransform);
            AssignComponent(out UIDocument);
            RectTransform.pivot = new Vector2(0, 0);
            _collider = gameObject.AddComponent<BoxCollider>();
        }

        private void Update() {
            _collider.size = RectTransform.rect.size;
            // ReSharper disable once Unity.InefficientPropertyAccess
            _collider.center = RectTransform.rect.center;
        }
    }
}