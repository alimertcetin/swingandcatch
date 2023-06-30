using System;
using System.Buffers;
using TheGame.HealthSystems;
using UnityEngine;
using UnityEngine.Pool;

namespace TheGame.HazzardSystems
{
    public class Projectile : MonoBehaviour
    {
        public GameObject projectileParticlePrefab;
        float lifeTime;
        float speed = 4f;
        float damageAmount = 4f;
        Vector3 moveDirection;
        int layerMask;

        ObjectPool<Projectile> pool;
        
        Vector3 previousPosition;

        public void Initialize(ObjectPool<Projectile> pool, float lifeTime, float speed, float damageAmount, Vector3 moveDirection, int layerMask)
        {
            this.pool = pool;
            this.lifeTime = lifeTime;
            this.speed = speed;
            this.damageAmount = damageAmount;
            this.moveDirection = moveDirection;
            this.layerMask = layerMask;
        }

        void Update()
        {
            lifeTime -= Time.deltaTime;
            
            var pos = transform.position;
            previousPosition = pos;
            pos += moveDirection * (speed * Time.deltaTime);
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var diff = pos - previousPosition;
            int count = Physics2D.OverlapCircleNonAlloc(previousPosition + ((diff) * 0.5f), diff.magnitude, buffer, layerMask);

            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.CanReceiveDamage()) damageable.ReceiveDamage(damageAmount);
                }
            }

            if (count > 0 || lifeTime < 0)
            {
                pool.Release(this);
                ArrayPool<Collider2D>.Shared.Return(buffer);
                return;
            }
            
            transform.position = pos;
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }
    }
}