using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Regrowth
{
    public class LoadingController : MonoBehaviour
    {
        [SerializeField] private Slider progressSlider; // ����������ѡ��

        private void Start()
        {
            Load().Forget();
        }

        private async UniTaskVoid Load()
        {
            var target = SceneLoadRequest.TargetSceneName;

            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[Loading] TargetSceneName Ϊ�գ���� LevelSelect ���� Loading");
                return;
            }

            // �����Ȼص�ӳ�䵽 UI
            IProgress<float> progress = new Progress<float>(p =>
            {
                if (progressSlider != null) progressSlider.value = p;
            });

            await SnSceneManager.LoadSceneAsync(target, progress);
        }
    }
}