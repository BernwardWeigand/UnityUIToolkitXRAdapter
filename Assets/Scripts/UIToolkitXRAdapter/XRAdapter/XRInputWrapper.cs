using CoreLibrary;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.XR.CommonUsages;

namespace UIToolkitXRAdapter.XRAdapter {
    [RequireComponent(typeof(RectTransform))]
    public class XRInputWrapper : InputWrapper {
        private RectTransform _rectTransform;
        private XRInputWrapperEventSystem _xrInputWrapperEventSystem;

        private void Awake() {
            _rectTransform = this.AsOrThrow<RectTransform>();
            _rectTransform.pivot = new Vector2(0, 1);
            var xrInputWrapperEventSystem = FindObjectOfType<XRInputWrapperEventSystem>();
            if (UtilityExtensions.IsNull(xrInputWrapperEventSystem)) {
                throw new UnityException(
                    $"There has to be an {typeof(XRInputWrapperEventSystem)} component in the hierarchy to use the " +
                    $"{typeof(XRInputWrapper)}");
            }

            _xrInputWrapperEventSystem = xrInputWrapperEventSystem;
        }

        private XRController PrimaryController => _xrInputWrapperEventSystem.PrimaryController;

        /// <inheritdoc cref="InputWrapper.mousePresent"/>
        public override bool mousePresent => PrimaryController.IsNotNull() && PrimaryController.isActiveAndEnabled;

        private InputDevice PrimaryInputDevice => PrimaryController.inputDevice;

        /// <inheritdoc cref="InputWrapper.mouseScrollDelta"/>
        /// <remarks>
        /// TODO check implementation
        /// TODO check inversion of y axis 
        /// TODO may add a deadzone
        /// </remarks>
        public override Vector2 mouseScrollDelta => PrimaryInputDevice.GetFeatureOrThrow(primary2DAxis).InvertY();

        private bool PrimaryTriggerIsDown => PrimaryInputDevice.GetFeatureOrThrow(triggerButton);
        private bool PrimaryGripIsDown => PrimaryInputDevice.GetFeatureOrThrow(gripButton);
        private bool PrimaryThumbIsPressed => PrimaryInputDevice.GetFeatureOrThrow(primary2DAxisClick);

        private bool _triggerWasDown;
        private bool _gripWasDown;
        private bool _thumbWasPressed;

        /// <inheritdoc cref="InputWrapper.GetMouseButtonDown"/>
        public override bool GetMouseButtonDown(int button) {
            switch (button) {
                case 0:
                    if (_triggerWasDown || !PrimaryTriggerIsDown) {
                        return false;
                    }

                    _triggerWasDown = true;
                    return true;
                case 1:
                    if (_gripWasDown || !PrimaryGripIsDown) {
                        return false;
                    }

                    _gripWasDown = true;
                    return true;
                case 2:
                    if (_thumbWasPressed || !PrimaryThumbIsPressed) {
                        return false;
                    }

                    _thumbWasPressed = true;
                    return true;
                default: return false;
            }
        }

        /// <inheritdoc cref="InputWrapper.GetMouseButtonUp"/>
        public override bool GetMouseButtonUp(int button) {
            switch (button) {
                case 0:
                    if (!_triggerWasDown || PrimaryTriggerIsDown) {
                        return false;
                    }

                    _triggerWasDown = false;
                    return true;
                case 1:
                    if (!_gripWasDown || PrimaryGripIsDown) {
                        return false;
                    }

                    _gripWasDown = false;
                    return true;
                case 2:
                    if (!_thumbWasPressed || PrimaryThumbIsPressed) {
                        return false;
                    }

                    _thumbWasPressed = false;
                    return true;
                default: return false;
            }
        }

        /// <inheritdoc cref="InputWrapper.GetMouseButton"/>
        public override bool GetMouseButton(int button) => button switch {
            // left click
            0 => PrimaryTriggerIsDown,
            // right click
            1 => PrimaryGripIsDown,
            // middle click
            2 => PrimaryThumbIsPressed,
            _ => false
        };

        public override Vector2 mousePosition => _xrInputWrapperEventSystem.CurrentPointerHit
            // With a pivot of (0,1) we need to invert the y value because it is calculated incorrectly
            .Bind(firstHit => firstHit.HitLocalPosition(_rectTransform).Map(Extensions.InvertY))
            // The controller points towards the nirvana
            .IfNone(Vector2.negativeInfinity);
    }
}