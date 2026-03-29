using DG.Tweening;
using SnExtension;
using UnityEngine;

namespace Regrowth
{
    public class TreePlanter : BehaviourSingleton<TreePlanter>
    {
        public bool PlantMode = false;

        [SerializeField] private Tree _treePref, _bigTreePref;
        [SerializeField] private TreeIndicator _indicator;
        [SerializeField] private float raycastDistance = 10f;
        [SerializeField] private float thickestGround = 20f;
        [SerializeField] public int SeedCount;

        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && SeedCount > 0)
                PlantMode = !PlantMode;
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
            RaycastHit2D hitPool = Physics2D.Raycast(mousePos + Vector2.up * raycastDistance, Vector2.down, raycastDistance * 2,
                LayerMask.GetMask("Water"));
            var onPool = hitPool.collider != null;
            if (hit.collider == null || hitUp.collider != null)
            {
                _indicator.SetIndicator(mousePos, 0, false);
                return;
            }
            _indicator.SetIndicator(hit.point, onPool ? 1 : 0);
            if (Input.GetMouseButton(0))
            {
                PlantMode = false;
                Tree tree = null;
                if (!onPool)
                {
                    tree = Instantiate(_treePref, transform);
                }
                else
                {
                    tree = Instantiate(_bigTreePref, transform);
                    var coll = hitPool.collider;
                    coll.enabled = false;
                    coll.gameObject.GetComponent<SpriteRenderer>()
                        .DOFade(0, 1)
                        .SetEase(Ease.Linear);
                }
                tree.transform.position = hit.point;
                SeedCount--;
            }
        }
    }
}