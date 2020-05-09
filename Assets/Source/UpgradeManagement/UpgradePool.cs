using System.Collections.Generic;
using System.Linq;
using Assets.Source.Units;
using UnityEngine;

namespace Assets.Source.UpgradeManagement
{
    public enum UpgradeType
    {
        Defender,
        Worker,
        Attacker,
        Miner,
    }

    /// <summary>
    /// Holds all available upgrades for golems
    /// </summary>
    public class UpgradePool : MonoBehaviour
    {
        private static UpgradePool instance;
        public static UpgradePool Instance => instance ?? (instance = FindObjectOfType<UpgradePool>());

        [SerializeField] private List<GameObject> golemsPrefabs;
        [SerializeField] public GameObject BaseGolemPrefab;
        private readonly Queue<Upgrade> upgrades = new Queue<Upgrade>();

        /// <summary>
        /// Adds upgrade to the pool
        /// </summary>
        /// <param name="upgrade"></param>
        public void Add(Upgrade upgrade)
        {
            upgrades.Enqueue(upgrade);
        }

        /// <summary>
        /// Returns any upgrade or null if non
        /// </summary>
        /// <returns></returns>
        public Upgrade TryGet()
        {
            if (!upgrades.Any())
                return null;

            return upgrades.Dequeue();
        }

        /// <summary>
        /// Returns golem prefab matching type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject GetPrefab(UpgradeType type)
        {
            return golemsPrefabs.First(p => p.GetComponent<GolemBase>().Specialization == type);
        }
    }
}
