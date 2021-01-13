using System;
using CoreLibrary;
using JetBrains.Annotations;
using LanguageExt;
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
        internal static T AsOrThrow<T>(this Component comp, Search where = Search.InObjectOnly) where T : class =>
            comp.gameObject.AsOrThrow<T>(where);

        /// <param name="where">Optional search scope if the object itself does not have the component.</param>
        /// <typeparam name="T">The type of the component to find.</typeparam>
        /// <returns>The first component of type T found in the search scope or throws if not found.</returns>
        [NotNull]
        private static T AsOrThrow<T>(this GameObject go, Search where = Search.InObjectOnly) where T : class {
            var res = go.As<T>(where);

            if (UtilityExtensions.IsNull(res)) {
                throw new MissingComponentException($"The component of type {typeof(T)} could not be found on {go}");
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return res;
        }

        [Pure]
        public static bool GetFeatureOrThrow(this InputDevice device, InputFeatureUsage<bool> feature) {
            if (!device.TryGetFeatureValue(feature, out var value)) {
                throw new UnityException($"Could not get bool feature {feature.name} from {device.name}.");
            }

            return value;
        }

        public static Option<bool> TryGetFeatureValue(this InputDevice device, InputFeatureUsage<bool> feature) =>
            !device.TryGetFeatureValue(feature, out var value) ? Option<bool>.None : value;

        [Pure]
        public static Vector2 GetFeatureOrThrow(this InputDevice device, InputFeatureUsage<Vector2> feature) {
            if (!device.TryGetFeatureValue(feature, out var value)) {
                throw new UnityException($"Could not get Vector2 feature {feature.name} from {device.name}.");
            }

            return value;
        }

        [Pure]
        public static bool IsNotNull<T>(this T value) where T : class {
            return !Util.IsNull(value);
        }

        [Pure]
        internal static Option<RaycastHit> FirstHit(this XRRayInteractor xrRayInteractor) =>
            xrRayInteractor.GetCurrentRaycastHit(out var firstHit) ? firstHit : Option<RaycastHit>.None;

        [Pure]
        private static Vector2 WorldToLocalPosition(this RectTransform rectTransform, Vector3 world) =>
            rectTransform.InverseTransformPoint(world);

        [Pure]
        internal static Option<Vector2> HitLocalPosition(this RaycastHit hit, RectTransform rect) =>
            rect.transform.Equals(hit.transform) ? rect.WorldToLocalPosition(hit.point) : Option<Vector2>.None;

        [Pure]
        internal static Vector2 InvertY(this Vector2 vector2) => vector2.WithY(y => -y);

        [Pure]
        internal static Option<T> AsOption<T>(this Component comp, Search where = Search.InObjectOnly) where T : class
            => comp.As<T>(where) ?? Option<T>.None;

        [Pure]
        internal static Option<T> AsOption<T>(this GameObject go, Search where = Search.InObjectOnly) where T : class
            => go.As<T>(where) ?? Option<T>.None;
    }
}