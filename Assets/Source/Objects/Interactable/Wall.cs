using System.Collections;
using System.Data;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Wall
    /// </summary>
    public class Wall : InteractableObject, IHitHandler, IBuildable
    {
        protected override int Cost => 3;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 2F;

        [SerializeField] private int maxHealth;
        private float health;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private Transform hitEffectTransform;
        [SerializeField] private float sizeX = 0F;
        [SerializeField] private GameObject buildingBase;

        private void Start()
        {
            MapManager.Instance.Targets.Add(transform, sizeX);
            MapManager.Instance.Walls.Add(transform, sizeX);
            health = maxHealth;
        }

        public BuildableType Type() => BuildableType.Building;

        public float GetX() => transform.position.x;

        public bool Build(float progress)
        {
            health += progress;

            if (health >= maxHealth)
            {
                BuildingsManager.Instance.Remove(this);
                return true;
            }

            return false;
        }

        private void OnDestroy()
        {
            MapManager.Instance.Targets.Remove(transform);
            MapManager.Instance.Walls.Remove(transform);
        }

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
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
            if (health < maxHealth)
                BuildingsManager.Instance.Add(this);
            if (health < 0)
            {
                BuildingsManager.Instance.Remove(this);
                Vector2 pos = new Vector2(transform.position.x, buildingBase.transform.position.y);
                Instantiate(buildingBase, pos, Quaternion.identity);
                Destroy(gameObject);
            }

            return true;
        }
    }
}
