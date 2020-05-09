using System.Collections;
using System.Collections.Generic;
using Assets.Source.Units;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Crystals on the map can be transformed in golems
    /// </summary>
    public class Crystal : InteractableObject
    {
        protected override int Cost => 1;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 0.8F;

        [SerializeField] private GameObject golemPrefab;

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
            yield return new WaitForSeconds(0.5F);
            GameObject go = Instantiate(golemPrefab, new Vector3(transform.position.x, golemPrefab.transform.position.y), Quaternion.identity);
            StartCoroutine(go.GetComponent<Golem>().FromCrystal());
            gameObject.SetActive(false);
            Destroy(gameObject, 2F);
        }
    }
}
