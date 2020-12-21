using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    internal interface IAngularSizeText {
        // TODO may refactor towards https://en.wikipedia.org/wiki/Law_of_cosines#Applications
        // TODO when unity supports default implementation of interface create a IAngularSizeTextElement
        // with default implementation to reduce code duplication
        internal void ResizeByTrigonometricRatios(float distanceToCamera, float pixelPerMeter);
    }

    internal interface IAngularSizeText<out T> : IAngularSizeText where T : VisualElement {
        internal T AsVisualElement();
    }
}