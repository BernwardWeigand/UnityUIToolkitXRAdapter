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
    public class InteractableUIDocument : BaseBehaviour {

        private BoxCollider _collider;
        private RectTransform _rectTransform;

        private void Awake() {
            AssignComponent(out _rectTransform);
            _collider = gameObject.AddComponent<BoxCollider>();
        }

        private void Update() {
            var pivotOffset = (new Vector2(0.5f, 0.5f) - _rectTransform.pivot);
            var size = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
            var offset = size.WithX(x => x * pivotOffset.x).WithY(y => y * pivotOffset.y);
            _collider.size = size;
            _collider.center = offset;
        }
    }
}