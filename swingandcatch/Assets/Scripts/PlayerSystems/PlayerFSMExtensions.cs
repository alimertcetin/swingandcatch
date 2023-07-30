using System.Buffers;
using TheGame.VerletRope;
using UnityEngine;
using XIV.Core;
using XIV.Core.Extensions;
using XIV.Core.XIVMath;

namespace TheGame.PlayerSystems
{
    public static class PlayerFSMExtensions
    {
        
        public static bool GetNearestRope(this PlayerFSM playerFSM, out Rope rope)
        {
            var position = playerFSM.transform.position;
            var checkRadius = playerFSM.stateDatas.climbStateDataSO.climbCheckRadius;
            
            rope = default;
            var colliderBuffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int count = Physics2D.OverlapCircleNonAlloc(position, checkRadius, colliderBuffer, 1 << PhysicsConstants.RopeLayer);
            if (count == 0)
            {
                ArrayPool<Collider2D>.Shared.Return(colliderBuffer);
                return false;
            }
            rope = colliderBuffer.GetClosest(position, count).GetComponent<Rope>();
            ArrayPool<Collider2D>.Shared.Return(colliderBuffer);
            return count > 0;
        }
    }
}