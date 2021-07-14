using CoreLibrary;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter {
    // TODO move to an interface as soon as a way is found to add interfaces in the editor
    public abstract class XRTextInput: BaseBehaviour {
        public abstract void Activate(TextField textField);
        
        public abstract void Deactivate(TextField textField);
    }
}