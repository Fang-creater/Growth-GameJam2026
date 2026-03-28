using UnityEngine;

namespace Regrowth
{
    public class Home : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && Physics2D.Raycast(transform.position, Vector2.down, LayerMask.GetMask("Tree")))
                Succeed();
        }
        private void Succeed()
        {
            Player.Instance.Sleep();
            Debug.Log("Level Complete");
        }
    }
}