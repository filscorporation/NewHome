using Assets.Source.UpgradeManagement;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Golem that can shoot enemies from the distance to protect base
    /// </summary>
    public class DefenderGolem : GolemBase
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
