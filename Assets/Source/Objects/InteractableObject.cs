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
        private int energyStored = 0;

        private void Update()
        {
            float dist = Mathf.Abs(Player.Instance.transform.position.x - transform.position.x);
            if (isInRadius && dist > InteractRadius)
            {
                if (energyStored > 0)
                {
                    Player.Instance.GainEnergy(energyStored);
                    energyStored = 0;
                }

                isInRadius = false;
                Player.Instance.CurrentInteractable = null;
                Invoke(nameof(HideUI), 1F);
            }
            if (!isInRadius && dist < InteractRadius)
            {
                if (Player.Instance.CurrentInteractable != null)
                    return;

                isInRadius = true;
                Player.Instance.CurrentInteractable = this;
                if (IsInvoking(nameof(HideUI)))
                {
                    CancelInvoke();
                    HideUI();
                }
                ShowUI();
            }
        }

        private void ShowUI()
        {
            GameUIManager.Instance.ActivateEnergyBar();
            costUI = GameUIManager.Instance.ShowCostUI(transform, Cost, Height);
        }

        private void HideUI()
        {
            GameUIManager.Instance.DeactivateEnergyBar();
            StartCoroutine(costUI.Hide());
        }

        /// <summary>
        /// Called when player spent enough energy
        /// </summary>
        protected abstract void OnComplete();

        /// <summary>
        /// Pays energy to object
        /// </summary>
        public void AddEnergy()
        {
            costUI.AddEnergy();
            energyStored++;

            if (energyStored == Cost)
            {
                Player.Instance.CurrentInteractable = null;
                OnComplete();
            }
        }
    }
}
