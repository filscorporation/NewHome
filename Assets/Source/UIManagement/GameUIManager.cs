﻿using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Controls UI in game: energy, popups, menu
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        private static GameUIManager instance;
        public static GameUIManager Instance => instance ?? (instance = FindObjectOfType<GameUIManager>());

        [SerializeField] private EnergyBar energyBar;
        [SerializeField] private GameObject costUIPrefab;

        /// <summary>
        /// Show energy bar
        /// </summary>
        public void ActivateEnergyBar()
        {
            energyBar.Activate();
        }

        /// <summary>
        /// Hide energy bar
        /// </summary>
        public void DeactivateEnergyBar()
        {
            energyBar.Deactivate();
        }

        /// <summary>
        /// Show UI with cost in energy
        /// </summary>
        /// <param name="target"></param>
        /// <param name="size"></param>
        /// <param name="height"></param>
        public EnergyCostUI ShowCostUI(Transform target, int size, float height)
        {
            EnergyCostUI ec = Instantiate(costUIPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<EnergyCostUI>();
            ec.Initialize(target, size, height);
            return ec;
        }
    }
}
