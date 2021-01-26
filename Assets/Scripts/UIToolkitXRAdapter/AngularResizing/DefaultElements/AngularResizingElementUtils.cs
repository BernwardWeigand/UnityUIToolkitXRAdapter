using System.Collections.Generic;
using JetBrains.Annotations;
using LanguageExt;
using UIToolkitXRAdapter.Utils;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.AngularResizingUtils;
using static UnityEngine.UIElements.Visibility;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    internal static class AngularResizingElementUtils {
        internal static Option<Length> ExtractHeight(IReadOnlyDictionary<string, string> stylesAsDict) =>
            ExtractLength(stylesAsDict, "height");

        internal static Option<Length> ExtractWidth(IReadOnlyDictionary<string, string> stylesAsDict) =>
            ExtractLength(stylesAsDict, "width");

        internal static Option<float> TryGetValueFromBag(this UxmlFloatAttributeDescription floatAttribute,
            IUxmlAttributes bag, CreationContext cc) {
            var result = 0f;
            return floatAttribute.TryGetValueFromBag(bag, cc, ref result) ? result : Option<float>.None;
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

            var newWidthInPixels = element.AngularSizeWidth.Filter(angularWidth => angularWidth.IsNotNearlyZero())
                .Match(angularSizeWidth => CalculateSizeInPixel(angularSizeWidth, distanceToCamera, pixelPerMeter),
                    () => currentWidth / currentHeight * newHeightInPixels);

            if (element.InitialHeight.Filter(iH => iH.ContentIsHigherThan(newHeightInPixels, visualElement)).IsSome
                && newHeightInPixels < currentHeight || newWidthInPixels < currentWidth &&
                element.InitialWidth.Filter(initialWidth => initialWidth.value.IsNotNearlyZero())
                    .Filter(initialWidth => initialWidth.ContentIsWiderThan(newWidthInPixels, visualElement)).IsSome) {
                // this shouldn't shrink
                return;
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

            visualElement.style.height = new StyleLength(newHeightInPixels);
            visualElement.style.width = new StyleLength(newWidthInPixels);
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