using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Object that creates building on completion
    /// </summary>
    public class BuildingBase : InteractableObject, IBuildable
    {
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private List<Sprite> constructionSprites;
        private int constructionAnimationStage = 0;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private int cost = 3;
        protected override int Cost => cost;
        [SerializeField] private float constructionTime = 5F;
        private float constructionProgress = 0;
        private bool isFinished = false;

        protected override float InteractRadius => 0.3F;
        protected override float Height => 1F;

        protected override void Initialize()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
            BuildingsManager.Instance.Add(this);
            AnimateConstruction();
            yield break;
        }

        public BuildableType Type() => BuildableType.BuildingBase;

        public float GetX() => transform.position.x;

        /// <summary>
        /// Adds progress to building construction
        /// </summary>
        /// <param name="progress"></param>
        /// <returns>True if building was finished</returns>
        public bool Build(float progress)
        {
            if (isFinished)
                return true;

            constructionProgress += progress;
            AnimateConstruction();
            if (constructionProgress >= constructionTime)
            {
                FinishBuilding();
                return true;
            }

            return false;
        }

        private void AnimateConstruction()
        {
            if (constructionAnimationStage == constructionSprites.Count)
                return;

            float step = constructionTime / constructionSprites.Count;
            if (constructionProgress - constructionAnimationStage * step > -Mathf.Epsilon)
            {
                spriteRenderer.sprite = constructionSprites[constructionAnimationStage];
                constructionAnimationStage++;
            }
        }

        private void FinishBuilding()
        {
            isFinished = true;
            BuildingsManager.Instance.Remove(this);
            Vector2 pos = new Vector2(transform.position.x, buildingPrefab.transform.position.y);
            Instantiate(buildingPrefab, pos, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
