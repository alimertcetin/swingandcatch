using System.Buffers;
using UnityEngine;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects;

namespace TheGame.CollectableSystems
{
    public class CollectableItem : MonoBehaviour
    {
        [SerializeField] ItemSO itemSO;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float radius = 0.8f;

        void Update()
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, radius, buffer, layerMask);
            ArrayPool<Collider2D>.Shared.Return(buffer);
            // if (hitCount > 0) this.enabled = false;

            for (int i = 0; i < hitCount; i++)
            {
                var coll = buffer[i];
                if (coll.TryGetComponent(out IInventoryContainer inventoryContainer))
                {
                    var amount = 1;
                    inventoryContainer.GetInventory().TryAdd(itemSO.GetItem(), ref amount);
                    Debug.Log("Collected : " + itemSO);
                }
            }
        }
    }
}
