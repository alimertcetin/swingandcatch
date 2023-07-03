using System.Buffers;
using TheGame.FSM;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.Extensions;

namespace TheGame.PlayerSystems.States
{
    public class PlayerFallingState : State<PlayerFSM, PlayerStateFactory>
    {
        const float MAX_DISTANCE_TO_ROPE_SEGMENT = 0.5f;
        float yVelocity;
        float fallingTime;
        const float MAX_FALL_DURATION = 5f;
        Vector3[] feetInitialLocalPositions;
        bool hasAirMovementInput;

        public PlayerFallingState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            RegisterInputActions();
            
            yVelocity = stateMachine.velocity.y;
            fallingTime = 0f;
            
            int feetLength = stateMachine.playerFeet.Length;
            feetInitialLocalPositions = ArrayPool<Vector3>.Shared.Rent(feetLength);
            for (int i = 0; i < feetLength; i++)
            {
                feetInitialLocalPositions[i] = stateMachine.playerFeet[i].localPosition;
            }
        }

        protected override void OnStateUpdate()
        {
            fallingTime += Time.deltaTime;
            SetTransformPosition();
            
            var normalizedFallingTime = fallingTime / MAX_FALL_DURATION;
            SetTransformScale(normalizedFallingTime);
            SetFeetPositions(normalizedFallingTime);
        }

        protected override void OnStateExit()
        {
            UnregisterInputActions();
            
            stateMachine.playerVisualTransform.localScale = stateMachine.playerVisualTransform.localScale.SetX(1f);
            int length = stateMachine.playerFeet.Length;
            for (int i = 0; i < length; i++)
            {
                stateMachine.playerFeet[i].localPosition = feetInitialLocalPositions[i];
            }
            ArrayPool<Vector3>.Shared.Return(feetInitialLocalPositions);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.CheckIsTouching(1 << PhysicsConstants.GroundLayer))
            {
                ChangeRootState(factory.GetState<PlayerGroundedState>());
                return;
            }

            if (stateMachine.CheckIsTouching(1 << PhysicsConstants.LavaLayer))
            {
                ChangeRootState(factory.GetState<PlayerDiedByLavaState>());
                return;
            }
            
            // TODO : We can't catch the rope when fps is lower than 15
            if (stateMachine.GetNearestRope(out var rope))
            {
                var pos = stateMachine.transform.position;
                ref var closestRopePoint = ref rope.GetClosestPoint(pos);
                if (closestRopePoint.index != 0 && Vector3.Distance(closestRopePoint.position, pos) < MAX_DISTANCE_TO_ROPE_SEGMENT)
                {
                    ChangeRootState(factory.GetState<PlayerClimbState>());
                    return;
                }
            }
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<OnAirMovementState>());
        }

        void SetTransformPosition()
        {
            var dt = Time.fixedDeltaTime * Time.timeScale;
            yVelocity += Physics.gravity.y * (stateMachine.stateDatas.fallStateDataSO.fallGravityScale * dt + Mathf.Clamp(fallingTime, 0f, 10f) * dt);
            yVelocity = Mathf.Clamp(yVelocity, -18f, 0f);

            var t = stateMachine.transform;
            var pos = t.position;
            pos.y += yVelocity * dt;
            
            if (hasAirMovementInput) stateMachine.SyncPosition();
            
            stateMachine.Move(pos);
        }

        void SetTransformScale(float normalizedFallingTime)
        {
            var normalizedXScale = Mathf.Clamp(1f - normalizedFallingTime, 0.5f, 1f);
            stateMachine.playerVisualTransform.localScale = stateMachine.playerVisualTransform.localScale.SetX(normalizedXScale);
        }

        void SetFeetPositions(float normalizedFallingTime)
        {
            var fallRepeated = Mathf.PingPong(normalizedFallingTime, MAX_FALL_DURATION);
            int length = stateMachine.playerFeet.Length;
            for (int i = 0; i < length; i++)
            {
                var footPos = feetInitialLocalPositions[i];
                footPos = Vector3.Lerp(footPos, footPos + Vector3.up * 0.2f, fallRepeated);
                stateMachine.playerFeet[i].localPosition = footPos;
            }
        }

        void RegisterInputActions()
        {
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.performed += OnMovementPerformed;
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.canceled += OnMovementCanceled;
        }
        
        void UnregisterInputActions()
        {
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.performed -= OnMovementPerformed;
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.canceled -= OnMovementCanceled;
        }

        void OnMovementPerformed(InputAction.CallbackContext context) => hasAirMovementInput = true;
        
        void OnMovementCanceled(InputAction.CallbackContext context) => hasAirMovementInput = false;
    }
}