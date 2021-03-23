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

        [HarmonyPatch(typeof(InputSystemEventSystem), "ScreenToPanel")]
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedType.Local
        class Patch {
            // ReSharper disable once ArrangeTypeMemberModifiers
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once RedundantAssignment
            // ReSharper disable once InconsistentNaming
            /// <summary>
            /// TODO
            /// see https://harmony.pardeike.net/articles/patching-postfix.html#reading-or-changing-the-result
            /// </summary>
            /// <param name="__result"></param>
            /// <param name="panel"></param>
            /// <param name="screenPosition"></param>
            /// <param name="screenDelta"></param>
            /// <param name="panelPosition"></param>
            /// <param name="panelDelta"></param>
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