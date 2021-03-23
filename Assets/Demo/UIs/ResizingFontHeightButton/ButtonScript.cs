using CoreLibrary;
using UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.UIs.ResizingFontHeightButton {
    public class ButtonScript : MonoBehaviour {
        private void Awake() {
            var document = this.As<UIDocument>();
            // ReSharper disable once InvertIf
            if (document != null) {
                var root = document.rootVisualElement;
                var label = root.Q<AngularFontHeightLabel>();
                var button = root.Q<AngularFontHeightButton>();
                if (button != null) {
                    button.clicked += () => label.visible = !label.visible;
                }
            }
        }
    }
}