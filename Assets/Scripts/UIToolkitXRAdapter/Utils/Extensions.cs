using System;
using System.Collections.Generic;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace UIToolkitXRAdapter.Utils {
    public static class Extensions {
        [Pure]
        public static float Abs(this float value) => Math.Abs(value);

        [Pure]
        public static bool IsNearly(this float value, float other, float threshold = float.Epsilon)
            => (value - other).Abs() <= threshold;

        [Pure]
        public static float ContentHeight(this IResolvedStyle resolvedStyle) =>
            resolvedStyle.height - resolvedStyle.marginTop - resolvedStyle.borderTopWidth - resolvedStyle.paddingTop -
            resolvedStyle.paddingBottom - resolvedStyle.borderBottomWidth - resolvedStyle.marginBottom;

        [Pure]
        public static float ContentWidth(this IResolvedStyle resolvedStyle) =>
            resolvedStyle.width - resolvedStyle.marginLeft - resolvedStyle.borderLeftWidth - resolvedStyle.paddingLeft -
            resolvedStyle.paddingRight - resolvedStyle.borderRightWidth - resolvedStyle.marginRight;

        [Pure]
        public static bool IsNearlyOrLargerThan(this float value, float other) =>
            value >= other; //|| value.IsNearly(other);

        [Pure]
        public static bool IsHigherThan(this Length length, float heightInPixel, IResolvedStyle rsStyle) {
            if (length.unit == LengthUnit.Pixel && length.value > heightInPixel) {
                return true;
            }

            return length.unit == LengthUnit.Percent &&
                   (rsStyle.ContentHeight() * (length.value / 100)).IsNearlyOrLargerThan(heightInPixel);
        }

        [Pure]
        public static T Max<T>(this T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0 ? b : a;

        /// <inheritdoc cref="AsOrThrow{T}(GameObject, Search)"/>
        [NotNull]
        public static T AsOrThrow<T>(this Component comp, Search where = Search.InObjectOnly) where T : class =>
            comp.gameObject.AsOrThrow<T>(where);

        /// <param name="where">Optional search scope if the object itself does not have the component.</param>
        /// <typeparam name="T">The type of the component to find.</typeparam>
        /// <returns>The first component of type T found in the search scope or throws if not found.</returns>
        [NotNull]
        public static T AsOrThrow<T>(this GameObject go, Search where = Search.InObjectOnly) where T : class {
            var res = go.As<T>(where);

            if (res.IsNull()) {
                throw new MissingComponentException($"The component of type {typeof(T)} could not be found on {go}");
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return res;
        }

        [Pure]
        public static bool GetFeatureOrThrow(this InputDevice device, InputFeatureUsage<bool> featureUsage) {
            if (!device.TryGetFeatureValue(featureUsage, out var value)) {
                throw new UnityException($"Could not get feature {featureUsage.name} from {device.name}.");
            }

            return value;
        }

        [Pure]
        public static bool IsNotNull<T>(this T value) where T : class {
            return !Util.IsNull(value);
        }

        public static Vector3? GetFirstHitPosition(this XRRayInteractor xrRayInteractor) =>
            xrRayInteractor.GetCurrentRaycastHit(out var firstHit) ? (Vector3?) firstHit.point : null;

        public static Vector2? LocalPositionFromWorldPosition(this RectTransform rectTransform, Vector3 world) {
            Vector3[] corners = {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
            rectTransform.GetWorldCorners(corners);
            corners.ForEach(v => Debug.Log(v));
            var b = new Bounds();
            // TODO add check if in bounds
            return rectTransform.InverseTransformPoint(world).xy();
        }
    }
}