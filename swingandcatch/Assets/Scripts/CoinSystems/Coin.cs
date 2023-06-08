using System.Buffers;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using XIV.Core;

namespace TheGame.CoinSystems
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] CoinChannelSO coinCollectedChannelSO;
        
        const float DISTANCE_THRESHOLD = 1.5f;
        Camera cam;
        bool isAwake;
        
        void Awake()
        {
            cam = Camera.main;
        }

        void Update()
        {
            CheckIsAwake();
            if (isAwake == false) return;

            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var tr = transform;
            int count = Physics2D.OverlapCircleNonAlloc(tr.position, tr.localScale.y, buffer, 1 << PhysicsConstants.PlayerLayer);

            if (count != 0)
            {
                coinCollectedChannelSO.RaiseEvent(this);
            }
            
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }

        void CheckIsAwake()
        {
            var viewportPoint = cam.WorldToViewportPoint(transform.position);
            isAwake = viewportPoint.x > -DISTANCE_THRESHOLD &&
                      viewportPoint.x < DISTANCE_THRESHOLD + 1f &&
                      viewportPoint.y > -DISTANCE_THRESHOLD &&
                      viewportPoint.y < DISTANCE_THRESHOLD + 1f;
        }

        [Button]
        void Collect()
        {
            coinCollectedChannelSO.RaiseEvent(this);
        }
    }
}
