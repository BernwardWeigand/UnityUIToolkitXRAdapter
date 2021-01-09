using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    internal interface IAngularSizeText {
        internal bool HasToBeCulledWhenCannotExpand { get; set; }

        internal float AngularTextHeight { get; set; }

        internal Length? InitialFontHeight { get; set; }
    }

    internal interface IAngularSizeText<out T> : IAngularSizeText where T : TextElement {
        internal T AsTextElement();
    }
}