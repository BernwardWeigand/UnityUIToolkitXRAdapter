using UIToolkitXRAdapter.XRAdapter;
using UnityEngine.UIElements;

namespace Demo.UIs.TextInput {
    public class DemoTextInput : XRTextInput {
        public override void RegisterAsCurrentlyActive(TextField textField) => textField.value += " Demo";
    }
}