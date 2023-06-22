using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.Extensions;

namespace TheGame.UISystems.SceneLoading
{
    public class LevelLoadingUI : LoadingUIBase
    {
        [SerializeField] TMP_Text txt_SceneLoadStatus;
        [SerializeField] Image parallaxImage;
        [SerializeField] Image progressbarImage;
            
        readonly string[] loadingSuffixes = new string[]
        {
            ".".ToColorRed(),
            "..".ToColor(Color.Lerp(Color.red, Color.green, 0.5f)),
            "...".ToColor(Color.green),
        };

        public override void Show()
        {
            txt_SceneLoadStatus.text ="";
            uiGameObject.transform.localScale = Vector3.one;
            uiGameObject.SetActive(true);
            isActive = true;
        }

        public override void UpdateProgressBar(float value)
        {
            var progressValue = value * 10f;
            var loadingText = "";
            if (value < 0.9f)
            {
                var index = Mathf.RoundToInt((value * 3) % (loadingSuffixes.Length - 1));
                var loadingSuffix = loadingSuffixes[index];
                loadingText = $"Loading{loadingSuffix} ";
            }

            progressValue = Mathf.RoundToInt(progressValue) / 10f;
            progressbarImage.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_Range, progressValue);
            
            txt_SceneLoadStatus.text = $"{loadingText} % {(value * 100f).ToString("F0").ToColor(Color.Lerp(Color.red, Color.green, value))}";
            
            parallaxImage.material.SetVector(ShaderConstants.ShaderGraphs_ParallaxBackgroundShader.ParallaxOffset_VectorID, Vector3.right * (value * 2.5f));
        }
    }
}