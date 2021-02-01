﻿using System.Runtime.InteropServices;
using LanguageExt;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;
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

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Vector2Control UIToolkitLocalPosition { get; private set; }

        protected override void FinishSetup() {
            base.FinishSetup();
            UIToolkitLocalPosition = GetChildControl<Vector2Control>(UIToolkitLocalPositionName);
            var controllers = Object.FindObjectsOfType<XRController>();
            controllers.Find(XRAdapterUtils.IsLeftHandController).IfSome(controller => _leftController = controller);
            controllers.Find(XRAdapterUtils.IsRightHandController).IfSome(controller => _rightController = controller);
        }

        static UIToolkitXRController() => InputSystem.RegisterLayout<UIToolkitXRController>();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            InputDevices.deviceConnected += DeviceConnected;
            InputDevices.deviceDisconnected += DeviceDisconnected;
        }

        public void OnUpdate() {
            if (usages.Contains(LeftHand)) {
                HandleController(_leftController);
            }

            if (usages.Contains(RightHand)) {
                HandleController(_rightController);
            }
        }

        private void HandleController(XRController controller) => controller.PointedLocalPosition()
            .Match(uiToolkitLocalPosition => {
                    InputSystem.QueueStateEvent(this, new UIToolkitXRControllerState {
                        UIToolkitLocalPosition = uiToolkitLocalPosition
                    });
                    UIToolkitLocalPosition = GetChildControl<Vector2Control>(UIToolkitLocalPositionName);
                }
                , () => {
                    InputSystem.QueueStateEvent(this, new UIToolkitXRControllerState {
                        UIToolkitLocalPosition = Vector2.zero
                    });
                    UIToolkitLocalPosition = GetChildControl<Vector2Control>(UIToolkitLocalPositionName);
                }
            );

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
            if ((characteristics & InputDeviceCharacteristics.Left) != 0) {
                Option<UIToolkitXRController> leftController = InputSystem.GetDevice<UIToolkitXRController>(LeftHand);
                leftController.IfSome(InputSystem.RemoveDevice);
            }

            if ((characteristics & InputDeviceCharacteristics.Right) == 0) return;
            Option<UIToolkitXRController> rightController = InputSystem.GetDevice<UIToolkitXRController>(RightHand);
            rightController.IfSome(InputSystem.RemoveDevice);
        }
    }
}