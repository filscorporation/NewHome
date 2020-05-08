using Assets.Source.Objects;
using Assets.Source.UIManagement;
using UnityEngine;

namespace Assets.Source
{
    /// <summary>
    /// Player
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        private static Player instance;
        public static Player Instance => instance ?? (instance = FindObjectOfType<Player>());

        [SerializeField] private float movementSpeed = 1F;

        private Animator animator;
        private const string animatorWalkingParam = "Walking";

        private int energy = 5;
        private int energyMax = 20;

        public InteractableObject CurrentInteractable;

        private void Start()
        {
            animator = GetComponent<Animator>();
            GameUIManager.Instance.SetEnergyBarValue(energy, energyMax);
        }

        private void Update()
        {
            ReadInput();
        }

        private void ReadInput()
        {
            bool animate = false;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.localScale = new Vector3(-1, 1, 1);
                transform.position -= new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                animator.SetBool(animatorWalkingParam, true);
                animate = true;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                animator.SetBool(animatorWalkingParam, true);
                animate = true;
            }
            if (!animate)
            {
                animator.SetBool(animatorWalkingParam, false);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (CurrentInteractable != null)
                {
                    if (energy > 0)
                    {
                        energy--;
                        GameUIManager.Instance.SetEnergyBarValue(energy, energyMax);
                        CurrentInteractable.AddEnergy();
                    }
                }
            }
        }

        /// <summary>
        /// Adds energy to player
        /// </summary>
        /// <param name="value"></param>
        public void GainEnergy(int value)
        {
            energy = Mathf.Min(energyMax, energy + value);
            GameUIManager.Instance.SetEnergyBarValue(energy, energyMax);
        }
    }
}
