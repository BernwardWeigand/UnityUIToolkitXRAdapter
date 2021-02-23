using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitXRAdapter.XRAdapter {
    internal static class XRUIToolkitEventUtils {
        private static void BlockEvent(this EventBase eventBase) {
            eventBase.PreventDefault();
            eventBase.StopImmediatePropagation();
        }

        private static IEnumerable<string> GetAllEventNames() => typeof(EventBase).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(EventBase)))
            .Where(type => !type.IsGenericType)
            .Where(type => !type.IsAbstract)
            .Where(type => type.IsPublic)
            .Select(type => type.Name);

        internal static void GenerateClassCode() {
            var names = GetAllEventNames().ToList();
            const string blockPre = "Block";
            const string modifiers = " static void ";
            var blockMethods = string.Join("\n", names.Select(eventName =>
                $"private{modifiers}{blockPre}{eventName}({eventName} uiEvent) => uiEvent.{nameof(BlockEvent)}();"));
            const string veArgName = "visualElement";
            const string intModifiers = "internal" + modifiers;
            var extensionSignature = $"AllEvents(this {nameof(VisualElement)} {veArgName}) " + "{\n";
            const string methodEnd = "\n}";
            var registerMethod =
                $"{intModifiers}{blockPre}{extensionSignature}" +
                string.Join("\n", names.Select(eventName =>
                    $"{veArgName}.{nameof(VisualElement.RegisterCallback)}<{eventName}>({blockPre}{eventName});")) +
                methodEnd;
            var unregisterMethod =
                $"{intModifiers}Allow{extensionSignature}" +
                string.Join("\n", names.Select(eventName =>
                    $"{veArgName}.{nameof(VisualElement.UnregisterCallback)}<{eventName}>({blockPre}{eventName});")) +
                methodEnd;
            Debug.Log(blockMethods + "\n" + registerMethod + "\n" + unregisterMethod);
        }

        private static void BlockPointerCaptureOutEvent(PointerCaptureOutEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerCaptureEvent(PointerCaptureEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseCaptureOutEvent(MouseCaptureOutEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseCaptureEvent(MouseCaptureEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockValidateCommandEvent(ValidateCommandEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockExecuteCommandEvent(ExecuteCommandEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockDragExitedEvent(DragExitedEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockDragEnterEvent(DragEnterEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockDragLeaveEvent(DragLeaveEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockDragUpdatedEvent(DragUpdatedEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockDragPerformEvent(DragPerformEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockFocusOutEvent(FocusOutEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockBlurEvent(BlurEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockFocusInEvent(FocusInEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockFocusEvent(FocusEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockInputEvent(InputEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockKeyDownEvent(KeyDownEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockKeyUpEvent(KeyUpEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockGeometryChangedEvent(GeometryChangedEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseDownEvent(MouseDownEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseUpEvent(MouseUpEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseMoveEvent(MouseMoveEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockContextClickEvent(ContextClickEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockWheelEvent(WheelEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseEnterEvent(MouseEnterEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseLeaveEvent(MouseLeaveEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseEnterWindowEvent(MouseEnterWindowEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseLeaveWindowEvent(MouseLeaveWindowEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseOverEvent(MouseOverEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockMouseOutEvent(MouseOutEvent uiEvent) => uiEvent.BlockEvent();

        private static void BlockContextualMenuPopulateEvent(ContextualMenuPopulateEvent uiEvent) =>
            uiEvent.BlockEvent();

        private static void BlockNavigationMoveEvent(NavigationMoveEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockNavigationCancelEvent(NavigationCancelEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockNavigationSubmitEvent(NavigationSubmitEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockAttachToPanelEvent(AttachToPanelEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockDetachFromPanelEvent(DetachFromPanelEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerDownEvent(PointerDownEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerMoveEvent(PointerMoveEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerStationaryEvent(PointerStationaryEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerUpEvent(PointerUpEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerCancelEvent(PointerCancelEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockClickEvent(ClickEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerEnterEvent(PointerEnterEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerLeaveEvent(PointerLeaveEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerOverEvent(PointerOverEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockPointerOutEvent(PointerOutEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockCustomStyleResolvedEvent(CustomStyleResolvedEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockTooltipEvent(TooltipEvent uiEvent) => uiEvent.BlockEvent();
        private static void BlockIMGUIEvent(IMGUIEvent uiEvent) => uiEvent.BlockEvent();

        internal static void BlockAllEvents(this VisualElement visualElement) {
            visualElement.RegisterCallback<PointerCaptureOutEvent>(BlockPointerCaptureOutEvent);
            visualElement.RegisterCallback<PointerCaptureEvent>(BlockPointerCaptureEvent);
            visualElement.RegisterCallback<MouseCaptureOutEvent>(BlockMouseCaptureOutEvent);
            visualElement.RegisterCallback<MouseCaptureEvent>(BlockMouseCaptureEvent);
            visualElement.RegisterCallback<ValidateCommandEvent>(BlockValidateCommandEvent);
            visualElement.RegisterCallback<ExecuteCommandEvent>(BlockExecuteCommandEvent);
            visualElement.RegisterCallback<DragExitedEvent>(BlockDragExitedEvent);
            visualElement.RegisterCallback<DragEnterEvent>(BlockDragEnterEvent);
            visualElement.RegisterCallback<DragLeaveEvent>(BlockDragLeaveEvent);
            visualElement.RegisterCallback<DragUpdatedEvent>(BlockDragUpdatedEvent);
            visualElement.RegisterCallback<DragPerformEvent>(BlockDragPerformEvent);
            visualElement.RegisterCallback<FocusOutEvent>(BlockFocusOutEvent);
            visualElement.RegisterCallback<BlurEvent>(BlockBlurEvent);
            visualElement.RegisterCallback<FocusInEvent>(BlockFocusInEvent);
            visualElement.RegisterCallback<FocusEvent>(BlockFocusEvent);
            visualElement.RegisterCallback<InputEvent>(BlockInputEvent);
            visualElement.RegisterCallback<KeyDownEvent>(BlockKeyDownEvent);
            visualElement.RegisterCallback<KeyUpEvent>(BlockKeyUpEvent);
            visualElement.RegisterCallback<GeometryChangedEvent>(BlockGeometryChangedEvent);
            visualElement.RegisterCallback<MouseDownEvent>(BlockMouseDownEvent);
            visualElement.RegisterCallback<MouseUpEvent>(BlockMouseUpEvent);
            visualElement.RegisterCallback<MouseMoveEvent>(BlockMouseMoveEvent);
            visualElement.RegisterCallback<ContextClickEvent>(BlockContextClickEvent);
            visualElement.RegisterCallback<WheelEvent>(BlockWheelEvent);
            visualElement.RegisterCallback<MouseEnterEvent>(BlockMouseEnterEvent);
            visualElement.RegisterCallback<MouseLeaveEvent>(BlockMouseLeaveEvent);
            visualElement.RegisterCallback<MouseEnterWindowEvent>(BlockMouseEnterWindowEvent);
            visualElement.RegisterCallback<MouseLeaveWindowEvent>(BlockMouseLeaveWindowEvent);
            visualElement.RegisterCallback<MouseOverEvent>(BlockMouseOverEvent);
            visualElement.RegisterCallback<MouseOutEvent>(BlockMouseOutEvent);
            visualElement.RegisterCallback<ContextualMenuPopulateEvent>(BlockContextualMenuPopulateEvent);
            visualElement.RegisterCallback<NavigationMoveEvent>(BlockNavigationMoveEvent);
            visualElement.RegisterCallback<NavigationCancelEvent>(BlockNavigationCancelEvent);
            visualElement.RegisterCallback<NavigationSubmitEvent>(BlockNavigationSubmitEvent);
            visualElement.RegisterCallback<AttachToPanelEvent>(BlockAttachToPanelEvent);
            visualElement.RegisterCallback<DetachFromPanelEvent>(BlockDetachFromPanelEvent);
            visualElement.RegisterCallback<PointerDownEvent>(BlockPointerDownEvent);
            visualElement.RegisterCallback<PointerMoveEvent>(BlockPointerMoveEvent);
            visualElement.RegisterCallback<PointerStationaryEvent>(BlockPointerStationaryEvent);
            visualElement.RegisterCallback<PointerUpEvent>(BlockPointerUpEvent);
            visualElement.RegisterCallback<PointerCancelEvent>(BlockPointerCancelEvent);
            visualElement.RegisterCallback<ClickEvent>(BlockClickEvent);
            visualElement.RegisterCallback<PointerEnterEvent>(BlockPointerEnterEvent);
            visualElement.RegisterCallback<PointerLeaveEvent>(BlockPointerLeaveEvent);
            visualElement.RegisterCallback<PointerOverEvent>(BlockPointerOverEvent);
            visualElement.RegisterCallback<PointerOutEvent>(BlockPointerOutEvent);
            visualElement.RegisterCallback<CustomStyleResolvedEvent>(BlockCustomStyleResolvedEvent);
            visualElement.RegisterCallback<TooltipEvent>(BlockTooltipEvent);
            visualElement.RegisterCallback<IMGUIEvent>(BlockIMGUIEvent);
        }

        internal static void AllowAllEvents(this VisualElement visualElement) {
            visualElement.UnregisterCallback<PointerCaptureOutEvent>(BlockPointerCaptureOutEvent);
            visualElement.UnregisterCallback<PointerCaptureEvent>(BlockPointerCaptureEvent);
            visualElement.UnregisterCallback<MouseCaptureOutEvent>(BlockMouseCaptureOutEvent);
            visualElement.UnregisterCallback<MouseCaptureEvent>(BlockMouseCaptureEvent);
            visualElement.UnregisterCallback<ValidateCommandEvent>(BlockValidateCommandEvent);
            visualElement.UnregisterCallback<ExecuteCommandEvent>(BlockExecuteCommandEvent);
            visualElement.UnregisterCallback<DragExitedEvent>(BlockDragExitedEvent);
            visualElement.UnregisterCallback<DragEnterEvent>(BlockDragEnterEvent);
            visualElement.UnregisterCallback<DragLeaveEvent>(BlockDragLeaveEvent);
            visualElement.UnregisterCallback<DragUpdatedEvent>(BlockDragUpdatedEvent);
            visualElement.UnregisterCallback<DragPerformEvent>(BlockDragPerformEvent);
            visualElement.UnregisterCallback<FocusOutEvent>(BlockFocusOutEvent);
            visualElement.UnregisterCallback<BlurEvent>(BlockBlurEvent);
            visualElement.UnregisterCallback<FocusInEvent>(BlockFocusInEvent);
            visualElement.UnregisterCallback<FocusEvent>(BlockFocusEvent);
            visualElement.UnregisterCallback<InputEvent>(BlockInputEvent);
            visualElement.UnregisterCallback<KeyDownEvent>(BlockKeyDownEvent);
            visualElement.UnregisterCallback<KeyUpEvent>(BlockKeyUpEvent);
            visualElement.UnregisterCallback<GeometryChangedEvent>(BlockGeometryChangedEvent);
            visualElement.UnregisterCallback<MouseDownEvent>(BlockMouseDownEvent);
            visualElement.UnregisterCallback<MouseUpEvent>(BlockMouseUpEvent);
            visualElement.UnregisterCallback<MouseMoveEvent>(BlockMouseMoveEvent);
            visualElement.UnregisterCallback<ContextClickEvent>(BlockContextClickEvent);
            visualElement.UnregisterCallback<WheelEvent>(BlockWheelEvent);
            visualElement.UnregisterCallback<MouseEnterEvent>(BlockMouseEnterEvent);
            visualElement.UnregisterCallback<MouseLeaveEvent>(BlockMouseLeaveEvent);
            visualElement.UnregisterCallback<MouseEnterWindowEvent>(BlockMouseEnterWindowEvent);
            visualElement.UnregisterCallback<MouseLeaveWindowEvent>(BlockMouseLeaveWindowEvent);
            visualElement.UnregisterCallback<MouseOverEvent>(BlockMouseOverEvent);
            visualElement.UnregisterCallback<MouseOutEvent>(BlockMouseOutEvent);
            visualElement.UnregisterCallback<ContextualMenuPopulateEvent>(BlockContextualMenuPopulateEvent);
            visualElement.UnregisterCallback<NavigationMoveEvent>(BlockNavigationMoveEvent);
            visualElement.UnregisterCallback<NavigationCancelEvent>(BlockNavigationCancelEvent);
            visualElement.UnregisterCallback<NavigationSubmitEvent>(BlockNavigationSubmitEvent);
            visualElement.UnregisterCallback<AttachToPanelEvent>(BlockAttachToPanelEvent);
            visualElement.UnregisterCallback<DetachFromPanelEvent>(BlockDetachFromPanelEvent);
            visualElement.UnregisterCallback<PointerDownEvent>(BlockPointerDownEvent);
            visualElement.UnregisterCallback<PointerMoveEvent>(BlockPointerMoveEvent);
            visualElement.UnregisterCallback<PointerStationaryEvent>(BlockPointerStationaryEvent);
            visualElement.UnregisterCallback<PointerUpEvent>(BlockPointerUpEvent);
            visualElement.UnregisterCallback<PointerCancelEvent>(BlockPointerCancelEvent);
            visualElement.UnregisterCallback<ClickEvent>(BlockClickEvent);
            visualElement.UnregisterCallback<PointerEnterEvent>(BlockPointerEnterEvent);
            visualElement.UnregisterCallback<PointerLeaveEvent>(BlockPointerLeaveEvent);
            visualElement.UnregisterCallback<PointerOverEvent>(BlockPointerOverEvent);
            visualElement.UnregisterCallback<PointerOutEvent>(BlockPointerOutEvent);
            visualElement.UnregisterCallback<CustomStyleResolvedEvent>(BlockCustomStyleResolvedEvent);
            visualElement.UnregisterCallback<TooltipEvent>(BlockTooltipEvent);
            visualElement.UnregisterCallback<IMGUIEvent>(BlockIMGUIEvent);
        }
    }
}