using System.Buffers;
using TheGame.FSM;
using TheGame.HazzardSystems;
using UnityEngine;
using XIV.Core;

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
            var pos = stateMachine.transform.position;
#if UNITY_EDITOR
            XIVDebug.DrawCircle(pos, stateMachine.recieveDamageRadius, Color.magenta, 0.1f);
#endif
            count = Physics2D.OverlapCircleNonAlloc(pos, stateMachine.recieveDamageRadius * 0.5f, buffer, 1 << PhysicsConstants.HazzardLayer);

            for (int i = 0; i < count; i++)
            {
                var hazzarMono = buffer[i].transform.GetComponent<HazzardMono>();
                var damageAmount = hazzarMono.damageAmount;
                stateMachine.health -= damageAmount;
                stateMachine.playerUpdateHealthChannelSO.RaiseEvent(stateMachine.transform);
                hazzarMono.RaiseEvent(stateMachine.transform);
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