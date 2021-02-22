using CoreLibrary;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.XR.CommonUsages;

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
            var nullableGripButton = leftController.inputDevice.TryGetFeatureValue(gripButton);
            if (nullableGripButton.HasValue) {
                if (_gripWasPressed && !nullableGripButton.Value) {
                    _turnProvider.enabled = false;
                    _gripWasPressed = false;
                }

                if (!_gripWasPressed && nullableGripButton.Value) {
                    _turnProvider.enabled = true;
                    _gripWasPressed = true;
                }
            }

            var nullableTriggerButton = leftController.inputDevice.TryGetFeatureValue(triggerButton);
            if (nullableTriggerButton.HasValue) {
                if (_triggerWasPressed && !nullableTriggerButton.Value) {
                    _moveProvider.enabled = false;
                    _triggerWasPressed = false;
                }

                if (!_gripWasPressed && nullableTriggerButton.Value) {
                    _moveProvider.enabled = true;
                    _triggerWasPressed = true;
                }
            }
        }
    }
}