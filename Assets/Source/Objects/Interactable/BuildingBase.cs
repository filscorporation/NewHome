using System.Collections;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Object that creates building on completion
    /// </summary>
    public class BuildingBase : InteractableObject
    {
        [SerializeField] private GameObject buildingPrefab;
        [field: SerializeField] protected override int Cost { get; }

        protected override float InteractRadius => 0.3F;
        protected override float Height => 0.5F;

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
            yield break;
        }
    }
}
