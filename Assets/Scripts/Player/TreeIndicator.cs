using UnityEngine;

namespace Regrowth
{
    public class TreeIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private Sprite[] sprites;
        
        public void SetIndicator(Vector2 pos, int idx, bool available = true)
        {
            transform.position = pos;
            renderer.sprite = sprites[idx];
            renderer.color = available ? new Color(1, 1, 1, 0.7f) : new Color(1, 0, 0, 0.7f);
        }
    }
}