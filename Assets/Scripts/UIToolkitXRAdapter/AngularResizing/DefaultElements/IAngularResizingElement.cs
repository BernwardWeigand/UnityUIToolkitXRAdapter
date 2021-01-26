using LanguageExt;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    internal interface IAngularResizingElement : ICullableElement {
        float AngularSizeHeight { get; set; }
        Option<Length> InitialHeight { get; set; }

        Option<float> AngularSizeWidth { get; set; }
        Option<Length> InitialWidth { get; set; }
    }

    internal interface IAngularResizingElement<out T> : IAngularResizableElement<T>, IAngularResizingElement
        where T : VisualElement { }
}