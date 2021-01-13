using LanguageExt;
using UIToolkitXRAdapter.AngularSizeText;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.UIs {
    public class ButtonScript : MonoBehaviour {
        private void Awake() {
            this.AsOption<UIDocument>().Map(doc => doc.rootVisualElement).IfSome(root => {
                var label = root.Q<AngularSizeLabel>();
                Option<AngularSizeButton> buttonOption = root.Q<AngularSizeButton>();
                buttonOption.IfSome(button => button.clicked += () => label.visible = !label.visible);
            });
        }
    }
}