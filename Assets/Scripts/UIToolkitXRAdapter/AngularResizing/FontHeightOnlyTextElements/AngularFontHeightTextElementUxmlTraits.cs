using UIToolkitXRAdapter.AngularResizing.DefaultElements;
using UnityEngine.UIElements;
using static UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements.AngularFontHeightTextElementsUtils;

namespace UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements {
    public class AngularFontHeightTextElementUxmlTraits : TextElement.UxmlTraits {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private UxmlFloatAttributeDescription m_angularFontHeight = new UxmlFloatAttributeDescription {
            name = AngularFontHeightTextFieldUxmlTraits.AngularFontHeightName,
            defaultValue = AngularFontHeightTextFieldUxmlTraits.DefaultAngularFontHeight
        };

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private UxmlBoolAttributeDescription m_cullWhenTextCannotExpand = new UxmlBoolAttributeDescription {
            name = AngularResizingVisualElementUxmlTraits.CullWhenCanNotExpandName,
            defaultValue = AngularResizingVisualElementUxmlTraits.CullWhenCanNotExpandDefault
        };

        /// <inheritdoc cref="TextElement.UxmlTraits.Init"/>
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            var angularHeightText = (IAngularFontHeightTextElement) ve;
            angularHeightText.AngularFontHeight = m_angularFontHeight.GetValueFromBag(bag, cc);
            angularHeightText.HasToBeCulledWhenCannotExpand = m_cullWhenTextCannotExpand.GetValueFromBag(bag, cc);
            var initialFontHeight = ExtractFontSize(bag);
            if (initialFontHeight.HasValue) {
                angularHeightText.InitialFontHeight = initialFontHeight.Value;
            }
        }
    }
}