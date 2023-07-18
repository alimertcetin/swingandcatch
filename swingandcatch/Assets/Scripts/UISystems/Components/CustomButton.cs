using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    public class CustomButton : Button
    {
        public new ButtonClickedEvent onClick = new ButtonClickedEvent();
        UnityAction onPointerUpAction;
        UnityAction onSelectAction;

        // --- Custom Events
        public CustomButton RegisterOnClick(UnityAction action)
        {
            onClick.RemoveAllListeners();
            onClick.AddListener(action);
            return this;
        }

        public CustomButton UnregisterOnClick()
        {
            onClick.RemoveAllListeners();
            return this;
        }

        public CustomButton RegisterOnPointerUp(UnityAction action)
        {
            onPointerUpAction = action.Invoke;
            return this;
        }

        public CustomButton UnregisterOnPointerUp()
        {
            onPointerUpAction = null;
            return this;
        }

        public CustomButton RegisterOnSelect(UnityAction action)
        {
            onSelectAction = action.Invoke;
            return this;
        }

        public CustomButton UnregisterOnSelect()
        {
            onSelectAction = null;
            return this;
        }
        
        // --- Overrides
        // TODO : Learn what changes when overriding below methods
        public override void OnPointerDown(PointerEventData eventData)
        {
            onClick.Invoke();
            // if we call base.OnPointerDown(eventData) state is not changing correctly, dont know why
            DoStateTransition(SelectionState.Pressed, false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            onPointerUpAction?.Invoke();
            // if we call base.OnPointerUp(eventData) state is not changing correctly, dont know why
            DoStateTransition(SelectionState.Normal, false);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            onSelectAction?.Invoke();
            DoStateTransition(SelectionState.Selected, false);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            onClick.Invoke();
            DoStateTransition(SelectionState.Pressed, false);
        }
    }
}
