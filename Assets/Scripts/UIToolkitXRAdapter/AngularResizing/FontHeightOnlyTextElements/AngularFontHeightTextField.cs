using UnityEngine.UIElements;

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

        // TODO optimized implementation
        public void Resize(float distanceToCamera, float pixelPerMeter) =>
            AngularFontHeightTextElementsUtils.Resize(this, distanceToCamera, pixelPerMeter);

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