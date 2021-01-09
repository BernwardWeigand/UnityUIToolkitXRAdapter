using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter{
    [RequireComponent(typeof(RectTransform))]
    public class XRInputWrapper : InputWrapper {

        private RectTransform _rectTransform;

        public override bool mousePresent => true;

        private void Awake() {
            // _rectTransform = this.AsOrThrow<RectTransform>();
            _rectTransform.pivot = new Vector2(0, 1);
        }

        public override Vector2 mousePosition {
            get {
                // if (UserInput.Actions.PointerTo is Vector3 t) {
                //     var res = Utils.Position.FromGlobal(t).RelativeTo(_rectTransform);
                //     // With a pivot of (0,1) we need to flip the y value because it is calculated incorrectly
                //     return res.xy().WithY(y => -y);
                // }
                return Vector2.zero;
            }
        }

        public override bool GetMouseButtonDown(int button) {
            switch (button) {
                case 0: // left click
                    // return UserInput.ActiveHandler.TriggerDown;
                case 1: // right click
                    // return UserInput.ActiveHandler.Button1Down;
                case 2: // middle click
                    return false;
                default:
                    return false;
            }
        }

        public override bool GetMouseButtonUp(int button) {
            switch (button) {
                case 0: // left click
                    // return UserInput.ActiveHandler.TriggerUp;
                case 1: // right click
                    // return UserInput.ActiveHandler.Button1Up;
                case 2: // middle click
                    return false;
                default:
                    return false;
            }
        }

        public override bool GetMouseButton(int button) {
            switch (button) {
                case 0: // left click
                    // return UserInput.ActiveHandler.Trigger;
                case 1: // right click
                    // return UserInput.ActiveHandler.Button1;
                case 2: // middle click
                    return false;
                default:
                    return false;
            }
        }

        // public override Vector2 mouseScrollDelta => UserInput.ActiveHandler.ScrollDelta.WithY(y => -y);
    }
}