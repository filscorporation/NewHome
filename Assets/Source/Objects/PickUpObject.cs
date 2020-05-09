using System.Collections;
using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Objects, that can be picked up
    /// </summary>
    public abstract class PickUpObject : MonoBehaviour
    {
        protected abstract float InteractRadius { get; }
        private bool pickedUp = false;

        private void FixedUpdate()
        {
            if (pickedUp)
                return;

            float dist = Mathf.Abs(Player.Instance.transform.position.x - transform.position.x);
            if (dist < InteractRadius)
            {
                pickedUp = true;
                StartCoroutine(OnPickUp());
            }
        }

        /// <summary>
        /// Called when player picked up object
        /// </summary>
        protected abstract IEnumerator OnPickUp();
    }
}
