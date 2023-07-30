using System.Buffers;
using TheGame.AbilitySystems.Core;
using TheGame.ScriptableObjects.AbilitySystems;
using UnityEngine;
using XIV.Core;

namespace TheGame.AbilitySystems.Tests
{
    public class AbilityUnlockTest : MonoBehaviour
    {
        [SerializeField] AbilitySO abilityToUnlock;
        [SerializeField] bool useImmediate;
        [SerializeField] float radius;

        void Update()
        {
            var mask = 1 << PhysicsConstants.PlayerLayer;
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, radius, buffer, mask);
            for (int i = 0; i < hitCount; i++)
            {
                var col = buffer[i];
                if (col.TryGetComponent(out IAbilityHandler abilityHandler))
                {
                    var ability = abilityToUnlock.GetAbility();
                    if (abilityHandler.AddAbility(ability))
                    {
                        if (useImmediate) abilityHandler.UseAbility(ability);
                        Debug.Log("Unlocked " + abilityToUnlock.GetType());
                        this.enabled = false;
                    }
                }
            }
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            XIVDebug.DrawCircle(transform.position, radius);
            if (abilityToUnlock)
            {
                var name = abilityToUnlock.GetType().ToString().Split('.')[^1];
                XIVDebug.DrawText(transform.position + Vector3.up * 2f, name, 16, Color.white);
            }
        }
#endif
        
    }
}