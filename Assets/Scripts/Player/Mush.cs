using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Regrowth
{
    public class Mush : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer mushRenderer;
        [SerializeField] private SpriteRenderer frog;
        [SerializeField] private Sprite active, deactive;
        
        private Collider2D coll;
        private void Awake()
        {
            coll = GetComponent<Collider2D>();
        }

        private bool _active = true;

        private void FixedUpdate()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.up, 10f, LayerMask.GetMask("Tree", "Ground"));
            var act = hit.collider != null;
            if (act == _active) return;
            coll.enabled = act;
            _active = act;
            frog.DOFade(act ? 1 : 0, 0.5f);
        }
        private void Update()
        {
            mushRenderer.sprite = _active ? active : deactive;
        }
    }
}