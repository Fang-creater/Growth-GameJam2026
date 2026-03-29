using UnityEngine;

namespace Regrowth
{
    public class ParallaxBackground : MonoBehaviour
    {
        public Vector2 parallaxMultiplier = new Vector2(0.5f, 0f);

        private Transform cameraTransform;
        private Vector2 startPosition;
        private float spriteWidth;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            startPosition = transform.position;
        }

        private void LateUpdate()
        {
            Vector2 distance = new Vector2(
                cameraTransform.position.x * parallaxMultiplier.x,
                cameraTransform.position.y * parallaxMultiplier.y
            );

            transform.position = new Vector3(
                startPosition.x + distance.x,
                startPosition.y + distance.y,
                transform.position.z
            );
        }
    }

}