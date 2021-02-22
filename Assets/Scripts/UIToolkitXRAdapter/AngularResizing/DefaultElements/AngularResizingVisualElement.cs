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
                var angularSizeElement = (IAngularResizingElement) ve;
                angularSizeElement.HasToBeCulledWhenCannotExpand = m_cullWhenCannotExpand.GetValueFromBag(bag, cc);

                angularSizeElement.AngularSizeHeight = m_angularHeight.GetValueFromBag(bag, cc);


                var nullableStyle = bag.ToStylesDictionary();
                if (nullableStyle != null) {
                    angularSizeElement.InitialHeight = ExtractHeight(nullableStyle);
                    angularSizeElement.InitialWidth = ExtractWidth(nullableStyle);
                }

                var nullableAngularSizeWidth = m_angularWidth.TryGetValueFromBag(bag, cc);
                if (nullableAngularSizeWidth.HasValue) {
                    angularSizeElement.AngularSizeWidth = nullableAngularSizeWidth;
                }
                else if (angularSizeElement.InitialHeight.HasValue && angularSizeElement.InitialWidth.HasValue) {
                    var ratio = angularSizeElement.InitialWidth.Value.value / 
                                angularSizeElement.InitialHeight.Value.value;
                    angularSizeElement.AngularSizeWidth = ratio * angularSizeElement.AngularSizeHeight;
                }
            }
        }

        public void Resize(float distanceToCamera, float pixelPerMeter) =>
            AngularResizingElementUtils.Resize(this, distanceToCamera, pixelPerMeter);
    }
}