using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Regrowth
{
    public static class SnSceneManager
    {
        #region 同步加载 (会卡顿主线程，仅适用于极小场景)

        /// <summary>
        /// 同步加载场景 (通过 Build Index)
        /// </summary>
        public static void LoadScene(int idx)
        {
            SceneManager.LoadScene(idx);
        }

        /// <summary>
        /// 同步加载场景 (通过 场景名称)
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion

        #region 异步加载 (推荐，支持 UniTask 和进度条)

        /// <summary>
        /// 异步加载场景 (通过 Build Index)，支持 UniTask 等待和进度反馈
        /// </summary>
        /// <param name="idx">场景索引</param>
        /// <param name="progress">用于接收加载进度的回调 (0.0 到 1.0)</param>
        public static async UniTask LoadSceneAsync(int idx, IProgress<float> progress = null)
        {
            // 1. 开始异步加载
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(idx);

            // 如果场景不存在，返回的 asyncOperation 会是 null，这里做个保护
            if (asyncOperation == null)
            {
                Debug.LogError($"[SceneManager] 找不到索引为 {idx} 的场景，请检查 Build Settings！");
                return;
            }

            // 2. 可选：如果你想等进度条播完或者玩家按任意键才进入场景，可以设为 false
            // asyncOperation.allowSceneActivation = true; 

            // 3. 使用 UniTask 等待加载完成，并自动将 AsyncOperation 的进度汇报给 progress
            await asyncOperation.ToUniTask(progress: progress);
        
            Debug.Log($"[SceneManager] 场景 {idx} 加载完成！");
        }

        /// <summary>
        /// 异步加载场景 (通过 场景名称)，支持 UniTask
        /// </summary>
        public static async UniTask LoadSceneAsync(string sceneName, IProgress<float> progress = null)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            if (asyncOperation == null)
            {
                Debug.LogError($"[SceneManager] 找不到名称为 {sceneName} 的场景，请检查 Build Settings！");
                return;
            }

            await asyncOperation.ToUniTask(progress: progress);
        
            Debug.Log($"[SceneManager] 场景 {sceneName} 加载完成！");
        }

        #endregion
        
        #region 重置当前场景 (Reload)

        /// <summary>
        /// 同步重置当前场景（瞬间重新加载，适用于小关卡或死亡快速重开）
        /// </summary>
        public static void ReloadCurrentScene()
        {
            // 获取当前正在运行的场景的索引
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
        
            // 复用我们之前写的同步加载逻辑
            LoadScene(currentIndex);
        
            Debug.Log($"[SceneManager] 已同步重置当前场景 (Index: {currentIndex})");
        }

        /// <summary>
        /// 异步重置当前场景（支持 UniTask 和进度条，适用于包含大量资源的大场景）
        /// </summary>
        /// <param name="progress">用于接收加载进度的回调</param>
        public static async UniTask ReloadCurrentSceneAsync(IProgress<float> progress = null)
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
        
            // 复用我们之前写的异步加载逻辑
            await LoadSceneAsync(currentIndex, progress);
        
            Debug.Log($"[SceneManager] 已异步重置当前场景 (Index: {currentIndex})");
        }

        #endregion
    }
}
