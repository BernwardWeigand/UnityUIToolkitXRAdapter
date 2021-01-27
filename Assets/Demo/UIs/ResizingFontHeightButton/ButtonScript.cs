﻿using LanguageExt;
using UIToolkitXRAdapter.AngularResizing.FontHeightOnlyTextElements;
using UIToolkitXRAdapter.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.UIs.ResizingFontHeightButton {
    public class ButtonScript : MonoBehaviour {
        private void Awake() {
            this.AsOption<UIDocument>().Map(doc => doc.rootVisualElement).IfSome(root => {
                var label = root.Q<AngularFontHeightLabel>();
                Option<AngularFontHeightButton> buttonOption = root.Q<AngularFontHeightButton>();
                buttonOption.IfSome(button => button.clicked += () => label.visible = !label.visible);
            });
        }
    }
}