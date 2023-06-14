using System;
using UnityEngine;

namespace TheGame.FSM
{
    public abstract class StateMachine : MonoBehaviour
    {
        State currentState;

        protected virtual void Awake() => InitializeStateMachine(GetInitialState());
        protected virtual void Start() => currentState.EnterState(currentState);
        protected virtual void FixedUpdate() => currentState.FixedUpdateState();
        protected virtual void Update() => currentState.UpdateState();
        protected virtual void LateUpdate() => currentState.LateUpdateState();
        protected void OnDestroy() => currentState.ExitState();

        void InitializeStateMachine(State initialState)
        {
            // TODO : Maybe we could need some initialization in here
            SetCurrentState(initialState);
        }
        
        public void SetCurrentState(State newState)
        {
            var from = currentState;
            currentState = newState;
            OnStateChanged(from);
        }

        protected virtual void OnStateChanged(State from) { }
        protected abstract State GetInitialState();

        /// <summary>
        /// Checks if current state of StateMachine same as <typeparamref name="TState"/>
        /// </summary>
        /// <typeparam name="TState">State</typeparam>
        /// <returns>True if current state is TState</returns>
        public bool IsInState<TState>() where TState : State
        {
            return currentState is TState;
        }

#if UNITY_EDITOR
        [SerializeField] bool showCurrentState;
        [SerializeField, Range(-0.5f, 0.5f)] float textOffsetX;
        [SerializeField, Range(-0.5f, 0.5f)] float textOffsetY;
        void OnGUI()
        {
            if (showCurrentState == false || currentState == null) return;

            const float WIDTH = 1000f;
            const float HEIGHT = 20f;

            var transformPos = transform.position;
            var cam = Camera.main;
            transformPos.z = Mathf.Abs(transformPos.z - cam.transform.position.z);
            var labelOffset = Vector2.up * HEIGHT;
            var screenPoint = cam.WorldToScreenPoint(transformPos);

            var rect = new Rect(screenPoint.x, Screen.height - screenPoint.y, WIDTH, HEIGHT);
            rect.position += (Vector2)cam.ViewportToScreenPoint(new Vector3(textOffsetX, -textOffsetY, 0f));
            GUI.color = Color.red;
            GUI.Label(rect, gameObject.name, GUI.skin.label);
            rect.position += labelOffset;
            GUI.color = Color.white;
            
            GUI.Label(rect, currentState.GetType().Name, GUI.skin.label);
            rect.position += labelOffset;
            
            foreach (State childState in currentState.childs)
            {
                GUI.Label(rect, childState.GetType().Name, GUI.skin.label);
                rect.position += labelOffset;
            }
        }
#endif
    }
}