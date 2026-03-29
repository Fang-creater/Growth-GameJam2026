using UnityEngine;

namespace Regrowth
{
    public class Home : MonoBehaviour
    {
        [SerializeField] private float _maxFloorDistance = 10f;
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var hit = Physics2D.Raycast(transform.position, Vector2.up, _maxFloorDistance, LayerMask.GetMask("Tree"));
                if (hit.collider != null) Succeed();
            }
        }
        private void Succeed()
        {
            Player.Instance.Sleep().Forget();
            Debug.Log("Level Complete");
        }
    }
}