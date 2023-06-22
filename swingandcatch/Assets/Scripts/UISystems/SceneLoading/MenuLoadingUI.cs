using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.Extensions;

namespace TheGame.UISystems.SceneLoading
{
    public class MenuLoadingUI : LoadingUIBase
    {
        [SerializeField] TMP_Text txt_SceneLoadStatus;
        [SerializeField] Image progressbarImage;

        public override void Show()
        {
            txt_SceneLoadStatus.text ="";
            uiGameObject.transform.localScale = Vector3.one;
            uiGameObject.SetActive(true);
            isActive = true;
        }

        public override void UpdateProgressBar(float value)
        {
            var progressValue = value * 10;
            progressValue = Mathf.RoundToInt(progressValue) / 10f;
            progressbarImage.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_Range, progressValue);
            txt_SceneLoadStatus.text = $"% {(value * 100f).ToString("F0").ToColor(Color.Lerp(Color.red, Color.green, value))}";
        }
    }
}