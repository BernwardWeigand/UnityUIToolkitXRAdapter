using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularSizeText.AngularSizeTextUtils;
using static UnityEngine.Mathf;
using static UnityEngine.UIElements.Visibility;
using static UnityEngine.UIElements.VisualElement.MeasureMode;

namespace UIToolkitXRAdapter.AngularSizeText {
    public sealed class AngularSizeLabel : Label, IAngularSizeText<AngularSizeLabel> {
        private float _angularHeight;

        private bool _cullWhenCannotExpand;

        private Length? _initialFontHeight;

        public new static readonly string ussClassName = "angular-size-label";


        public new class UxmlFactory : UxmlFactory<AngularSizeLabel, UxmlTraits> { }

        public new class UxmlTraits : TextElement.UxmlTraits {
            private UxmlFloatAttributeDescription m_angularTextSize = new UxmlFloatAttributeDescription {
                name = "size-in-arc-minutes",
                defaultValue = 90f
            };

            private UxmlBoolAttributeDescription m_cullWhenCannotExpand = new UxmlBoolAttributeDescription {
                name = "cull-when-cannot-expand",
                defaultValue = false
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var angularSizeLabel = (AngularSizeLabel) ve;
                angularSizeLabel._angularHeight = m_angularTextSize.GetValueFromBag(bag, cc);
                angularSizeLabel._cullWhenCannotExpand = m_cullWhenCannotExpand.GetValueFromBag(bag, cc);

                var initialFontHeight = ExtractFontSize(bag);
                if (initialFontHeight.HasValue) {
                    angularSizeLabel._initialFontHeight = initialFontHeight.Value;
                }
            }
        }

        public AngularSizeLabel() : this(string.Empty) { }

        public AngularSizeLabel(string text) {
            AddToClassList(ussClassName);

            this.text = text;
        }

        void IAngularSizeText.ResizeByTrigonometricRatios(float distanceToCamera, float pixelPerMeter) {
            // see https://en.wikipedia.org/wiki/Trigonometry#Trigonometric_ratios
            var angularHeightInPixel = distanceToCamera * Tan(Deg2Rad * (_angularHeight / 60)) / pixelPerMeter;
            var currentFontHeight = resolvedStyle.fontSize;
            var newDimensions = MeasureTextSize(text, resolvedStyle.width, AtMost, angularHeightInPixel, AtMost);

            if (_initialFontHeight.HasValue && _initialFontHeight.Value.IsHigherThan(newDimensions.y, resolvedStyle) &&
                angularHeightInPixel < currentFontHeight) {
                // this shouldn't shrink
                return;
            }

            var visibility = resolvedStyle.visibility;
            if (IsHigherThan(newDimensions.y, resolvedStyle) || IsWiderThan(newDimensions.x, resolvedStyle)) {
                if (_cullWhenCannotExpand && visibility.Equals(Visible)) {
                    // this too far way
                    style.visibility = Hidden;
                    return;
                }

                if (angularHeightInPixel > currentFontHeight) {
                    // this shouldn't expand
                    return;
                }
            }

            if (angularHeightInPixel < currentFontHeight && _cullWhenCannotExpand && visibility.Equals(Hidden)) {
                // this can be displayed now
                style.visibility = Visible;
            }

            style.fontSize = new StyleLength(angularHeightInPixel);

            // This re-assignment is necessary to trigger the re-render event, thank you Unity!
            var old = text;
            text = string.Empty;
            text = old;
        }

        AngularSizeLabel IAngularSizeText<AngularSizeLabel>.AsVisualElement() => this;
    }
}