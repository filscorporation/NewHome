using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Objects;
using Assets.Source.Objects.Interactable;
using Assets.Source.UIManagement;
using Assets.Source.Units;
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

            //Debug
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                FindObjectsOfType<GolemBase>().FirstOrDefault(g => !g.IsDead)?.Die();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(GainEnergy(5));
            }
        }

        /// <summary>
        /// Adds energy to player
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delay"></param>
        public IEnumerator GainEnergy(int value, float delay = 0F)
        {
            if (Mathf.Abs(delay) > Mathf.Epsilon)
                yield return new WaitForSeconds(delay);
            energy = Mathf.Min(energyMax, energy + value);
            GameUIManager.Instance.SetEnergyBarValue(energy, energyMax);
        }
    }
}
