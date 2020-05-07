using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Energy bar UI logic
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class EnergyBar : MonoBehaviour
    {
        private bool energyBarActive = false;
        private Animator animator;
        private const string animatorActiveParam = "Active";

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            if (!energyBarActive)
                return;
        }

        /// <summary>
        /// Show energy bar
        /// </summary>
        public void Activate()
        {
            if (energyBarActive)
                return;

            energyBarActive = true;
            animator.SetBool(animatorActiveParam, true);
        }

        /// <summary>
        /// Hide energy bar
        /// </summary>
        public void Deactivate()
        {
            if (!energyBarActive)
                return;

            energyBarActive = false;
            animator.SetBool(animatorActiveParam, false);
        }
    }
}
