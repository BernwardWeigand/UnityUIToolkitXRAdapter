using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    internal interface IAngularResizingElement : ICullableElement {
        float AngularSizeHeight { get; set; }
        Length? InitialHeight { get; set; }

        float? AngularSizeWidth { get; set; }
        Length? InitialWidth { get; set; }
    }

    internal interface IAngularResizingElement<out T> : IAngularResizableElement<T>, IAngularResizingElement
        where T : VisualElement { }
}