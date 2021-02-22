using System;
using CoreLibrary;
using JetBrains.Annotations;
using LanguageExt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using CommonUsages = UnityEngine.InputSystem.CommonUsages;

namespace UIToolkitXRAdapter.XRAdapter {
    internal static class XRAdapterUtils {
        [Pure]
        internal static bool IsLeftHandController(this XRController xrController) =>
            xrController.controllerNode.Equals(XRNode.LeftHand);

        [Pure]
        internal static bool IsRightHandController(this XRController xrController) =>
            xrController.controllerNode.Equals(XRNode.RightHand);

        [Pure]
        internal static Option<Vector2> PointedLocalPosition(this XRController controller) =>
            PointedLocalPosition(controller.transform);

        [Pure]
        private static Vector2 WorldToLocalPosition(this RectTransform rectTransform, Vector3 world) =>
            rectTransform.InverseTransformPoint(world);


        [Pure]
        private static Option<T> AsOption<T>(this GameObject go, Search where = Search.InObjectOnly) where T : class
            => go.As<T>(where) ?? Option<T>.None;

        [Pure]
        private static Option<Vector2> PointedLocalPosition(this Transform transform) =>
            Physics.Raycast(transform.position, transform.forward, out var hit)
                ? hit.UIToolkitLocalPosition()
                : Option<Vector2>.None;

        [Pure]
        private static Option<Vector2> UIToolkitLocalPosition(this RaycastHit hit) =>
            hit.transform.gameObject.AsOption<XRInteractableUIDocument>()
                .Map(document => (document.RectTransform.WorldToLocalPosition(hit.point) / document.Resizer.RenderScale)
                    .WithY(y => Screen.height + y));

        internal static void EnablePointerDebug(this UIDocument uiDocument) {
            const int width = 15;
            const int clickOffset = 1;

            var pointerLeft = CreatePointerPart();
            pointerLeft.style.height = new StyleLength(width + clickOffset * 3);
            pointerLeft.style.width = new StyleLength(width);
            uiDocument.rootVisualElement.Add(pointerLeft);

            var pointerRight = CreatePointerPart();
            pointerRight.style.height = new StyleLength(width + clickOffset * 2);
            pointerRight.style.width = new StyleLength(width);
            uiDocument.rootVisualElement.Add(pointerRight);


            var pointerTop = CreatePointerPart();
            pointerTop.style.height = new StyleLength(width);
            pointerTop.style.width = new StyleLength(width + clickOffset * 3);
            uiDocument.rootVisualElement.Add(pointerTop);


            var pointerBottom = CreatePointerPart();
            pointerBottom.style.height = new StyleLength(width);
            pointerBottom.style.width = new StyleLength(width + clickOffset * 3);
            uiDocument.rootVisualElement.Add(pointerBottom);

            uiDocument.rootVisualElement.RegisterCallback<PointerMoveEvent>(evt => {
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

        private static VisualElement CreatePointerPart() {
            var pointer = new VisualElement();
            pointer.style.backgroundColor = new StyleColor(Color.red);
            pointer.style.position = Position.Absolute;
            return pointer;
        }
    }
}