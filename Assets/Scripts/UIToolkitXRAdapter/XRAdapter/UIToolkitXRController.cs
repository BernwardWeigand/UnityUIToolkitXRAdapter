using System.Linq;
using System.Runtime.InteropServices;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;
using UnityEngine.UIElements.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UIToolkitXRAdapter.XRAdapter.UIToolkitXRController;
using static UnityEngine.InputSystem.CommonUsages;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace UIToolkitXRAdapter.XRAdapter {
    [StructLayout(LayoutKind.Sequential)]
    public struct UIToolkitXRControllerState : IInputStateTypeInfo {
        public FourCC format => new FourCC('X', 'R', 'U', 'I');

        [InputControl(name = UIToolkitLocalPositionName, displayName = "UI Toolkit Local Position", layout = "Vector2",
            synthetic = true)]
        // ReSharper disable once MemberCanBePrivate.Global
        public Vector2 UIToolkitLocalPosition;
    }


#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    [InputControlLayout(displayName = DisplayName, stateType = typeof(UIToolkitXRControllerState),
        isGenericTypeOfDevice = true, commonUsages = new[] {nameof(RightHand), nameof(LeftHand)})]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIToolkitXRController : InputDevice, IInputUpdateCallbackReceiver {
        private const string DisplayName = "UI Toolkit XR Controller";

        internal const string UIToolkitLocalPositionName = "uiToolkitLocalPosition";

        private XRController _leftController;
        private XRController _rightController;

        private InputSystemEventSystem _inputSystemEventSystem;
        [CanBeNull] private XRInteractableUIDocument _previouslyPointedDocument;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Vector2Control UIToolkitLocalPosition { get; private set; }

        protected override void FinishSetup() {
            base.FinishSetup();
            UIToolkitLocalPosition = GetChildControl<Vector2Control>(UIToolkitLocalPositionName);
            var controllers = Object.FindObjectsOfType<XRController>();
            var leftController = controllers.Where(XRAdapterUtils.IsLeftHandController).First();
            if (leftController != null) {
                _leftController = leftController;
            }

            var rightController = controllers.Where(XRAdapterUtils.IsRightHandController).First();
            if (rightController != null) {
                _rightController = rightController;
            }

            var inputSystemEventSystem = Object.FindObjectOfType<InputSystemEventSystem>();
            if (inputSystemEventSystem != null) {
                _inputSystemEventSystem = inputSystemEventSystem;
            }
        }

        static UIToolkitXRController() => InputSystem.RegisterLayout<UIToolkitXRController>();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            InputDevices.deviceConnected += DeviceConnected;
            InputDevices.deviceDisconnected += DeviceDisconnected;
        }

        public void OnUpdate() {
            // TODO maybe add dominant hand to ensure one pointer is used
            if (usages.Contains(LeftHand)) {
                HandleController(_leftController);
            }

            if (usages.Contains(RightHand)) {
                HandleController(_rightController);
            }
        }

        private void HandleController(XRController controller) {
            var possibleRaycastHit = controller.PointingAt();
            if (possibleRaycastHit.HasValue) {
                var raycastHit = possibleRaycastHit.Value;
                var xrInteractableUIDocument = raycastHit.transform.gameObject.As<XRInteractableUIDocument>();
                // ReSharper disable once InvertIf
                if (xrInteractableUIDocument != null) {
                    if (!xrInteractableUIDocument.Equals(_previouslyPointedDocument)) {
                        if (_previouslyPointedDocument != null) {
                            _previouslyPointedDocument.IsFocused = false;
                        }

                        _previouslyPointedDocument = xrInteractableUIDocument;
                    }

                    if (!xrInteractableUIDocument.IsFocused) {
                        xrInteractableUIDocument.IsFocused = true;
                    }

                    _inputSystemEventSystem.FocusOn(xrInteractableUIDocument);

                    InputSystem.QueueStateEvent(this, new UIToolkitXRControllerState {
                        UIToolkitLocalPosition = xrInteractableUIDocument.UIToolkitPosition(raycastHit.point)
                    });
                    UIToolkitLocalPosition = GetChildControl<Vector2Control>(UIToolkitLocalPositionName);
                }
            }
            else {
                if (_previouslyPointedDocument != null) {
                    _previouslyPointedDocument.IsFocused = false;
                    _previouslyPointedDocument = null;

                    _inputSystemEventSystem.FocusOn(_previouslyPointedDocument);
                }

                InputSystem.QueueStateEvent(this, new UIToolkitXRControllerState {
                    UIToolkitLocalPosition = Vector2.zero
                });
                UIToolkitLocalPosition = GetChildControl<Vector2Control>(UIToolkitLocalPositionName);
            }
        }

        private static void DeviceConnected(UnityEngine.XR.InputDevice inputDevice) {
            var characteristics = inputDevice.characteristics;
            if ((characteristics & InputDeviceCharacteristics.Controller) == 0) return;
            if ((characteristics & InputDeviceCharacteristics.Left) != 0 &&
                InputSystem.GetDevice<UIToolkitXRController>(LeftHand).IsNull()) {
                InputSystem.SetDeviceUsage(InputSystem.AddDevice<UIToolkitXRController>(), LeftHand);
            }

            if ((characteristics & InputDeviceCharacteristics.Right) != 0 &&
                InputSystem.GetDevice<UIToolkitXRController>(RightHand).IsNull()) {
                InputSystem.SetDeviceUsage(InputSystem.AddDevice<UIToolkitXRController>(), RightHand);
            }
        }

        private static void DeviceDisconnected(UnityEngine.XR.InputDevice inputDevice) {
            var characteristics = inputDevice.characteristics;
            if ((characteristics & InputDeviceCharacteristics.Controller) == 0) return;
            if ((characteristics & InputDeviceCharacteristics.Left) != 0 &&
                // ReSharper disable once PatternAlwaysOfType
                InputSystem.GetDevice<UIToolkitXRController>(LeftHand) is UIToolkitXRController leftController) {
                InputSystem.RemoveDevice(leftController);
            }

            if ((characteristics & InputDeviceCharacteristics.Right) == 0) return;
            // ReSharper disable once PatternAlwaysOfType
            if (InputSystem.GetDevice<UIToolkitXRController>(RightHand) is UIToolkitXRController rightController) {
                InputSystem.RemoveDevice(rightController);
            }
        }
    }
}