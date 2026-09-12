using UnityEngine;
namespace AppleGrapple
{
    public class CameraFollowController : MonoBehaviour
    {
       [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float smoothSpeed = 0.125f;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

}
