using Assets.Source.UpgradeManagement;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Golem that builds and repairs buildings
    /// </summary>
    public class WorkerGolem : GolemBase
    {

        public override void Die()
        {
            IsActive = false;

            Instantiate(
                UpgradePool.Instance.BaseGolemPrefab,
                new Vector3(transform.position.x, CrystalPrefab.transform.position.y),
                Quaternion.identity);
            Destroy(gameObject);

            base.Die();
        }
    }
}
