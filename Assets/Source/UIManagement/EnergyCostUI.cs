using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// UI showing that player can spend energy here
    /// </summary>
    public class EnergyCostUI : MonoBehaviour
    {
        private Transform targetToFollow;
        private float height = 0;

        private void Update()
        {
            if (targetToFollow == null)
            {
                Destroy(this);
                Destroy(gameObject, 0.5F);
                return;
            }

            FollowTarget();
        }

        private void FollowTarget()
        {
            transform.position = Camera.main.WorldToScreenPoint(targetToFollow.position + new Vector3(0, height));
        }

        public void Initialize(Transform target, int size, float h)
        {
            targetToFollow = target;
            height = h;
        }
    }
}
