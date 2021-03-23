using UIToolkitXRAdapter.XRAdapter;
using UnityEngine.UIElements;

namespace Demo.UIs.TextInput {
    public class DemoTextInput : XRTextInput {
        public override void Activate(TextField textField) => textField.value = "Active";

        public override void Deactivate(TextField textField) => textField.value = "Inactive";
    }
}