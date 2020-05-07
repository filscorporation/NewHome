using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source
{
    /// <summary>
    /// Randomly generates effects
    /// </summary>
    public class EffectGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private List<Transform> effectPositions;
        [SerializeField] private float minSpawnPeriod = 4F;
        [SerializeField] private float maxSpawnPeriod = 10F;

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            float time = Random.Range(minSpawnPeriod, maxSpawnPeriod);
            yield return new WaitForSeconds(time);

            Vector2 pos = effectPositions[Random.Range(0, effectPositions.Count)].position;
            Destroy(Instantiate(effectPrefab, pos, Quaternion.identity, transform), 5F);

            StartCoroutine(Spawn());
        }
    }
}
