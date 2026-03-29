using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Regrowth
{
    public class LoadingController : MonoBehaviour
    {
        [SerializeField] private Slider progressSlider; // 进度条（可选）

        private void Start()
        {
            Load().Forget();
        }

        private async UniTaskVoid Load()
        {
            var target = SceneLoadRequest.TargetSceneName;

            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[Loading] TargetSceneName 为空：请从 LevelSelect 进入 Loading");
                return;
            }

            // 将进度回调映射到 UI
            IProgress<float> progress = new Progress<float>(p =>
            {
                if (progressSlider != null) progressSlider.value = p;
            });

            await SnSceneManager.Instance.LoadSceneAsync(target, progress);
        }
    }
}