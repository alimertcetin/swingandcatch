using System.Buffers;
using TheGame.FSM;
using UnityEngine;

namespace TheGame.PlayerSystems.States.DamageStates
{
    public class CheckDamageState : State<PlayerFSM, PlayerStateFactory>
    {
        int count;
        Collider2D[] buffer;
        
        public CheckDamageState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            buffer = ArrayPool<Collider2D>.Shared.Rent(8);
        }

        protected override void OnStateUpdate()
        {
            count = Physics2D.OverlapCircleNonAlloc(stateMachine.transform.position, stateMachine.recieveDamageRadius * 0.5f, buffer, 1 << PhysicsConstants.HazzardLayer);

            for (int i = 0; i < count; i++)
            {
                stateMachine.OnHazzardHit(buffer[i]);
            }
        }

        protected override void OnStateExit()
        {
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.health <= 0f)
            {
                // var lastCollider = buffer[count - 1]; // last collider that damages the player
                ChangeRootState(factory.GetState<PlayerDiedByLavaState>());
                return;
            }

            if (count > 0)
            {
                ChangeChildState(factory.GetState<DamageImmuneState>());
                return;
            }
            
        }
    }
}