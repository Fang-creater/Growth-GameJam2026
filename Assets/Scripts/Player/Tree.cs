using UnityEngine;

namespace Regrowth
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] private Transform _up, _root;

        public Vector3 GetTargetPos(Vector3 pos)
        {
            return Vector3.Distance(pos, _up.position) > Vector3.Distance(pos, _root.position) ? _up.position : _root.position;
        }
    }
}
