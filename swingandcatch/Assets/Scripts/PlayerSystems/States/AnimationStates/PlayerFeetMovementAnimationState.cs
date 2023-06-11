using System.Buffers;
using TheGame.FSM;
using UnityEngine;

namespace TheGame.PlayerSystems.States.AnimationStates
{
    public class PlayerFeetMovementAnimationState : State<PlayerFSM, PlayerStateFactory>
    {
        public float animationTime;
        Vector3[] feetInitialPositions;
        float timePassedSinceEnter;
        
        public PlayerFeetMovementAnimationState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            timePassedSinceEnter = 0f;
            int length = stateMachine.playerFeet.Length;
            feetInitialPositions = ArrayPool<Vector3>.Shared.Rent(length);
            for (int i = 0; i < length; i++)
            {
                feetInitialPositions[i] = stateMachine.playerFeet[i].localPosition;
            }
        }

        protected override void OnStateUpdate()
        {
            timePassedSinceEnter += Time.deltaTime;

            var normalizedTime = Mathf.PingPong(timePassedSinceEnter / animationTime, 1f);
            
            int length = stateMachine.playerFeet.Length;
            const float OFFSET = 0.1f;
            for (int i = 0; i < length; i++)
            {
                var pos = feetInitialPositions[i];
                var targetPos = pos + Vector3.up * 0.25f;
                var newPos = Vector3.Lerp(pos, targetPos, normalizedTime + (OFFSET * i));
                stateMachine.playerFeet[i].localPosition = newPos;
                normalizedTime = 1f - normalizedTime;
            }
        }

        protected override void OnStateExit()
        {
            int length = stateMachine.playerFeet.Length;
            for (int i = 0; i < length; i++)
            {
                stateMachine.playerFeet[i].localPosition = feetInitialPositions[i];
            }
            
            ArrayPool<Vector3>.Shared.Return(feetInitialPositions);
            feetInitialPositions = null;
        }
    }
}