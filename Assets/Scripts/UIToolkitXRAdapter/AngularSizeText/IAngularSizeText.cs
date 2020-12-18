using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    internal interface IAngularSizeText {
        // TODO may refactor towards https://en.wikipedia.org/wiki/Law_of_cosines#Applications
        internal void ResizeByTrigonometricRatios(float distanceToCamera, float pixelPerMeter);
    }

    internal interface IAngularSizeText<out T> : IAngularSizeText where T : VisualElement {
        internal T AsVisualElement();
    }
}