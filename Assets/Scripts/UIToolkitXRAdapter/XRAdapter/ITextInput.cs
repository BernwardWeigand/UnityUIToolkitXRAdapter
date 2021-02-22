using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter {
    public interface ITextInput {
        public void RegisterAsCurrentlyActive(TextField textField);
    }
}