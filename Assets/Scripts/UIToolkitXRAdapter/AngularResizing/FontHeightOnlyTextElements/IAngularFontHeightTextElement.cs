using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    internal interface IAngularFontHeightTextElement : ICullableElement {
        float AngularFontHeight { get; set; }

        Length? InitialFontHeight { get; set; }
    }

    internal interface IAngularFontHeightTextElement<out T> : IAngularResizableElement<T>,
        IAngularFontHeightTextElement where T : TextElement {
        T AsTextElement();
    }
}