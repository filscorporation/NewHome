using Assets.Source.Units;
using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Crystals on the map can be transformed in golems
    /// </summary>
    public class Crystal : InteractableObject
    {
        protected override int Cost => 1;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 1.5F;

        [SerializeField] private GameObject golemPrefab;

        protected override void OnComplete()
        {
            GameObject go = Instantiate(golemPrefab, new Vector3(transform.position.x, golemPrefab.transform.position.y), Quaternion.identity);
            StartCoroutine(go.GetComponent<Golem>().FromCrystal());
            Destroy(gameObject);
        }
    }
}
