using System;
using System.Buffers;
using TheGame.HealthSystems;
using UnityEngine;
using XIV.Core;

namespace TheGame.HazzardSystems
{
    [RequireComponent(typeof(Collider2D))]
    public class HazzardMono : MonoBehaviour
    {
        [SerializeField] float damageAmount;
        Collider2D collider2D;

        void Awake()
        {
            collider2D = GetComponent<Collider2D>();
        }

        void Update()
        {
            var bounds = collider2D.bounds;
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = Physics2D.OverlapBoxNonAlloc(transform.position, bounds.size, 0f, buffer, 1 << PhysicsConstants.PlayerLayer);
            XIVDebug.DrawBounds(bounds);

            for (int i = 0; i < hitCount; i++)
            {
                var coll = buffer[i];
                if (coll.TryGetComponent(out IDamageable damageable))
                {
                    if (damageable.CanReceiveDamage()) damageable.ReceiveDamage(damageAmount);
                }
            }
            
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }
    }
}