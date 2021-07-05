using UnityEngine;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.AngularResizingUtils;
using static UnityEngine.UIElements.Visibility;
using static UnityEngine.UIElements.VisualElement.MeasureMode;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    public sealed class AngularFontHeightTextField : TextField, IAngularFontHeightTextElement<Label> {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once InconsistentNaming
        public new static readonly string ussClassName = "angular-size-text-field";

        Label IAngularFontHeightTextElement<Label>.AsTextElement() => labelElement;
        Label IAngularResizableElement<Label>.AsVisualElement() => labelElement;

        public bool HasToBeCulledWhenCannotExpand { get; set; }
        public float AngularFontHeight { get; set; }
        public Length? InitialFontHeight { get; set; }

        public void Resize(float distanceToCamera, float pixelPerMeter) {
            var angularFontHeight = CalculateSizeInPixel(AngularFontHeight, distanceToCamera, pixelPerMeter);

            var availableWidth = resolvedStyle.width;
            //Debug.Log("availableWidth: " + availableWidth);

            var textInputNewBounds = MeasureTextSize(text, availableWidth, AtMost, angularFontHeight, AtMost);
            var labelNewBounds = labelElement.MeasureTextSize(labelElement.text,
                availableWidth - textInputNewBounds.y, AtMost, angularFontHeight, AtMost);
            var newBounds =
                new Vector2(labelNewBounds.y.Max(textInputNewBounds.y), textInputNewBounds.x + labelNewBounds.x);
            //Debug.Log("newBounds: " + newBounds);
            var currentFontHeight = resolvedStyle.fontSize;

            if (InitialFontHeight.HasValue) {
                var initHeight = InitialFontHeight.Value;
                if (initHeight.value.IsNotNearlyZero() && initHeight.ContentIsHigherThan(newBounds.y, this) &&
                    angularFontHeight < currentFontHeight) {
                    // this shouldn't shrink
                    return;
                }
            }

            var visibility = resolvedStyle.visibility;

            if (newBounds.y.IsHigherThanContent(this) || newBounds.x.IsWiderThanContent(this)) {
                if (HasToBeCulledWhenCannotExpand && visibility.Equals(Visible)) {
                    // this too far way
                    style.visibility = Hidden;
                    return;
                }

                if (angularFontHeight > currentFontHeight) {
                    // this shouldn't expand
                    return;
                }
            }

            if (angularFontHeight < currentFontHeight && HasToBeCulledWhenCannotExpand && visibility.Equals(Hidden)) {
                // this can be displayed now
                style.visibility = Visible;
            }
            // TODO fix label sizing bug 
            // labelElement.style.width = (availableWidth - newBounds.x) / 2 + labelNewBounds.x;
            style.fontSize = angularFontHeight;
            MarkDirtyRepaint();
        }

        // ReSharper disable once UnusedType.Global
        public new class UxmlFactory : UxmlFactory<AngularFontHeightTextField, AngularFontHeightTextFieldUxmlTraits> { }

        // ReSharper disable once UnusedMember.Global
        public AngularFontHeightTextField() : this(null) { }

        // ReSharper disable once MemberCanBePrivate.Global
        public AngularFontHeightTextField(string label) : base(label) => AddToClassList(ussClassName);

        // ReSharper disable once UnusedMember.Global
        public AngularFontHeightTextField(int maxLength, bool multiline, bool isPasswordField, char maskChar) :
            base(null, maxLength, multiline, isPasswordField, maskChar) => AddToClassList(ussClassName);

        // ReSharper disable once UnusedMember.Global
        public AngularFontHeightTextField(string label, int maxLength, bool multiline, bool isPasswordField,
            char maskChar) : base(label, maxLength, multiline, isPasswordField, maskChar) =>
            AddToClassList(ussClassName);
    }
}