using CoreLibrary;
using LanguageExt;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Demo.UIs {
    [RequireComponent(typeof(DeviceBasedContinuousMoveProvider), typeof(DeviceBasedContinuousMoveProvider))]
    public class ControllerScript : BaseBehaviour {
        private DeviceBasedContinuousTurnProvider _turnProvider;
        private DeviceBasedContinuousMoveProvider _moveProvider;
        private bool _gripWasPressed;
        private bool _triggerWasPressed;

        [SerializeField] private XRController leftController;

        private void Awake() {
            AssignComponent(out _turnProvider);
            AssignComponent(out _moveProvider);
        }

        private void Update() {
            leftController.inputDevice.TryGetFeatureValue(gripButton).IfSome(gripIsPressed => {
                if (_gripWasPressed && !gripIsPressed) {
                    _turnProvider.enabled = false;
                    _gripWasPressed = false;
                }

                if (!_gripWasPressed && gripIsPressed) {
                    _turnProvider.enabled = true;
                    _gripWasPressed = true;
                }
            });

            leftController.inputDevice.TryGetFeatureValue(triggerButton).IfSome(triggerIsPressed => {
                if (_triggerWasPressed && !triggerIsPressed) {
                    _moveProvider.enabled = false;
                    _triggerWasPressed = false;
                }

                if (!_gripWasPressed && triggerIsPressed) {
                    _moveProvider.enabled = true;
                    _triggerWasPressed = true;
                }
            });
        }
    }
}