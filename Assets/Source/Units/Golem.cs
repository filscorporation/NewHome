using System.Collections;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Logic of base game unit
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Golem : MonoBehaviour
    {
        private const string animatorCrystalParam = "FromCrystal";
        private Animator animator;
        private bool active = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Animates golem as made from crystal
        /// </summary>
        public IEnumerator FromCrystal()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger(animatorCrystalParam);
            yield return new WaitForSeconds(2F);
            active = true;
        }
    }
}
