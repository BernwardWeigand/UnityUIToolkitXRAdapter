using UIToolkitXRAdapter.AngularSizeText;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.UIs {
    public class ButtonScript : MonoBehaviour {
        private void Awake() {
            var root = this.AsOrThrow<UIDocument>().rootVisualElement;
            var label = root.Q<AngularSizeLabel>();
            root.Q<AngularSizeButton>().clicked += () => label.visible = !label.visible;
        }
    }
}