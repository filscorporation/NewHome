using Assets.Source.UIManagement;
using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Object that activates energy UI and allows to spend it
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        protected abstract int Cost { get; }
        protected abstract float InteractRadius { get; }
        protected abstract float Height { get; }
        private bool isInRadius = false;
        private EnergyCostUI costUI;

        private void Update()
        {
            float dist = Vector3.Distance(Player.Instance.transform.position, transform.position);
            if (isInRadius && dist > InteractRadius)
            {
                isInRadius = false;
                HideUI();
            }
            if (!isInRadius && dist < InteractRadius)
            {
                isInRadius = true;
                ShowUI();
            }
        }

        private void ShowUI()
        {
            Player.Instance.CurrentInteractable = this;
            GameUIManager.Instance.ActivateEnergyBar();
            costUI = GameUIManager.Instance.ShowCostUI(transform, Cost, Height);
        }

        private void HideUI()
        {
            Player.Instance.CurrentInteractable = null;
            GameUIManager.Instance.DeactivateEnergyBar();
            Destroy(costUI.gameObject);
        }

        /// <summary>
        /// Called when player spent enough energy
        /// </summary>
        protected abstract void OnComplete();

        /// <summary>
        /// Pays energy to object
        /// </summary>
        /// <param name="amount"></param>
        public void AddEnergy(int amount)
        {
            // TODO: temp
            OnComplete();
        }
    }
}
