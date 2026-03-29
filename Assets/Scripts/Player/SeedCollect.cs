using System;
using UnityEngine;

namespace Regrowth
{
    public class SeedCollect : MonoBehaviour
    {
        [SerializeField] private int count;
        
        private Collider2D coll;
        private void Awake()
        {
            coll = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TreePlanter.Instance.SeedCount += count;
                coll.enabled = false;
            }
        }
    }
}