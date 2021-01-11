using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using CoreLibrary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static System.Reflection.BindingFlags;
using static UnityEngine.XR.InputDeviceCharacteristics;
using PointerType = UnityEngine.UIElements.PointerType;

namespace UIToolkitXRAdapter.XRAdapter {
    /// Currently, as of com.unity.ui version 1.0.0preview6, the <see cref="EventSystem"/> does not properly use
    /// the methods of the <see cref="InputWrapper"/>, making it impossible to abstract the input given to UI Toolkit ui.
    ///
    /// This class is an alternative implementation of the <see cref="EventSystem"/>that "shadows"
    /// its behaviour and implements a custom solution that uses the <see cref="InputWrapper"/>.
    ///
    /// It allows interaction with any <see cref="UIDocument"/> that also have a <see cref="InputWrapper"/>
    /// component attached and are pointed at by the <see cref="UserInput"/> system.
    /// However, only one <see cref="XRInputWrapperEventSystem"/> should be active in any loaded scene,
    /// as it can handle the interaction and events for any amount of ui documents. 
    public class XRInputWrapperEventSystem : EventSystem {
        public XRController PrimaryController => primaryHand == XRNode.RightHand ? controllerRight : controllerLeft;

        public XRNode primaryHand = XRNode.RightHand;

        [SerializeField] private XRController controllerLeft;

        [SerializeField] private XRController controllerRight;

        private readonly Event mEvent = new Event();

        private const BindingFlags defaultBindingFlags = Instance | Public | NonPublic;

        private Res callMethodWithReturnValue<Res>(string methodName, params object[] args) {
            var res = typeof(EventSystem).GetMethod(methodName, defaultBindingFlags)?.Invoke(this, args)
                      ?? throw new ArgumentException(
                          $"A method with name {methodName} could not be found in the {nameof(EventSystem)} class"
                      );
            return (Res) res;
        }

        private void setPropertyOf(object instance, string propertyName, object newValue) {
            if (instance.GetType().GetProperty(propertyName, defaultBindingFlags) is PropertyInfo property) {
                property.SetValue(instance, newValue);
            }
            else {
                throw new ArgumentException(
                    $"A property with name {propertyName} could not be found in the {instance.GetType().Name} class / struct"
                );
            }
        }

        private Res getFieldValue<Res>(string fieldName) {
            var res = typeof(EventSystem).GetField(fieldName, defaultBindingFlags)?.GetValue(this) ??
                      throw new ArgumentException(
                          $"A field with name {fieldName} could not be found in the {nameof(EventSystem)} class"
                      );
            return (Res) res;
        }

        #region Single Event System Validation

        // Note DG: Static state needed to ensure that only one
        // of this component is in any loaded scene at any time
        private static bool _eventSystemAlreadyPresent = false;

        private void OnEnable() {
            Contract.Assert(
                !_eventSystemAlreadyPresent,
                $"There are multiple active {nameof(XRInputWrapperEventSystem)} components" +
                $" in all loaded scenes. This should not happen and may send UI events multiple times."
            );
            _eventSystemAlreadyPresent = true;
        }

        private void OnDisable() {
            Contract.Assert(_eventSystemAlreadyPresent,
                $"There is no active {nameof(XRInputWrapperEventSystem)} component" +
                $" in any loaded scene. This should not happen and indicates a programming error in this class."
            );
            _eventSystemAlreadyPresent = false;
        }

        #endregion

        /// A bitfield that represents all buttons that are currently clicked.
        /// Bit 0 is set when mouse button 0 is pressed, and so on..
        /// This replicates the functionality of <see cref="PointerEventBase{T}.pressedButtons" />
        private int _currentlyPressedMouseButtonsBitfield = 0;

        /// We intentionally provide an Update() function here and do not call the base class Update method.
        /// This "shadows" the <see cref="EventSystem"/> behaviour and allows us to selectively
        /// re-implement it, but alter specific parts to our needs.
        ///
        /// IF THE EVENT SYSTEM USES A DIFFERENT EVENT IN THE FUTURE, THAT HAS TO BE SHADOWED INSTEAD.
        // private void Update() {
        //     
        //     // Note DG: This code determines the UIDocument that is currently pointed at. This needs to be
        //     // calculated every frame, as the user may point to a different UI Document between frames.
        //     
        //     var currentUI = UserInput.Actions.PointerTarget.IfNotNull(t => t
        //         .As<UIDocument>(Search.InChildren).IfNotNull(doc => doc.isActiveAndEnabled ? doc : null)
        //     );
        //     var currentInputWrapper = currentUI.IfNotNull(ui => ui.As<InputWrapper>());
        //     var currentUiRootElement = currentUI.IfNotNull(ui => ui.rootVisualElement);
        //     
        //     // Re-implementation of the first 2 lines of the event system's update, which are fine
        //     if (!isAppFocused && callMethodWithReturnValue<bool>("ShouldIgnoreEventsOnAppNotFocused"))
        //         return;
        //     
        //     if(currentInputWrapper != null) {
        //         // Instead of calling SendImGuiEvents like in the base class, we have our own implementation
        //         // that properly uses the input events from the InputWrapper
        //         
        //         makeAndSendPointerEvent<PointerMoveEvent>(currentUiRootElement, currentInputWrapper.mousePosition);
        //     
        //         var mouseButtons = checkMouseButtons(currentInputWrapper.GetMouseButton);
        //         for (int i = 0; i < mouseButtons.Length; i++) {
        //             if (currentInputWrapper.GetMouseButtonDown(i)) {
        //                 // Set the bit at the index of i
        //                 _currentlyPressedMouseButtonsBitfield |= 1 << i;
        //     
        //                 makeAndSendPointerEvent<PointerDownEvent>(currentUiRootElement, currentInputWrapper.mousePosition);
        //                 makeAndSendMouseEvent<MouseDownEvent>(currentInputWrapper, currentUiRootElement, i);
        //             } else if (currentInputWrapper.GetMouseButtonUp(i)) {
        //                 // Un-set the bit at the index of i
        //                 _currentlyPressedMouseButtonsBitfield &= ~(1 << i);
        //     
        //                 makeAndSendPointerEvent<PointerUpEvent>(currentUiRootElement, currentInputWrapper.mousePosition);
        //                 makeAndSendMouseEvent<MouseUpEvent>(currentInputWrapper, currentUiRootElement, i);
        //                 // Send ClickEvent only on leftClickUp
        //                 if (i==0) makeAndSendPointerEvent<ClickEvent>(currentUiRootElement, currentInputWrapper.mousePosition);
        //             }
        //         }
        //     
        //         var scrollDelta = currentInputWrapper.mouseScrollDelta;
        //         if (scrollDelta.y > float.Epsilon || scrollDelta.y < -float.Epsilon) {
        //             var mouseEvent = MouseEventBase<WheelEvent>.GetPooled();
        //             setPropertyOf(mouseEvent, "mousePosition", currentInputWrapper.mousePosition);
        //             setPropertyOf(mouseEvent, "delta", (Vector3) scrollDelta);
        //             
        //             currentUiRootElement.SendEvent(mouseEvent);
        //         }
        //         sendFocusBasedIMGUIEvents(currentUiRootElement);
        //         
        //         sendInputEvents(currentInputWrapper, currentUiRootElement);
        //     }
        // }

        // uses old IMGUI events to send keyboard inputs
        private void sendFocusBasedIMGUIEvents(VisualElement currentRootElement) {
            while (Event.PopEvent(mEvent)) {
                if (mEvent.type == EventType.Repaint)
                    continue;

                if (mEvent.type == EventType.KeyUp) {
                    currentRootElement.SendEvent(KeyUpEvent.GetPooled(mEvent));
                }
                else if (mEvent.type == EventType.KeyDown) {
                    currentRootElement.SendEvent(KeyDownEvent.GetPooled(mEvent));
                }
            }
        }

        private void sendInputEvents(InputWrapper currentInputWrapper, VisualElement currentRootElement) {
            var sendNavigationMove = callMethodWithReturnValue<bool>("ShouldSendMoveFromInput");

            if (sendNavigationMove) {
                var evt = NavigationMoveEvent.GetPooled(callMethodWithReturnValue<Vector2>("GetRawMoveVector"));
                currentRootElement.SendEvent(evt);
            }

            if (currentInputWrapper.GetButtonDown(getFieldValue<string>("m_SubmitButton"))) {
                currentRootElement.SendEvent(NavigationSubmitEvent.GetPooled());
            }

            if (currentInputWrapper.GetButtonDown(getFieldValue<string>("m_CancelButton"))) {
                currentRootElement.SendEvent(NavigationCancelEvent.GetPooled());
            }

            for (var i = 0; i < currentInputWrapper.touchCount; ++i) {
                var touch = currentInputWrapper.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                using (var evt = callMethodWithReturnValue<EventBase>("MakeTouchEvent", touch, EventModifiers.None)) {
                    currentRootElement.SendEvent(evt);
                }
            }
        }

        /// Calls mouseFunction for every mouse button and returns bool array with results.
        /// Index 0 is left mouse button, 1 is right mouse button, 2 is middle mouse button
        private bool[] checkMouseButtons(Func<int, bool> mouseFunction) {
            bool[] buttons = {false, false, false};
            for (int i = 0; i < buttons.Length; i++) {
                buttons[i] = mouseFunction(i);
            }

            return buttons;
        }

        private void makeAndSendMouseEvent<TMouseEvent>(
            InputWrapper currentInputWrapper, VisualElement currentRootElement, int button
        ) where TMouseEvent : MouseEventBase<TMouseEvent>, new() {
            var mouseEvent = MouseEventBase<TMouseEvent>.GetPooled();
            setPropertyOf(mouseEvent, "mousePosition", currentInputWrapper.mousePosition);
            setPropertyOf(mouseEvent, "button", button);
            setPropertyOf(mouseEvent, "pressedButtons", _currentlyPressedMouseButtonsBitfield);

            currentRootElement.SendEvent(mouseEvent);
        }

        private void makeAndSendPointerEvent<TPointerEvent>(VisualElement currentRootElement, Vector2 position)
            where TPointerEvent : PointerEventBase<TPointerEvent>, new() {
            var pointerEvent = PointerEventBase<TPointerEvent>.GetPooled();
            setPropertyOf(pointerEvent, "position", (Vector3) position);
            setPropertyOf(pointerEvent, "isPrimary", true);
            setPropertyOf(pointerEvent, "pointerType", PointerType.mouse);
            setPropertyOf(pointerEvent, "pressedButtons", _currentlyPressedMouseButtonsBitfield);

            currentRootElement.SendEvent(pointerEvent);
        }
    }
}