﻿using System;
using System.Collections.Generic;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
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
        /// The <see cref="XRInteractableUIDocument.debugPointer"/> may be useful to debug this method, if necessary.
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

        internal static Tuple<EventCallback<PointerMoveEvent>, List<VisualElement>> AddPointerPositionDebugCallback(
            this VisualElement visualElement) {
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

            void Callback(PointerMoveEvent evt) {
                pointerLeft.transform.position = evt.localPosition.WithY(y => y - (width + clickOffset))
                    .WithX(x => x - (width + clickOffset));
                pointerBottom.transform.position = evt.localPosition.WithX(x => x - (width + clickOffset))
                    .WithY(y => y + clickOffset);
                pointerTop.transform.position = evt.localPosition.WithY(y => y - (width + clickOffset))
                    .WithX(x => x - clickOffset);
                pointerRight.transform.position = evt.localPosition.WithX(x => x + clickOffset)
                    .WithY(y => y - clickOffset);
            }

            visualElement.RegisterCallback((EventCallback<PointerMoveEvent>) Callback);
            return new Tuple<EventCallback<PointerMoveEvent>, List<VisualElement>>(Callback,
                new List<VisualElement>(new[] {pointerBottom, pointerTop, pointerLeft, pointerRight}));
        }

        [Pure]
        private static VisualElement CreatePointerPart() {
            var pointer = new VisualElement();
            pointer.style.backgroundColor = new StyleColor(Color.red);
            pointer.style.position = Position.Absolute;
            return pointer;
        }
    }
}