using CoreLibrary;
using JetBrains.Annotations;
using LanguageExt;
using UnityEngine;
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
        internal static Option<Vector2> PointedLocalPosition(this XRController controller) =>
            PointedLocalPosition(controller.transform);

        [Pure]
        private static Vector2 WorldToLocalPosition(this RectTransform rectTransform, Vector3 world) =>
            rectTransform.InverseTransformPoint(world);


        [Pure]
        private static Option<T> AsOption<T>(this GameObject go, Search where = Search.InObjectOnly) where T : class
            => go.As<T>(where) ?? Option<T>.None;

        [Pure]
        private static Option<Vector2> HitLocalPosition(this RaycastHit hit) =>
            hit.transform.gameObject.AsOption<XRInteractableUIDocument>().Map(document => document.RectTransform)
                .Map(rectTransform => rectTransform.WorldToLocalPosition(hit.point));

        [Pure]
        private static Option<Vector2> PointedLocalPosition(this Transform transform) =>
            Physics.Raycast(transform.position, transform.forward, out var hit)
                ? hit.HitLocalPosition()
                : Option<Vector2>.None;
    }
}