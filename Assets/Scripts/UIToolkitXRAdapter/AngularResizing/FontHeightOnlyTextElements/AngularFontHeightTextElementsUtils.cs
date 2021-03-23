using System.Collections.Generic;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.AngularResizingUtils;
using static UnityEngine.UIElements.Visibility;
using static UnityEngine.UIElements.VisualElement.MeasureMode;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    internal static class AngularFontHeightTextElementsUtils {
        [Pure]
        internal static Length? ExtractFontSize(IUxmlAttributes uxmlAttributes) {
            var value = uxmlAttributes.ToStylesDictionary();
            return value.IsNull() ? (Length?) null : ExtractFontSize(value);
        }


        /// <inheritdoc cref="IAngularResizableElement{T}"/>
        /// <remarks>
        /// TODO when unity supports default implementations move it to <see cref="IAngularFontHeightTextElement{T}"/>>
        /// TODO fix bug for initial font height, if it is not visible
        /// </remarks>
        internal static void Resize<T>(this IAngularFontHeightTextElement<T> element, float distanceToCamera,
            float pixelPerMeter) where T : TextElement {
            var angularFontHeight = CalculateSizeInPixel(element.AngularFontHeight, distanceToCamera, pixelPerMeter);

            var textElement = element.AsTextElement();

            var currentFontHeight = textElement.resolvedStyle.fontSize;
            var newBounds = textElement.MeasureTextSize(textElement.text, textElement.resolvedStyle.width, AtMost,
                angularFontHeight, AtMost);

            if (element.InitialFontHeight.HasValue) {
                var initHeight = element.InitialFontHeight.Value;
                if (initHeight.value.IsNotNearlyZero() && initHeight.ContentIsHigherThan(newBounds.y, textElement) &&
                    angularFontHeight < currentFontHeight) {
                    // this shouldn't shrink
                    return;
                }
            }

            var visibility = textElement.resolvedStyle.visibility;

            if (newBounds.y.IsHigherThanContent(textElement) || newBounds.x.IsWiderThanContent(textElement)) {
                if (element.HasToBeCulledWhenCannotExpand && visibility.Equals(Visible)) {
                    // this too far way
                    textElement.style.visibility = Hidden;
                    return;
                }

                if (angularFontHeight > currentFontHeight) {
                    // this shouldn't expand
                    return;
                }
            }

            if (angularFontHeight < currentFontHeight && element.HasToBeCulledWhenCannotExpand &&
                visibility.Equals(Hidden)) {
                // this can be displayed now
                textElement.style.visibility = Visible;
            }

            textElement.style.fontSize = new StyleLength(angularFontHeight);
            textElement.MarkDirtyRepaint();
        }

        [Pure]
        private static Length ExtractFontSize(IReadOnlyDictionary<string, string> stylesAsDict) {
            var length = ExtractLength(stylesAsDict, "font-size");
            if (length.HasValue) {
                return length.Value;
            }

            throw new UnityException(
                $"Could not read font size for an {nameof(IAngularFontHeightTextElement<TextElement>)}.");
        }

        [Pure]
        private static bool IsHigherThanContent(this float heightInPixel, VisualElement visualElement) =>
            heightInPixel >= visualElement.contentRect.height;

        [Pure]
        private static bool IsWiderThanContent(this float widthInPixel, VisualElement visualElement) =>
            widthInPixel >= visualElement.contentRect.width;
    }
}