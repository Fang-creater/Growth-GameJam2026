using UnityEngine;

namespace Regrowth
{
    public class TreeIndicator : MonoBehaviour
    {
        public void SetIndicator(Vector2 pos, bool available = true)
        {
            transform.position = pos;
        }
    }
}