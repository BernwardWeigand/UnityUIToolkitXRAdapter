﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary;
using JetBrains.Annotations;
using LanguageExt;
using UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing {
    internal static class AngularResizingUtils {
        /// <summary>
        /// Calculates the size in pixels via <a href="https://en.wikipedia.org/wiki/Trigonometry#Trigonometric_ratios">
        /// trigonometric ratios</a> based on an angular size.
        /// </summary>
        /// <remarks>
        /// TODO may refactor towards <a href="https://en.wikipedia.org/wiki/Law_of_cosines#Applications">cosines</a>
        /// </remarks>
        /// <param name="angularSize">the angular size in arc minutes</param>
        /// <param name="distanceToCamera">the distance to the camera in meters</param>
        /// <param name="pixelPerMeter">the pixel per meter ratio</param>
        /// <returns></returns>
        internal static float CalculateSizeInPixel(float angularSize, float distanceToCamera, float pixelPerMeter) =>
            distanceToCamera * Mathf.Tan(Mathf.Deg2Rad * (angularSize / 60)) / pixelPerMeter;


        private static IReadOnlyDictionary<string, string> ToDict(string stylesAsString) {
            return stylesAsString.Split(';').Where(UtilityExtensions.IsNotEmpty).Select(styleProperty => {
                var split = styleProperty.Split(':');
                if (split.Length < 2) {
                    throw new UnityException($"Not enough data in the style information {styleProperty} for an " +
                                             $"{nameof(IAngularFontHeightTextElement<TextElement>)}.");
                }

                return new KeyValuePair<string, string>(split[0].Trim(), split[1].Trim());
            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static Option<IReadOnlyDictionary<string, string>> ToStylesDictionary(this IUxmlAttributes attributes)
            => attributes.ExtractStylesAsString().Map(ToDict);

        private static Option<string> ExtractStylesAsString(this IUxmlAttributes uxmlAttributes) =>
            !uxmlAttributes.TryGetAttributeValue("style", out var stylesAsString)
                ? Option<string>.None
                : stylesAsString;

        private const string Pixel = "px";
        private const string Percent = "%";

        [Pure]
        internal static Option<Length> ExtractLength(IReadOnlyDictionary<string, string> stylesAsDict, string name) {
            if (!stylesAsDict.TryGetValue(name, out var lengthString)) {
                return Option<Length>.None;
            }

            LengthUnit lengthUnit;
            float lengthSize;
            if (lengthString.EndsWith(Pixel)) {
                lengthUnit = LengthUnit.Pixel;
                if (!float.TryParse(lengthString.Substring(0, lengthString.Length - Pixel.Length), out lengthSize)) {
                    return Option<Length>.None;
                }
            }
            else if (lengthString.EndsWith(Percent)) {
                lengthUnit = LengthUnit.Percent;
                if (!float.TryParse(lengthString.Substring(0, lengthString.Length - Percent.Length), out lengthSize)) {
                    return Option<Length>.None;
                }
            }
            else {
                return Option<Length>.None;
            }

            return new Length(lengthSize, lengthUnit);
        }

        internal static float AsDecimal(this Length length) {
            if (length.unit != LengthUnit.Percent) {
                throw new UnityException("To convert a Length to a decimal it's unit has to be in percent.");
            }

            return length.value / 100;
        }

        [Pure]
        internal static bool ContentIsHigherThan(this Length l, float comparedHeight, VisualElement veOfLength) {
            if (l.unit == LengthUnit.Pixel && l.value > comparedHeight) {
                return true;
            }
            // TODO may calculate the the height differently
            return l.unit == LengthUnit.Percent && veOfLength.contentRect.height * l.AsDecimal() > comparedHeight;
        }

        [Pure]
        internal static T Max<T>(this T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0 ? b : a;

        [Pure]
        internal static bool IsNotNearlyZero(this float value) => !value.IsNearly(0);
    }
}