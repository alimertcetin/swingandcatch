using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.Extensions;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.UISystems
{
    public class SceneLoadingUI : GameUI
    {
        [SerializeField] FloatChannelSO sceneLoadingProgressChannel;
        [SerializeField] VoidChannelSO newSceneLoadedChannel;
        [SerializeField] VoidChannelSO activateNewlyLoadedScene;
        [SerializeField] TMP_Text txt_SceneLoadStatus;
        [SerializeField] Image progressbarImage;
        
        void OnEnable()
        {
            sceneLoadingProgressChannel.Register(UpdateProgressBar);
            newSceneLoadedChannel.Register(OnSceneLoaded);
        }

        void OnDisable()
        {
            sceneLoadingProgressChannel.Unregister(UpdateProgressBar);
            newSceneLoadedChannel.Unregister(OnSceneLoaded);
        }

        public override void Show()
        {
            txt_SceneLoadStatus.text ="";
            uiGameObject.transform.localScale = Vector3.one;
            uiGameObject.SetActive(true);
            isActive = true;
        }

        void UpdateProgressBar(float value)
        {
            var progressValue = value * 10;
            progressValue = Mathf.RoundToInt(progressValue) / 10f;
            progressbarImage.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_Range, progressValue);
            txt_SceneLoadStatus.text = $"% {(value * 100f).ToString("F0").ToColor(Color.Lerp(Color.red, Color.green, value))}";
        }

        void OnSceneLoaded()
        {
            txt_SceneLoadStatus.text = "Press Any Key To Start";
            bool terminate = false;
            XIVEventSystem.SendEvent(new InvokeUntilEvent().AddAction(() =>
                {
                    if (Input.anyKey)
                    {
                        terminate = true;
                        activateNewlyLoadedScene.RaiseEvent();
                    }
                })
                .AddCancelCondition(() => terminate));
        }
    }
}
