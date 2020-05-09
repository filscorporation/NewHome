using System.Collections;
using Assets.Source.UIManagement;
using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Energy on the ground
    /// </summary>
    public class EnergyObject : PickUpObject
    {
        protected override float InteractRadius => 0.2F;
        private bool animate = false;
        private Vector2 targetPosition;
        private SpriteRenderer sr;

        private void Update()
        {
            if (!animate)
                return;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3F);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(sr.color.a, 0, Time.deltaTime * 3F));
        }

        protected override IEnumerator OnPickUp()
        {
            GameUIManager.Instance.ActivateEnergyBar();
            animate = true;
            targetPosition = transform.position + new Vector3(0, 0.8F);
            sr = GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(1F);
            StartCoroutine(Player.Instance.GainEnergy(1));
            yield return new WaitForSeconds(0.5F);
            GameUIManager.Instance.DeactivateEnergyBar();
            Destroy(gameObject);
        }
    }
}
