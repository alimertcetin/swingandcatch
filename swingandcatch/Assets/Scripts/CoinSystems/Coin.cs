using System;
using System.Buffers;
using TheGame.AnimationSystems;
using TheGame.InventorySystems.Items;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems;
using TheGame.UISystems.Core;
using UnityEngine;
using XIV.Core;
using XIV.Core.Extensions;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects.ChannelSOs;
using Random = UnityEngine.Random;

namespace TheGame.CoinSystems
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] GameObject coinCollectedParticlePrefab;
        [SerializeField] CoinItemSO coinItemSO;
        [SerializeField] InventoryChannelSO inventoryLoadedChannel;
        
        const float DISTANCE_THRESHOLD = 1.5f;
        Camera cam;
        bool isAwake;
        Inventory inventory;
        
        void Awake()
        {
            cam = Camera.main;
        }

        void OnEnable()
        {
            inventoryLoadedChannel.Register(OnInventoryLaoded);
        }

        void OnDisable()
        {
            inventoryLoadedChannel.Unregister(OnInventoryLaoded);
        }

        void OnInventoryLaoded(Inventory inventory)
        {
            this.inventory = inventory;
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
                Collect();
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
            if (Application.isPlaying == false)
            {
                Debug.LogError("You cant use this in editor mode");
                return;
            }
            DisableAnimations();
            InstantiateParticle();
            
            this.enabled = false;
            var coinTransform = this.transform;
            var coinTransformPosition = coinTransform.position;
            var coinScreenPos = cam.WorldToScreenPoint(coinTransformPosition);
            var hud = UISystem.GetUI<HudUI>();
            var coinUIItemRectPosition = hud != null ? hud.coinUIItemRectPosition.SetZ(coinScreenPos.z) : (Vector3.one * 2f).SetZ(coinScreenPos.z);
            
            var curve = BezierMath.CreateCurve(coinScreenPos, coinUIItemRectPosition, Random.value * 3f);
            coinTransform.XIVTween()
                .WorldToUIMove(curve, Camera.main, Random.Range(0.5f, 1f), EasingFunction.EaseInExpo)
                .OnComplete(OnCoinTweenCompleted)
                .Start();
        }

        void OnCoinTweenCompleted(GameObject go)
        {
            int amount = 1;
            inventory?.TryAdd(coinItemSO.GetItem(), ref amount);
            if (go) Destroy(go);
        }

        void DisableAnimations()
        {
            var animations = GetComponentsInChildren<AnimationMonoBase>();
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].enabled = false;
            }
        }

        void InstantiateParticle()
        {
            var particleGo = Instantiate(coinCollectedParticlePrefab);
            particleGo.transform.position = transform.position;
            Destroy(particleGo, 5f);
        }
    }
}
