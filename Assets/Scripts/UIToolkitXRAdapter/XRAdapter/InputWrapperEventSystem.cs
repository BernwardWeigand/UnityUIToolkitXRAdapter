using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.UIElements.PointerType;

namespace UIToolkitXRAdapter.XRAdapter {

    // Note DG: All UI events need to be constructed inside the
    // "evtFactory" lambda when calling EventSystem methods.
    // I actually have no idea why this is necessary, but trial and error
    // determined that this is the only way this seems to work.
    
    /// Currently, as of com.unity.ui version 1.0.0preview6, the <see cref="EventSystem"/> does not properly use
    /// the methods of the <see cref="InputWrapper"/>, making it impossible to abstract the input given to UI Toolkit ui.
    ///
    /// This class is an alternative implementation of the <see cref="EventSystem"/>that "shadows"
    /// its behaviour and implements a custom solution that uses the <see cref="InputWrapper"/>.
    public class InputWrapperEventSystem : EventSystem {
        
        private readonly Event mEvent = new Event();
        private const BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private void callMethod(string methodName, params object[] args) {
            if (typeof(EventSystem).GetMethod(methodName, defaultBindingFlags) is MethodInfo method) {
                method.Invoke(this, args);
            } else {
                throw new ArgumentException($"A method with name {methodName} could not be found in the {nameof(EventSystem)} class");
            }
        }
        
        private Res callMethodWithReturnValue<Res>(string methodName, params object[] args) {
            var res = typeof(EventSystem).GetMethod(methodName, defaultBindingFlags)?.Invoke(this, args)
                ?? throw new ArgumentException($"A method with name {methodName} could not be found in the {nameof(EventSystem)} class");;
            return (Res) res;
        }
        
        private void callGenericMethod<GenericParameter>(string methodName, params object[] args) {
            if (typeof(EventSystem).GetMethod(methodName, defaultBindingFlags) is MethodInfo method) {
                method.MakeGenericMethod(typeof(GenericParameter)).Invoke(this, args);
            } else {
                throw new ArgumentException($"A method with name {methodName} could not be found in the {nameof(EventSystem)} class");
            }
        }

        private void setPropertyOf(object instance, string propertyName, object newValue) {
            if (instance.GetType().GetProperty(propertyName, defaultBindingFlags) is PropertyInfo property) {
                property.SetValue(instance, newValue);
            } else {
                throw new ArgumentException($"A property with name {propertyName} could not be found in the {instance.GetType().Name} class / struct");
            }
        }

        /// We intentionally provide an Update() function here and do not call the base class Update method.
        /// This "shadows" the <see cref="EventSystem"/> behaviour and allows us to selectively
        /// re-implement it, but alter specific parts to our needs.
        ///
        /// IF THE EVENT SYSTEM USES A DIFFERENT EVENT IN THE FUTURE, THAT HAS TO BE SHADOWED INSTEAD.
        private void Update() {
            // Re-implementation of the first 2 lines of the event system's update, which are fine
            // if (!isAppFocused && callMethodWithReturnValue<bool>("ShouldIgnoreEventsOnAppNotFocused"))
            //     return;
            //
            // // Instead of calling SendImGuiEvents like in the base class, we have our own implementation
            // // that properly uses the input events from the InputWrapper
            // {
            //     makeAndSendPointerEvent<PointerMoveEvent>(inputOverride.mousePosition);
            //
            //     var mouseButtons = checkMouseButtons(inputOverride.GetMouseButton);
            //     for (int i = 0; i < mouseButtons.Length; i++) {
            //         if (mouseButtons[i] && inputOverride.GetMouseButtonDown(i)) {
            //             makeAndSendPointerEvent<PointerDownEvent>(inputOverride.mousePosition);
            //             makeAndSendMouseEvent<MouseDownEvent>(i);
            //             
            //         } else if (inputOverride.GetMouseButtonUp(i)) {
            //             makeAndSendPointerEvent<PointerUpEvent>(inputOverride.mousePosition);
            //             makeAndSendMouseEvent<MouseUpEvent>(i);
            //             
            //             // Send ClickEvent only on leftClickUp
            //             if (i==0) makeAndSendPointerEvent<ClickEvent>(inputOverride.mousePosition);
            //         }
            //     }
            //
            //     var scrollDelta = inputOverride.mouseScrollDelta;
            //     if (scrollDelta.y > float.Epsilon || scrollDelta.y < -float.Epsilon) {
            //         callGenericMethod<Func<object, EventBase>>(
            //             "SendFocusBasedEvent",
            //             // Note DG: Events need to be constructed inside the lambda, for unity's pooling to work properly
            //             (Func<object, EventBase>) (_ => {
            //                 var mouseEvent = MouseEventBase<WheelEvent>.GetPooled();
            //                 setPropertyOf(mouseEvent, "mousePosition", inputOverride.mousePosition);
            //                 setPropertyOf(mouseEvent, "delta", (Vector3) scrollDelta);
            //                 return mouseEvent;
            //             }),
            //             null
            //         );
            //         
            //     }
            //     sendFocusBasedIMGUIEvents();
            // }
            // // Re-implementation of the last line of the event system's update, which is fine
            // callMethod("SendInputEvents");
        }

        // uses old IMGUI events to send keyboard inputs
        private void sendFocusBasedIMGUIEvents() {
            while (Event.PopEvent(mEvent)) {
                if (mEvent.type == EventType.Repaint)
                    continue;

                if (mEvent.type == EventType.KeyUp) {
                    callGenericMethod<Func<object, EventBase>>(
                        "SendFocusBasedEvent",
                        // Note DG: Events need to be constructed inside the lambda, for unity's pooling to work properly
                        (Func<object, EventBase>) (_ => KeyUpEvent.GetPooled(mEvent)),
                        null
                    );
                } else if (mEvent.type == EventType.KeyDown) {
                    callGenericMethod<Func<object, EventBase>>(
                        "SendFocusBasedEvent",
                        // Note DG: Events need to be constructed inside the lambda, for unity's pooling to work properly
                        (Func<object, EventBase>) (_ => KeyDownEvent.GetPooled(mEvent)),
                        null
                    );
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

        private void makeAndSendMouseEvent<TMouseEvent>(int button)
        where TMouseEvent : MouseEventBase<TMouseEvent>, new() {
            
            callGenericMethod<Func<object, EventBase>>(
                "SendFocusBasedEvent",
                // Note DG: Events need to be constructed inside the lambda, for unity's pooling to work properly
                (Func<object, EventBase>) (_ => {
                    var mouseEvent = MouseEventBase<TMouseEvent>.GetPooled();
                    setPropertyOf(mouseEvent, "mousePosition", inputOverride.mousePosition);
                    setPropertyOf(mouseEvent, "button", button);
                    return mouseEvent;
                }),
                null
            );
        }

        private void makeAndSendPointerEvent<TPointerEvent>(Vector2 position)
        where TPointerEvent : PointerEventBase<TPointerEvent>, new() {
            
            callGenericMethod<Func<object, EventBase>>(
                "SendPositionBasedEvent",
                // Note DG: Events need to be constructed inside the lambda, for unity's pooling to work properly
                (Func<object, EventBase>) (_ => {
                    var pointerEvent = PointerEventBase<TPointerEvent>.GetPooled();
                    setPropertyOf(pointerEvent, "position", (Vector3) position);
                    setPropertyOf(pointerEvent, "isPrimary", true);
                    setPropertyOf(pointerEvent, "pointerType", PointerType.mouse);
                    return pointerEvent;
                }),
                null
            );
        }
    }
}