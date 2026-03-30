using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Regrowth
{
    public class ResetButton : MonoBehaviour
    {
        // 给 UI Button 的 OnClick() 绑定这个函数
        public void ResetToSpawn()
        {
            ResetAsync().Forget();
        }

        private async UniTaskVoid ResetAsync()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}