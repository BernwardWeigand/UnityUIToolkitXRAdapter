using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreLibrary;
using JetBrains.Annotations;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;
using static UnityEngine.UIElements.Visibility;
using static UnityEngine.UIElements.VisualElement.MeasureMode;

namespace UIToolkitXRAdapter.AngularSizeText {
    internal static class AngularSizeTextUtils {
        internal static Length? ExtractFontSize(IUxmlAttributes uxmlAttributes) {
            var stylesDictionary = ToStylesDictionary(uxmlAttributes);
            return stylesDictionary == null ? (Length?) null : ExtractFontSize(stylesDictionary);
        }

        /// <summary>
        /// TODO add docs
        /// </summary>
        /// <remarks>
        /// TODO when unity supports default implementation of interfaces move it to <see cref="IAngularSizeText{T}"/>>
        /// </remarks>
        /// <param name="element"></param>
        /// <param name="distanceToCamera"></param>
        /// <param name="pixelPerMeter"></param>
        /// <typeparam name="T"></typeparam>
        internal static void ResizeTextByTrigonometricRatios<T>(this IAngularSizeText<T> element,
            float distanceToCamera, float pixelPerMeter) where T : TextElement {
            // TODO may refactor towards https://en.wikipedia.org/wiki/Law_of_cosines#Applications
            // see https://en.wikipedia.org/wiki/Trigonometry#Trigonometric_ratios
            var angularHeightInPixel =
                distanceToCamera * Tan(Deg2Rad * (element.AngularTextHeight / 60)) / pixelPerMeter;

            var textElement = element.AsTextElement();

            var resolvedStyle = textElement.resolvedStyle;

            var currentFontHeight = resolvedStyle.fontSize;
            var newDimensions = textElement.MeasureTextSize(textElement.text, resolvedStyle.width, AtMost,
                angularHeightInPixel, AtMost);

            var initialFontHeight = element.InitialFontHeight;
            if (initialFontHeight.HasValue && initialFontHeight.Value.IsHigherThan(newDimensions.y, resolvedStyle) &&
                angularHeightInPixel < currentFontHeight) {
                // this shouldn't shrink
                return;
            }

            var visibility = resolvedStyle.visibility;
            if (IsHigherThan(newDimensions.y, resolvedStyle) || IsWiderThan(newDimensions.x, resolvedStyle)) {
                if (element.HasToBeCulledWhenCannotExpand && visibility.Equals(Visible)) {
                    // this too far way
                    textElement.style.visibility = Hidden;
                    return;
                }

                if (angularHeightInPixel > currentFontHeight) {
                    // this shouldn't expand
                    return;
                }
            }

            if (angularHeightInPixel < currentFontHeight && element.HasToBeCulledWhenCannotExpand &&
                visibility.Equals(Hidden)) {
                // this can be displayed now
                textElement.style.visibility = Visible;
            }

            textElement.style.fontSize = new StyleLength(angularHeightInPixel);
            textElement.MarkDirtyRepaint();
        }

        private const string Pixel = "px";
        private const string Percent = "%";

        [CanBeNull]
        private static string ExtractStylesAsString(IUxmlAttributes uxmlAttributes) {
            return !uxmlAttributes.TryGetAttributeValue("style", out var stylesAsString) ? null : stylesAsString;
        }

        [CanBeNull]
        private static IReadOnlyDictionary<string, string> ToStylesDictionary(IUxmlAttributes uxmlAttributes) {
            var stylesAsString = ExtractStylesAsString(uxmlAttributes);
            return stylesAsString == null ? null : ToDict(stylesAsString);
        }

        private static IReadOnlyDictionary<string, string> ToDict(string stylesAsString) {
            return stylesAsString.Split(';').Where(UtilityExtensions.IsNotEmpty).Select(styleProperty => {
                var split = styleProperty.Split(':');
                if (split.Length < 2) {
                    throw new UnityException(
                        $"Not enough data in the style information {styleProperty} for an {nameof(AngularSizeText)}.");
                }

                return new KeyValuePair<string, string>(split[0].Trim(), split[1].Trim());
            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static Length? ExtractLength(IReadOnlyDictionary<string, string> stylesAsDict, string propertyName) {
            if (!stylesAsDict.TryGetValue(propertyName, out var lengthString)) {
                return null;
            }

            LengthUnit lengthUnit;
            float lengthSize;
            if (lengthString.EndsWith(Pixel)) {
                lengthUnit = LengthUnit.Pixel;
                if (!float.TryParse(lengthString.Substring(0, lengthString.Length - Pixel.Length), out lengthSize)) {
                    return null;
                }
            }
            else if (lengthString.EndsWith(Percent)) {
                lengthUnit = LengthUnit.Percent;
                if (!float.TryParse(lengthString.Substring(0, lengthString.Length - Percent.Length), out lengthSize)) {
                    return null;
                }
            }
            else {
                return null;
            }

            return new Length(lengthSize, lengthUnit);
        }

        private static Length ExtractFontSize(IReadOnlyDictionary<string, string> stylesAsDict) {
            var fontSize = ExtractLength(stylesAsDict, "font-size");

            if (fontSize.HasValue) {
                return fontSize.Value;
            }

            throw new UnityException($"Could not read font size for an {nameof(IAngularSizeText<TextElement>)}.");
        }

        private static bool IsHigherThan(float heightInPixel, IResolvedStyle resolvedStyle) =>
            heightInPixel.IsNearlyOrLargerThan(resolvedStyle.ContentHeight());

        private static bool IsWiderThan(float widthInPixel, IResolvedStyle resolvedStyle) =>
            widthInPixel.IsNearlyOrLargerThan(resolvedStyle.ContentWidth());
    }
}