using System.Collections;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Wall
    /// </summary>
    public class Wall : InteractableObject, IHitHandler
    {
        protected override int Cost => 3;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 2F;

        [SerializeField] private int maxHealth;
        private int health;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private Transform hitEffectTransform;

        private void Start()
        {
            MapManager.Instance.Targets.Add(transform);
            health = maxHealth;
        }

        private void OnDestroy()
        {
            MapManager.Instance.Targets.Remove(transform);
        }

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
            Debug.Log("Wall complete");
            yield break;
        }
        
        /// <summary>
        /// Takes a hit
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="buildingDamage"></param>
        /// <returns>True if need to bounce</returns>
        public bool TakeHit(int damage, int buildingDamage)
        {
            GameObject effect = Instantiate(hitEffect, hitEffectTransform.position, Quaternion.identity);
            effect.transform.localScale = transform.localScale;
            Destroy(effect, 2F);

            health -= buildingDamage;
            if (health < 0)
                Destroy(gameObject);

            return true;
        }
    }
}
