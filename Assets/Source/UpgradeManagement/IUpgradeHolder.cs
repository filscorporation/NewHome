using UnityEngine;

namespace Assets.Source.UpgradeManagement
{
    /// <summary>
    /// Interface for objects (buildings) providing upgrades
    /// </summary>
    public interface IUpgradeHolder
    {
        /// <summary>
        /// Try to get item for the golem
        /// </summary>
        /// <returns></returns>
        bool TryGetItem();

        /// <summary>
        /// Returns holder transform
        /// </summary>
        /// <returns></returns>
        Transform GetTransform();
    }
}
