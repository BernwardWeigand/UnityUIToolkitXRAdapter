using LanguageExt;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    public sealed class AngularFontHeightButton : Button, IAngularFontHeightTextElement<AngularFontHeightButton> {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once InconsistentNaming
        public new static readonly string ussClassName = "angular-size-text-button";
        AngularFontHeightButton IAngularFontHeightTextElement<AngularFontHeightButton>.AsTextElement() => this;

        AngularFontHeightButton IAngularResizableElement<AngularFontHeightButton>.AsVisualElement() => this;

        public bool HasToBeCulledWhenCannotExpand { get; set; }

        public float AngularFontHeight { get; set; }

        Option<Length> IAngularFontHeightTextElement.InitialFontHeight { get; set; }

        // ReSharper disable once UnusedType.Global
        public new class UxmlFactory : UxmlFactory<AngularFontHeightButton, UxmlTraits> { }

        public new class UxmlTraits : AngularFontHeightTextElementUxmlTraits { }

        public AngularFontHeightButton() : this(string.Empty) { }

        // ReSharper disable once MemberCanBePrivate.Global
        public AngularFontHeightButton(string text) {
            AddToClassList(ussClassName);
            this.text = text;
        }

        public void Resize(float distanceToCamera, float pixelPerMeter) =>
            AngularFontHeightTextElementsUtils.Resize(this, distanceToCamera, pixelPerMeter);
    }
}