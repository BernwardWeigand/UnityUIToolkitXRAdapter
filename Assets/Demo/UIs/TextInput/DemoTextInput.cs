using UIToolkitXRAdapter.XRAdapter;
using UnityEngine.UIElements;

namespace Demo.UIs.TextInput {
    public class DemoTextInput : XRTextInput {
        private const string Demo = " Demo";

        public override void Activate(TextField textField) => textField.value += Demo;

        public override void Deactivate(TextField textField) =>
            textField.value = textField.value.Substring(0, Demo.Length);
    }
}