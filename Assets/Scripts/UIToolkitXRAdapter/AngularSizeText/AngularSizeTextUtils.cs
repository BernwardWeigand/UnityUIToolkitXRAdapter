using System.Collections.Generic;
using System.Linq;
using CoreLibrary;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    internal static class AngularSizeTextUtils {
        private const string Pixel = "px";
        private const string Percent = "%";

        private static string ExtractStylesAsString(IUxmlAttributes uxmlAttributes) {
            if (!uxmlAttributes.TryGetAttributeValue("style", out var stylesAsString)) {
                throw new UnityException(
                    $"Could not read the required style string from an {nameof(AngularSizeLabel)}.");
            }

            return stylesAsString;
        }

        private static IReadOnlyDictionary<string, string> ToStylesDictionary(IUxmlAttributes uxmlAttributes) =>
            ToDict(ExtractStylesAsString(uxmlAttributes));

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

        internal static Length ExtractFontSize(IUxmlAttributes uxmlAttributes) =>
            ExtractFontSize(ToStylesDictionary(uxmlAttributes));

        private static Length ExtractFontSize(IReadOnlyDictionary<string, string> stylesAsDict) {
            var fontSize = ExtractLength(stylesAsDict, "font-size");

            if (fontSize.HasValue) {
                return fontSize.Value;
            }

            throw new UnityException($"Could not read font size for an {nameof(IAngularSizeText<VisualElement>)}.");
        }

        internal static bool IsHigherThan(float heightInPixel, IResolvedStyle resolvedStyle) =>
            heightInPixel.IsNearlyOrLargerThan(resolvedStyle.ContentHeight());

        internal static bool IsWiderThan(float widthInPixel, IResolvedStyle resolvedStyle) =>
            widthInPixel.IsNearlyOrLargerThan(resolvedStyle.ContentWidth());
    }
}