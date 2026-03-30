using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Regrowth
{
    public class LoadingController : MonoBehaviour
    {
        [Header("UI (Image swap)")]
        [SerializeField] private Image progressImage;      // 用来显示“带文字的进度图片”
        [SerializeField] private Sprite[] stepSprites;     // 4张图：25/50/75/100（按顺序放）

        private int lastStep = -1;

        private void Start()
        {
            // 可选：初始化显示第一张（25%之前也可以先显示25%那张或空白图）
            SetStep(0);

            Load().Forget();
        }

        private async UniTaskVoid Load()
        {
            var target = SceneLoadRequest.TargetSceneName;

            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[Loading] TargetSceneName 为空，请检查从 LevelSelect 进入 Loading");
                return;
            }

            IProgress<float> progress = new Progress<float>(p =>
            {
                // Unity 常见：p 最大约到 0.9，这里归一化到 0~1
                float normalized = Mathf.Clamp01(p / 0.9f);

                // 计算档位：0/1/2/3 对应 25/50/75/100
                int step = GetStepIndex(normalized);

                if (step != lastStep)
                {
                    SetStep(step);
                }
            });

            await SnSceneManager.Instance.LoadSceneAsync(target, progress);

            // 最终确保显示 100%
            SetStep(3);
        }

        // 返回 0..3，分别代表 25/50/75/100
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

            progressImage.sprite = stepSprites[step];
            progressImage.SetNativeSize(); // 可选：如果你希望图片尺寸自动匹配
            progressImage.preserveAspect = true;
        }
    }
}