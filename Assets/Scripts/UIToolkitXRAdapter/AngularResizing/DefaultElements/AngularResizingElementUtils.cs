using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.AngularResizingUtils;
using static UnityEngine.UIElements.Visibility;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    internal static class AngularResizingElementUtils {
        internal static Length? ExtractHeight(IReadOnlyDictionary<string, string> stylesAsDict) =>
            ExtractLength(stylesAsDict, "height");

        internal static Length? ExtractWidth(IReadOnlyDictionary<string, string> stylesAsDict) =>
            ExtractLength(stylesAsDict, "width");

        internal static float? TryGetValueFromBag(this UxmlFloatAttributeDescription floatAttribute,
            IUxmlAttributes bag, CreationContext cc) {
            var result = 0f;
            return floatAttribute.TryGetValueFromBag(bag, cc, ref result) ? result : (float?) null;
        }

        /// <inheritdoc cref="IAngularResizableElement{T}"/>
        /// <remarks>
        /// TODO when unity supports default implementations move it to <see cref="IAngularResizingElement{T}"/>>
        /// </remarks>
        internal static void Resize<T>(this IAngularResizingElement<T> element, float distanceToCamera,
            float pixelPerMeter) where T : VisualElement {
            var newHeightInPixels = CalculateSizeInPixel(element.AngularSizeHeight, distanceToCamera, pixelPerMeter);

            var visualElement = element.AsVisualElement();

            var currentHeight = visualElement.contentRect.width;
            var currentWidth = visualElement.contentRect.width;

            float newWidthInPixels;
            if (element.AngularSizeWidth.HasValue) {
                var angularSizeWidth = element.AngularSizeWidth.Value;
                if (angularSizeWidth.IsNotNearlyZero()) {
                    newWidthInPixels = CalculateSizeInPixel(angularSizeWidth, distanceToCamera, pixelPerMeter);
                }
                else {
                    newWidthInPixels = currentWidth / currentHeight * newHeightInPixels;
                }
            }
            else {
                newWidthInPixels = currentWidth / currentHeight * newHeightInPixels;
            }

            if (newHeightInPixels < currentHeight && element.InitialHeight.HasValue &&
                element.InitialHeight.Value.ContentIsHigherThan(newHeightInPixels, visualElement)) {
                // this shouldn't shrink
                return;
            }

            if (newWidthInPixels < currentWidth && element.InitialWidth.HasValue) {
                var iniWidth = element.InitialWidth.Value;
                if (iniWidth.value.IsNotNearlyZero() && iniWidth.ContentIsWiderThan(newWidthInPixels, visualElement)) {
                    // this shouldn't shrink
                    return;
                }
            }

            var maxHeight = visualElement.PossibleHeight();
            var maxWidth = visualElement.PossibleWidth();

            var hasToBeCulledWhenCannotExpand = element.HasToBeCulledWhenCannotExpand;

            if (newHeightInPixels >= maxHeight || newWidthInPixels >= maxWidth) {
                if (hasToBeCulledWhenCannotExpand && visualElement.resolvedStyle.visibility.Equals(Visible)) {
                    // this too far way
                    visualElement.style.visibility = Hidden;
                    return;
                }

                if (newHeightInPixels > currentHeight || newWidthInPixels > currentWidth) {
                    // this shouldn't expand
                    return;
                }
            }

            if ((newHeightInPixels < maxHeight || newWidthInPixels < maxWidth) && hasToBeCulledWhenCannotExpand &&
                visualElement.resolvedStyle.visibility.Equals(Hidden)) {
                // this can be displayed now
                visualElement.style.visibility = Visible;
            }

            visualElement.style.height = newHeightInPixels;
            visualElement.style.width = newWidthInPixels;
        }

        // TODO refactor it and use the other elements height & height, if they are in a line
        [Pure]
        private static float PossibleHeight(this VisualElement ve) =>
            ve.parent.contentRect.height - (ve.resolvedStyle.marginTop + ve.resolvedStyle.marginBottom);

        [Pure]
        private static float PossibleWidth(this VisualElement ve) =>
            ve.parent.contentRect.width - (ve.resolvedStyle.marginLeft + ve.resolvedStyle.marginRight);

        [Pure]
        private static bool ContentIsWiderThan(this Length l, float comparedWidth, VisualElement veOfLength) {
            if (l.unit == LengthUnit.Pixel && l.value > comparedWidth) {
                return true;
            }

            return l.unit == LengthUnit.Percent && veOfLength.contentRect.width * l.AsDecimal() > comparedWidth;
        }
    }
}