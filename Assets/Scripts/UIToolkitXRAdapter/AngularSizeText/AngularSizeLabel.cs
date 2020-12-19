using UIToolkitXRAdapter.Utils;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;
using static UnityEngine.UIElements.VisualElement.MeasureMode;

namespace UIToolkitXRAdapter.AngularSizeText {
    public sealed class AngularSizeLabel : Label, IAngularSizeText<AngularSizeLabel> {
        private float _angularHeight;

        private Length? _initialFontHeight;

        public new static readonly string ussClassName = "angular-size-label";


        public new class UxmlFactory : UxmlFactory<AngularSizeLabel, UxmlTraits> { }

        public new class UxmlTraits : TextElement.UxmlTraits {
            private UxmlFloatAttributeDescription m_TextSize = new UxmlFloatAttributeDescription {
                name = "size-in-arc-minutes",
                defaultValue = 90f
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var angularSizeLabel = (AngularSizeLabel) ve;
                angularSizeLabel._angularHeight = m_TextSize.GetValueFromBag(bag, cc);

                var initialFontHeight = AngularSizeTextUtils.ExtractFontSize(bag);
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
                // this shouldn't be smaller
                return;
            }

            if ((AngularSizeTextUtils.IsHigherThan(newDimensions.y, resolvedStyle) ||
                 AngularSizeTextUtils.IsWiderThan(newDimensions.x, resolvedStyle)) &&
                angularHeightInPixel > currentFontHeight) {
                // this shouldn't be larger
                return;
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