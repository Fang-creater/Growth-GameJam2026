using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Regrowth
{
    public class LoadingController : MonoBehaviour
    {
        [Header("UI (Image swap)")]
        [SerializeField] private Image progressImage;
        [SerializeField] private Sprite[] stepSprites;     // 4张：25/50/75/100
        [SerializeField] private Slider progressSlider;

        private int lastStep = -1;

        private void Start()
        {
            SetStep(0);
            if (progressSlider != null) progressSlider.value = 0f;

            Load().Forget();
        }

        private async UniTaskVoid Load()
        {
            var target = SceneLoadRequest.TargetSceneName;

            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[Loading] TargetSceneName 为空，不能从 LevelSelect 进入 Loading");
                return;
            }

            IProgress<float> progress = new Progress<float>(p =>
            {
                // Unity 场景加载 progress 通常到 0.9，归一化到 0~1
                float normalized = Mathf.Clamp01(p / 0.9f);

                if (progressSlider != null)
                    progressSlider.value = normalized;

                int step = GetStepIndex(normalized);
                if (step != lastStep)
                    SetStep(step);
            });

            // 这里用 static 调用（你 SnSceneManager 目前就是 static）
            await SnSceneManager.LoadSceneAsync(target, progress);

            // 确保最终显示 100%
            if (progressSlider != null) progressSlider.value = 1f;
            SetStep(3);
        }

        // 0..3 -> 25/50/75/100
        private int GetStepIndex(float normalized)
        {
            if (normalized >= 0.75f) return 3; // 100%
            if (normalized >= 0.50f) return 2; // 75%
            if (normalized >= 0.25f) return 1; // 50%
            return 0;                           // 25%
        }

        private void SetStep(int step)
        {
            lastStep = step;

            if (progressImage == null) return;
            if (stepSprites == null || stepSprites.Length < 4) return;

            step = Mathf.Clamp(step, 0, 3);

            progressImage.sprite = stepSprites[step];
            progressImage.preserveAspect = true;

            // SetNativeSize 可选：有时会把 UI 撑坏（看你的布局）
            // progressImage.SetNativeSize();
        }
    }
}