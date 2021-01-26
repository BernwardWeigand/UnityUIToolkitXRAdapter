using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularResizing {
    internal interface IAngularResizableElement<out T> where T : VisualElement {
        /// <summary>
        /// Resizes the element properly
        /// </summary>
        /// <param name="distanceToCamera">the distance to the camera in meters</param>
        /// <param name="pixelPerMeter">the pixel per meter ratio</param>
        void Resize(float distanceToCamera, float pixelPerMeter);

        T AsVisualElement();
    }

    // ReSharper disable once IdentifierTypo
    internal interface ICullableElement {
        bool HasToBeCulledWhenCannotExpand { get; set; }
    }
}