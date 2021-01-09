using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    public sealed class AngularSizeLabel : Label, IAngularSizeText<AngularSizeLabel> {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once InconsistentNaming
        public new static readonly string ussClassName = "angular-size-label";

        AngularSizeLabel IAngularSizeText<AngularSizeLabel>.AsTextElement() => this;

        bool IAngularSizeText.HasToBeCulledWhenCannotExpand { get; set; }

        float IAngularSizeText.AngularTextHeight { get; set; }

        Length? IAngularSizeText.InitialFontHeight { get; set; }

        public new class UxmlFactory : UxmlFactory<AngularSizeLabel, UxmlTraits> { }

        public new class UxmlTraits : AngularSizeTextUxmlTraits { }

        public AngularSizeLabel() : this(string.Empty) { }

        // ReSharper disable once MemberCanBePrivate.Global
        public AngularSizeLabel(string text) {
            AddToClassList(ussClassName);

            this.text = text;
        }
    }
}