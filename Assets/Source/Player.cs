using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1F;

        private Animator animator;
        private const string animatorWalkingParam = "Walking";

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.localScale = new Vector3(-1, 1, 1);
                transform.position -= new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                animator.SetBool(animatorWalkingParam, true);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                animator.SetBool(animatorWalkingParam, true);
            }
            else
            {
                animator.SetBool(animatorWalkingParam, false);
            }
        }
    }
}
