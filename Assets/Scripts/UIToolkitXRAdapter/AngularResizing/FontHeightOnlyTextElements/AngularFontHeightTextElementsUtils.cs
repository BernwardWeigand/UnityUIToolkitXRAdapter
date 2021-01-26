using System.Collections.Generic;
using JetBrains.Annotations;
using LanguageExt;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.AngularResizingUtils;
using static UnityEngine.UIElements.Visibility;
using static UnityEngine.UIElements.VisualElement.MeasureMode;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    internal static class AngularFontHeightTextElementsUtils {
        [Pure]
        internal static Option<Length> ExtractFontSize(IUxmlAttributes uxmlAttributes) =>
            uxmlAttributes.ToStylesDictionary().Map(ExtractFontSize);


        /// <inheritdoc cref="IAngularResizableElement{T}"/>
        /// <remarks>
        /// TODO when unity supports default implementations move it to <see cref="IAngularFontHeightTextElement{T}"/>>
        /// </remarks>
        internal static void Resize<T>(this IAngularFontHeightTextElement<T> element, float distanceToCamera,
            float pixelPerMeter) where T : TextElement {
            var angularFontHeight = CalculateSizeInPixel(element.AngularFontHeight, distanceToCamera, pixelPerMeter);

            var textElement = element.AsTextElement();

            var currentFontHeight = textElement.resolvedStyle.fontSize;
            var newBounds = textElement.MeasureTextSize(textElement.text, textElement.resolvedStyle.width, AtMost,
                angularFontHeight, AtMost);

            if (element.InitialFontHeight.Filter(initialFontHeight => initialFontHeight.value.IsNotNearlyZero())
                    .Filter(initialFontHeight => initialFontHeight.ContentIsHigherThan(newBounds.y, textElement)).IsSome
                && angularFontHeight < currentFontHeight) {
                // this shouldn't shrink
                return;
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
        private static Length ExtractFontSize(IReadOnlyDictionary<string, string> stylesAsDict)
            => ExtractLength(stylesAsDict, "font-size")
                .Some(Prelude.identity)
                .None(() => throw new UnityException(
                    $"Could not read font size for an {nameof(IAngularFontHeightTextElement<TextElement>)}."));

        [Pure]
        private static bool IsHigherThanContent(this float heightInPixel, VisualElement visualElement) =>
            heightInPixel >= visualElement.contentRect.height;

        [Pure]
        private static bool IsWiderThanContent(this float widthInPixel, VisualElement visualElement) =>
            widthInPixel >= visualElement.contentRect.width;
    }
}