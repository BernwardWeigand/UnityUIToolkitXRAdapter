using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.DefaultElements.AngularResizingElementUtils;

namespace UIToolkitXRAdapter.AngularResizing.DefaultElements {
    public class AngularResizingVisualElementUxmlTraits : VisualElement.UxmlTraits {
        public const string CullWhenCanNotExpandName = "cull-when-cannot-expand";
        public const bool CullWhenCanNotExpandDefault = false;
        
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
                name = CullWhenCanNotExpandName,
                defaultValue = CullWhenCanNotExpandDefault
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
}