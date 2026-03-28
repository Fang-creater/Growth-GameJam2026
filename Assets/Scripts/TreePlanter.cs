using UnityEngine;

namespace Regrowth
{
    public class TreePlanter : MonoBehaviour
    {
        public bool PlantMode = false;

        [SerializeField] private Tree _treePref;
        [SerializeField] private TreeIndicator _indicator;
        [SerializeField] private float raycastDistance = 10f;
        [SerializeField] private float thickestGround = 20f;

        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                PlantMode = true;
            if (!PlantMode)
            {
                _indicator.gameObject.SetActive(false);
                return;
            }
            _indicator.gameObject.SetActive(true);
            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos + Vector2.up * raycastDistance, Vector2.down, raycastDistance * 2,
                LayerMask.GetMask("Ground"));
            RaycastHit2D hitUp = Physics2D.Raycast(mousePos + Vector2.up * raycastDistance, Vector2.up, thickestGround,
                LayerMask.GetMask("Ground"));
            if (hit.collider == null || hitUp.collider != null)
            {
                _indicator.SetIndicator(mousePos, false);
                return;
            }
            _indicator.SetIndicator(hit.point);
            if (Input.GetMouseButton(0))
            {
                PlantMode = false;
                var tree = Instantiate(_treePref, transform);
                tree.transform.position = hit.point;
            }
        }
    }
}