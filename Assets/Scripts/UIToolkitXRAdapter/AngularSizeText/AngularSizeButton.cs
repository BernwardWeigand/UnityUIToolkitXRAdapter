using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    public class AngularSizeButton : Button, IAngularSizeText<AngularSizeButton> {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once InconsistentNaming
        public new static readonly string ussClassName = "angular-size-text-button";
        AngularSizeButton IAngularSizeText<AngularSizeButton>.AsTextElement() => this;

        bool IAngularSizeText.HasToBeCulledWhenCannotExpand { get; set; }

        float IAngularSizeText.AngularTextHeight { get; set; }

        Length? IAngularSizeText.InitialFontHeight { get; set; }

        public new class UxmlFactory : UxmlFactory<AngularSizeButton, UxmlTraits> { }

        public new class UxmlTraits : AngularSizeTextUxmlTraits { }
        
        public AngularSizeButton() : this(string.Empty) { }

        public AngularSizeButton(string text) {
            AddToClassList(ussClassName);

            this.text = text;
        }
    }
}