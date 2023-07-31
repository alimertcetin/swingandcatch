using System;
using System.Buffers;
using TheGame.AnimationSystems.Extensions;
using TheGame.PlayerSystems;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using UnityEngine;
using XIV.Core;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.EndGateSystems
{
    public class LevelEndGate : MonoBehaviour
    {
        [SerializeField] Vector3 offset;
        [SerializeField] float checkRadius;
        [SerializeField] Animator animator;
        [SerializeField] SceneListSO sceneListSO;
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] LevelDataChannelSO levelDataLoadedChannel;

        LevelData levelData;

        void OnEnable()
        {
            levelDataLoadedChannel.Register(OnLevelDataLoaded);
        }

        void OnDisable()
        {
            levelDataLoadedChannel.Unregister(OnLevelDataLoaded);
        }

        void OnLevelDataLoaded(LevelData levelData)
        {
            this.levelData = levelData;
        }

        void Update()
        {
            if (CheckPlayer(out var player) == false) return;
            
            this.enabled = false;
            animator.Play(AnimationConstants.EndGate.Clips.EndGate_Open);
            
            player.GetComponent<PlayerFSM>().isReachedEndGate = true;
            var duration = animator.GetClipDuration(AnimationConstants.EndGate.Clips.EndGate_Open);
            
            XIVEventSystem.SendEvent(new InvokeAfterEvent(duration + 0.5f).OnCompleted(() =>
            {
                if (levelData != null && levelData.TryGetNextLevel(levelData.lastPlayedLevel, out var nextLevel))
                {
                    sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
                }
                else
                {
                    sceneLoadChannel.RaiseEvent(SceneLoadOptions.MenuLoad(sceneListSO.mainMenuSceneIndex));
                }
            }));
        }

        bool CheckPlayer(out Collider2D player)
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var pos = transform.position + offset;
            int count = Physics2D.OverlapCircleNonAlloc(pos, checkRadius, buffer, 1 << PhysicsConstants.PlayerLayer);
            ArrayPool<Collider2D>.Shared.Return(buffer);
            player = count > 0 ? buffer[0] : default;
            return count != 0;
        }

#if UNITY_EDITOR

        void OnValidate()
        {
            if (animator) return;
            animator = GetComponentInChildren<Animator>();
        }

        void OnDrawGizmos()
        {
            var pos = transform.position + offset;
            XIVDebug.DrawCircle(pos, checkRadius, Vector3.forward, Color.green);
        }
#endif
    }
}
