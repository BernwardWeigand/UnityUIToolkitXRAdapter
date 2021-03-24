using System.Reflection;
using UnityEditor;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.InputSystem;
using static UnityEngine.InputSystem.CommonUsages;

namespace UIToolkitXRAdapter.XRAdapter {
    [InitializeOnLoad]
    internal class InputSystenEventSystemPatcher {
        static InputSystenEventSystemPatcher() {
            var harmony = new Harmony("ui.toolkit.xr.adapter");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Patches the <see cref="InputSystemEventSystem.ScreenToPanel"/> to work properly in XR
        /// </summary>
        [HarmonyPatch(typeof(InputSystemEventSystem), "ScreenToPanel")]
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedType.Local
        class Patch {
            // ReSharper disable once ArrangeTypeMemberModifiers
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once RedundantAssignment
            // ReSharper disable once InconsistentNaming
            /// <remarks>
            /// Have a look at 
            /// <a href="https://harmony.pardeike.net/articles/patching-postfix.html#reading-or-changing-the-result">
            /// the documentation</a> to understand how the patching works
            /// </remarks>>
            /// <summary>
            /// Checks if the dominant <see cref="UIToolkitXRController"/> is currently pointing at the given
            /// <see cref="UnityEngine.UIElements.BaseRuntimePanel"/> and corrects the result of the original method.
            /// The out params of the original methods were already corrected by the
            /// <see cref="XRAdapterUtils.UIToolkitPosition"/> processing before.
            /// </summary>
            /// <param name="__result">the result of the previous method call, which is corrected</param>
            /// <param name="panel">
            /// the <see cref="UnityEngine.UIElements.BaseRuntimePanel"/> that has to be checked
            /// </param>
            /// <param name="screenPosition"><see cref="InputSystemEventSystem.ScreenToPanel"/></param>
            /// <param name="screenDelta"><see cref="InputSystemEventSystem.ScreenToPanel"/></param>
            /// <param name="panelPosition">see summary</param>
            /// <param name="panelDelta">see summary</param>
            static void Postfix(ref bool __result, object panel, Vector2 screenPosition, Vector2 screenDelta,
                ref Vector2 panelPosition, ref Vector2 panelDelta) {
                var rightController = InputSystem.GetDevice<UIToolkitXRController>(RightHand);
                var dominantController = rightController.IsDominantHand
                    ? rightController
                    : InputSystem.GetDevice<UIToolkitXRController>(LeftHand);
                __result = dominantController.CurrentlyFocusedPanel == panel;
            }
        }
    }
}