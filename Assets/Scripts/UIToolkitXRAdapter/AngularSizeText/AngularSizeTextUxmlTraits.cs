using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.AngularSizeText {
    public class AngularSizeTextUxmlTraits : TextElement.UxmlTraits {
        private UxmlFloatAttributeDescription m_angularTextSize = new UxmlFloatAttributeDescription {
            name = "size-in-arc-minutes",
            defaultValue = 90f
        };

        private UxmlBoolAttributeDescription m_cullWhenCannotExpand = new UxmlBoolAttributeDescription {
            name = "cull-when-cannot-expand",
            defaultValue = false
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            var angularSizeLabel = (IAngularSizeText) ve;
            angularSizeLabel.AngularTextHeight = m_angularTextSize.GetValueFromBag(bag, cc);
            angularSizeLabel.HasToBeCulledWhenCannotExpand = m_cullWhenCannotExpand.GetValueFromBag(bag, cc);

            var initialFontHeight = AngularSizeTextUtils.ExtractFontSize(bag);
            if (initialFontHeight.HasValue) {
                angularSizeLabel.InitialFontHeight = initialFontHeight.Value;
            }
        }
    }
}