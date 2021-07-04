using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.DefaultElements.AngularResizingElementUtils;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    public class AngularResizingVisualElement : VisualElement, IAngularResizingElement<VisualElement> {
        // ReSharper disable once InconsistentNaming
        public static readonly string ussClassName = "angular-size-element";

        VisualElement IAngularResizableElement<VisualElement>.AsVisualElement() => this;

        public bool HasToBeCulledWhenCannotExpand { get; set; }

        public float AngularSizeHeight { get; set; }
        Length? IAngularResizingElement.InitialHeight { get; set; }

        public float? AngularSizeWidth { get; set; }
        Length? IAngularResizingElement.InitialWidth { get; set; }

        // ReSharper disable once UnusedType.Global
        public new class UxmlFactory :
            UxmlFactory<AngularResizingVisualElement, AngularResizingVisualElementUxmlTraits> { }

        public void Resize(float distanceToCamera, float pixelPerMeter) =>
            AngularResizingElementUtils.Resize(this, distanceToCamera, pixelPerMeter);
    }
}