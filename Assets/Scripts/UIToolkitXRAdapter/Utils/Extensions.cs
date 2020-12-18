using System;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.Utils {
    public static class Extensions {
        public static float Abs(this float value) => Math.Abs(value);

        public static bool IsNearly(this float value, float other, float threshold = float.Epsilon)
            => (value - other).Abs() <= threshold;

        public static float ContentHeight(this IResolvedStyle resolvedStyle) =>
            resolvedStyle.height - resolvedStyle.marginTop - resolvedStyle.borderTopWidth - resolvedStyle.paddingTop -
            resolvedStyle.paddingBottom - resolvedStyle.borderBottomWidth - resolvedStyle.marginBottom;

        public static float ContentWidth(this IResolvedStyle resolvedStyle) =>
            resolvedStyle.width - resolvedStyle.marginLeft - resolvedStyle.borderLeftWidth - resolvedStyle.paddingLeft -
            resolvedStyle.paddingRight - resolvedStyle.borderRightWidth - resolvedStyle.marginRight;

        public static bool IsNearlyOrLargerThan(this float value, float other) =>
            value >= other; //|| value.IsNearly(other);

        public static bool IsHigherThan(this Length length, float heightInPixel, IResolvedStyle rsStyle) {
            if (length.unit == LengthUnit.Pixel && length.value > heightInPixel) {
                return true;
            }

            return length.unit == LengthUnit.Percent &&
                   (rsStyle.ContentHeight() * (length.value / 100)).IsNearlyOrLargerThan(heightInPixel);
        }

        public static T Max<T>(this T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0 ? b : a;
    }
}