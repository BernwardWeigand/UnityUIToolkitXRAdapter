using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter {
    // TODO move to an interface as soon as a way is found to add interfaces in the editor
    public abstract class XRTextInput: MonoBehaviour {
        public abstract void RegisterAsCurrentlyActive(TextField textField);
    }
}