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
            var pointer = new VisualElement();
            pointer.style.backgroundColor = new StyleColor(Color.red);
            pointer.style.position = Position.Absolute;
            const int size = 30;
            pointer.style.height = new StyleLength(size);
            pointer.style.width = new StyleLength(size);
            uiDocument.rootVisualElement.Add(pointer);
            const int clickOffset = 1;
            uiDocument.rootVisualElement.RegisterCallback<PointerMoveEvent>(evt => {
                pointer.transform.position = evt.localPosition.WithY(y => y + clickOffset).WithX(x => x + clickOffset);
            });
        }
    }
}