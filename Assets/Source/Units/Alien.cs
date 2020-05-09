using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Objects;
using Assets.Source.Objects.Interactable;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// In game enemy
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Alien : MonoBehaviour
    {
        private const string chargeAnimatorParam = "Charge";
        private const string jumpAnimatorParam = "Jump";
        private const string bounceAnimatorParam = "Bounce";
        private const string fedAnimatorParam = "Fed";
        private const string landAnimatorParam = "Land";
        private const string groundAnimatorParam = "Ground";
        private const string walkAnimatorParam = "Walk";
        private Animator animator;

        [Range(0, 20F)] [SerializeField] private float speed;
        [Range(1, 50)] [SerializeField] private float health;
        [Range(1, 10)] [SerializeField] private int damage;
        [Range(1, 20)] [SerializeField] private int buildingDamage;
        [Range(0, 10F)] [SerializeField] private float jumpRange;
        [Range(0, 3F)] [SerializeField] private float jumpHeight;
        [Range(0, 5F)] [SerializeField] private float chargeTime;
        [Range(0, 1F)] [SerializeField] private float attackRange;

        private int direction = 1;
        private bool isActive = true;
        private GameObject target;
        private float jumpSpeedX;
        private float jumpSpeedY;
        private const float groundLevel = -3.5F;
        private const float gravity = 10F;

        private enum State
        {
            Walk,
            Charge,
            Jump,
            Bounce,
            Fed,
            Ground,
        }

        private State state = State.Walk;
        private bool isDead = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
            if (transform.position.x < FindObjectOfType<Spaceship>().transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            transform.localScale = new Vector3(direction, 1);
        }

        private void FixedUpdate()
        {
            if (!isActive)
                return;

            switch (state)
            {
                case State.Walk:
                    transform.position += new Vector3(direction * speed * Time.fixedDeltaTime, 0);
                    if (Mathf.Abs(transform.position.x) > MapManager.Instance.GetMapSize() / 2F)
                    {
                        Die();
                    }

                    target = direction == 1
                        ? MapManager.Instance.Targets.FirstToTheRight(transform.position.x, jumpRange / 2F)?.gameObject
                        : MapManager.Instance.Targets.FirstToTheLeft(transform.position.x, jumpRange / 2F)?.gameObject;
                    if (target != null)
                    {
                        StartCoroutine(Charge());
                    }
                    break;
                case State.Charge:
                    break;
                case State.Jump:
                    transform.position += new Vector3(direction * jumpSpeedX, jumpSpeedY) * Time.fixedDeltaTime;
                    jumpSpeedY -= gravity * Time.fixedDeltaTime;
                    if (transform.position.y < groundLevel)
                    {
                        // If died in the air
                        if (isDead)
                            Die();
                        else
                        {
                            state = State.Walk;
                            animator.SetTrigger(landAnimatorParam);
                            break;
                        }
                    }
                    GameObject hit = direction == 1
                        ? MapManager.Instance.Targets.FirstToTheRight(transform.position.x, attackRange)?.gameObject
                        : MapManager.Instance.Targets.FirstToTheLeft(transform.position.x, attackRange)?.gameObject;
                    if (hit != null)
                    {
                        Camera.main.GetComponent<CameraController>().Shake(0.05F);
                        bool bounce = hit.GetComponent<IHitHandler>().TakeHit(damage, buildingDamage);
                        if (bounce)
                        {
                            state = State.Bounce;
                            animator.SetTrigger(bounceAnimatorParam);
                            jumpSpeedY = 0;
                        }
                        else
                        {
                            state = State.Fed;
                            animator.SetTrigger(fedAnimatorParam);
                        }
                    }
                    break;
                case State.Bounce:
                    transform.position += new Vector3(-direction * jumpSpeedX, jumpSpeedY) * Time.fixedDeltaTime;
                    jumpSpeedY -= gravity * Time.fixedDeltaTime;
                    if (transform.position.y < groundLevel)
                    {
                        // If died in the air
                        if (isDead)
                            Die();
                        else
                        {
                            state = State.Walk;
                            animator.SetTrigger(landAnimatorParam);
                        }
                    }
                    break;
                case State.Fed:
                    transform.position += new Vector3(direction * jumpSpeedX, jumpSpeedY) * Time.fixedDeltaTime;
                    jumpSpeedY -= gravity * Time.fixedDeltaTime;
                    if (transform.position.y < groundLevel)
                    {
                        StartCoroutine(Ground());
                        break;
                    }
                    break;
                case State.Ground:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator Charge()
        {
            state = State.Charge;
            animator.SetTrigger(chargeAnimatorParam);
            yield return new WaitForSeconds(chargeTime);
            Jump();
        }

        private void Jump()
        {
            state = State.Jump;
            animator.SetTrigger(jumpAnimatorParam);
            jumpSpeedY = Mathf.Sqrt(2 * gravity * jumpHeight);
            float t = 2 * jumpSpeedY / gravity;
            jumpSpeedX = jumpRange / t;
        }

        private IEnumerator Ground()
        {
            state = State.Ground;
            yield return new WaitForSeconds(0.1F);
            animator.SetTrigger(groundAnimatorParam);
            yield break;
        }
        
        /// <summary>
        /// Makes alien to take damage
        /// </summary>
        /// <param name="dmg"></param>
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health < 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            if (state == State.Walk || state == State.Charge)
            {
                state = State.Ground;
                animator.SetTrigger(groundAnimatorParam);
                Destroy(gameObject, 2F);
                isActive = false;
            }
        }
    }
}
