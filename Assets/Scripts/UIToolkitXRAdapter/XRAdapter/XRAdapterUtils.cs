using System;
using System.Collections.Generic;
using System.Reflection;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace UIToolkitXRAdapter.XRAdapter {
    internal static class XRAdapterUtils {
        [Pure]
        internal static bool IsLeftHandController(this XRController xrController) =>
            xrController.controllerNode.Equals(XRNode.LeftHand);

        [Pure]
        internal static bool IsRightHandController(this XRController xrController) =>
            xrController.controllerNode.Equals(XRNode.RightHand);

        [Pure]
        private static Vector2 WorldToLocal(this Transform rectTransform, Vector3 world) =>
            rectTransform.InverseTransformPoint(world);

        [Pure]
        public static RaycastHit? PointingAt(this XRController xrController) => PointingAt(xrController.transform);

        [Pure]
        private static RaycastHit? PointingAt(this Transform transform) =>
            Physics.Raycast(transform.position, transform.forward, out var hit) ? hit : (RaycastHit?) null;

        /// <remarks>because of the lack of world space render support in the
        /// <see cref="UnityEngine.UIElements.InputSystem.InputSystemEventSystem.OnPointPerformed"/> method,
        /// which processes the return value, the <see cref="Screen.height"/> is part of the y-axis calculation.
        /// The <see cref="XRInteractableUIDocument.debugPointerPosition"/> may be useful to debug this method.
        /// </remarks>
        /// <param name="xrUIDocument">
        /// The <see cref="RaycastHit.point"/> on the <see cref="XRInteractableUIDocument"/>
        /// </param>
        /// <param name="worldHitPos">The world space position where the xrUIDocument was hit</param>
        /// <returns>the local UI Toolkit position on the <see cref="XRInteractableUIDocument"/> or null</returns>
        [Pure]
        public static Vector2 UIToolkitPosition(this XRInteractableUIDocument xrUIDocument, Vector3 worldHitPos) {
            var resizer = xrUIDocument.Resizer;
            return (resizer.WorldBounds.WorldToLocal(worldHitPos) * resizer.RenderScale).WithY(y => Screen.height + y);
        }

        internal static void AddPointerPositionDebugCallback(this VisualElement visualElement) {
            const int width = 15;
            const int clickOffset = 1;

            var pointerLeft = CreatePointerPart();
            pointerLeft.style.height = new StyleLength(width + clickOffset * 3);
            pointerLeft.style.width = new StyleLength(width);
            visualElement.Add(pointerLeft);

            var pointerRight = CreatePointerPart();
            pointerRight.style.height = new StyleLength(width + clickOffset * 2);
            pointerRight.style.width = new StyleLength(width);
            visualElement.Add(pointerRight);


            var pointerTop = CreatePointerPart();
            pointerTop.style.height = new StyleLength(width);
            pointerTop.style.width = new StyleLength(width + clickOffset * 3);
            visualElement.Add(pointerTop);


            var pointerBottom = CreatePointerPart();
            pointerBottom.style.height = new StyleLength(width);
            pointerBottom.style.width = new StyleLength(width + clickOffset * 3);
            visualElement.Add(pointerBottom);

            visualElement.RegisterCallback<PointerMoveEvent>(evt => {
                pointerLeft.transform.position = evt.localPosition.WithY(y => y - (width + clickOffset))
                    .WithX(x => x - (width + clickOffset));
                pointerBottom.transform.position = evt.localPosition.WithX(x => x - (width + clickOffset))
                    .WithY(y => y + clickOffset);
                pointerTop.transform.position = evt.localPosition.WithY(y => y - (width + clickOffset))
                    .WithX(x => x - clickOffset);
                pointerRight.transform.position = evt.localPosition.WithX(x => x + clickOffset)
                    .WithY(y => y - clickOffset);
            });
        }

        [Pure]
        private static VisualElement CreatePointerPart() {
            var pointer = new VisualElement();
            pointer.style.backgroundColor = new StyleColor(Color.red);
            pointer.style.position = Position.Absolute;
            return pointer;
        }

        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;

        /// <summary> Extracts the <see cref="PanelSettings.panel"/> via reflection, because it is internal</summary>
        /// <param name="xrInteractableUIDocument">
        /// the <see cref="XRInteractableUIDocument"/> holding the property that has to be extracted
        /// </param>
        /// <returns>the extracted <see cref="BaseRuntimePanel"/></returns>
        internal static object GetPanel(this XRInteractableUIDocument xrInteractableUIDocument) {
            var panelSettings = xrInteractableUIDocument.Resizer.Content.panelSettings;
            return panelSettings.GetType().GetProperty("panel", BindingFlags)?.GetValue(panelSettings);
        }

        /// <summary>
        /// Sets the <see cref="InputSystemEventSystem.focusedPanel"/> via reflection, because it is internal
        /// </summary>
        /// <param name="eventSystem">the <see cref="InputSystemEventSystem"/> where the property is on</param>
        /// <param name="panel">the <see cref="BaseRuntimePanel"/> that has to be marked as focused</param>
        internal static void SetFocusedPanel(this InputSystemEventSystem eventSystem, object panel) =>
            eventSystem.GetType().GetProperty("focusedPanel", BindingFlags)?.SetValue(eventSystem, panel);
    }
}