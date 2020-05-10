using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Spawns aliens
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        private bool spawned = false;

        [SerializeField] private float distance = 3F;
        [SerializeField] private List<GameObject> prefabs;
        [SerializeField] [Range(0, 100)] private int amountMin = 5;
        [SerializeField] [Range(0, 100)] private int amoutMax = 10;

        private void FixedUpdate()
        {
            if (spawned && DayNightCycleController.CycleState == CycleState.Day)
            {
                spawned = false;
            }

            if (!spawned && DayNightCycleController.CycleState == CycleState.Night)
            {
                StartCoroutine(Spawn());
            }
        }

        private IEnumerator Spawn()
        {
            spawned = true;

            int amount = Random.Range(amountMin, amoutMax);
            for (int i = 0; i < amount; i++)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
                Instantiate(prefab,
                    new Vector3(Random.Range(
                            transform.position.x - distance,
                            transform.position.x + distance),
                        prefab.transform.position.y), Quaternion.identity);
                yield return new WaitForSeconds(0.5F);
            }
        }
    }
}
