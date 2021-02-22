using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    public sealed class AngularFontHeightLabel : Label, IAngularFontHeightTextElement<AngularFontHeightLabel> {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once InconsistentNaming
        public new static readonly string ussClassName = "angular-size-text-label";

        AngularFontHeightLabel IAngularFontHeightTextElement<AngularFontHeightLabel>.AsTextElement() => this;

        AngularFontHeightLabel IAngularResizableElement<AngularFontHeightLabel>.AsVisualElement() => this;
        public bool HasToBeCulledWhenCannotExpand { get; set; }

        public float AngularFontHeight { get; set; }

        Length? IAngularFontHeightTextElement.InitialFontHeight { get; set; }

        // ReSharper disable once UnusedType.Global
        public new class UxmlFactory : UxmlFactory<AngularFontHeightLabel, UxmlTraits> { }

        public new class UxmlTraits : AngularFontHeightTextElementUxmlTraits { }

        public AngularFontHeightLabel() : this(string.Empty) { }

        // ReSharper disable once MemberCanBePrivate.Global
        public AngularFontHeightLabel(string text) {
            AddToClassList(ussClassName);

            this.text = text;
        }

        public void Resize(float distanceToCamera, float pixelPerMeter) =>
            AngularFontHeightTextElementsUtils.Resize(this, distanceToCamera, pixelPerMeter);
    }
}