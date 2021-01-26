using LanguageExt;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.DefaultElements.AngularResizingElementUtils;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    public class AngularResizingVisualElement : VisualElement, IAngularResizingElement<VisualElement> {
        // ReSharper disable once InconsistentNaming
        public static readonly string ussClassName = "angular-size-element";

        VisualElement IAngularResizableElement<VisualElement>.AsVisualElement() => this;

        public bool HasToBeCulledWhenCannotExpand { get; set; }

        public float AngularSizeHeight { get; set; }
        Option<Length> IAngularResizingElement.InitialHeight { get; set; }

        public Option<float> AngularSizeWidth { get; set; }
        Option<Length> IAngularResizingElement.InitialWidth { get; set; }

        // ReSharper disable once UnusedType.Global
        public new class UxmlFactory : UxmlFactory<AngularResizingVisualElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            // ReSharper disable once InconsistentNaming
            private UxmlFloatAttributeDescription m_angularHeight = new UxmlFloatAttributeDescription {
                name = "height-in-arc-minutes",
                defaultValue = 180f
            };

            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            // ReSharper disable once InconsistentNaming
            private UxmlFloatAttributeDescription m_angularWidth = new UxmlFloatAttributeDescription {
                name = "width-in-arc-minutes"
            };

            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            // ReSharper disable once InconsistentNaming
            private UxmlBoolAttributeDescription m_cullWhenCannotExpand = new UxmlBoolAttributeDescription {
                name = "cull-when-cannot-expand",
                defaultValue = false
            };

            /// <inheritdoc cref="VisualElement.UxmlTraits.Init"/>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var angularResizingElement = (IAngularResizingElement) ve;
                angularResizingElement.HasToBeCulledWhenCannotExpand = m_cullWhenCannotExpand.GetValueFromBag(bag, cc);

                angularResizingElement.AngularSizeHeight = m_angularHeight.GetValueFromBag(bag, cc);


                var styleOption = bag.ToStylesDictionary();
                var initialHeight = styleOption.Bind(ExtractHeight);
                initialHeight.IfSome(initial => angularResizingElement.InitialHeight = initial);
                var initialWidth = styleOption.Bind(ExtractWidth);
                initialWidth.IfSome(initial => angularResizingElement.InitialWidth = initial);

                m_angularWidth.TryGetValueFromBag(bag, cc).Match(w => angularResizingElement.AngularSizeWidth = w,
                    () => initialHeight.Bind(iHeight => initialWidth.Map(iWidth => iWidth.value / iHeight.value))
                        .Map(widthHeightRatio => widthHeightRatio * angularResizingElement.AngularSizeHeight)
                        .Match(angularSizeWidth => angularResizingElement.AngularSizeWidth = angularSizeWidth,
                            () => angularResizingElement.AngularSizeWidth = Option<float>.None));
            }
        }

        public void Resize(float distanceToCamera, float pixelPerMeter) =>
            AngularResizingElementUtils.Resize(this, distanceToCamera, pixelPerMeter);
    }
}