using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.UpgradeManagement;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Building where golems pick up upgreats
    /// </summary>
    public class RecruitBuilding : InteractableObject, IUpgradeHolder
    {
        [SerializeField] private int cost;
        protected override int Cost => cost;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 1F;

        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private List<Transform> itemSlots;
        private List<GameObject> items = new List<GameObject>();
        [SerializeField] private GameObject itemPrefab;

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
            yield return new WaitForSeconds(0.5F);
            IsActive = true;

            GameObject go = Instantiate(itemPrefab, itemSlots[items.Count].position, Quaternion.identity, transform);
            items.Add(go);
            UpgradePool.Instance.Add(new Upgrade(upgradeType, this));

            if (items.Count >= itemSlots.Count)
            {
                IsActive = false;
            }
        }

        /// <summary>
        /// Try to get item for the golem
        /// </summary>
        /// <returns></returns>
        public bool TryGetItem()
        {
            if (!items.Any())
                return false;

            Destroy(items[items.Count - 1]);
            items.RemoveAt(items.Count - 1);
            IsActive = true;

            return true;
        }

        /// <summary>
        /// Returns holder transform
        /// </summary>
        /// <returns></returns>
        public Transform GetTransform() => transform;
    }
}
