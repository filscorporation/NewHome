using Assets.Source.Objects;
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

        public InteractableObject CurrentInteractable;

        private void Start()
        {
            animator = GetComponent<Animator>();
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
                    CurrentInteractable.AddEnergy(1);
                }
            }
        }
    }
}
