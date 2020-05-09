using Assets.Source.Objects.Interactable;
using Assets.Source.UpgradeManagement;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Base logic for all golem units
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class GolemBase : MonoBehaviour
    {
        private const string animatorDieParam = "Die";
        protected Animator Animator;
        protected bool IsActive = false;
        public bool IsDead { private set; get; } = false;
        [SerializeField] public UpgradeType Specialization;
        public GameObject DetransformEffect;
        public GameObject CrystalPrefab;

        [SerializeField] protected float Speed = 1F;
        protected Transform center;
        protected Transform target;
        private const float wanderWaitMin = 3F;
        private const float wanderWaitMax = 10F;
        private float wanderWaitTimer = wanderWaitMin;
        private float wanderSpeedModifier = 0.5F;
        private float? wanderWayPoint;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            center = FindObjectOfType<Spaceship>().transform;
        }

        private void Start()
        {
            Invoke(nameof(Activate), 2F);
        }

        protected void Activate()
        {
            IsActive = true;
        }

        protected void MoveToTarget()
        {
            transform.localScale = transform.position.x < target.position.x ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, target.position.x, Time.deltaTime * Speed),
                transform.position.y);
        }

        /// <summary>
        /// Makes golem to randomly move around x point in radius
        /// </summary>
        /// <param name="pivotX"></param>
        /// <param name="radius"></param>
        protected void WanderAround(float pivotX, float radius)
        {
            if (wanderWayPoint.HasValue)
            {
                transform.localScale = transform.position.x < wanderWayPoint.Value ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                transform.position = new Vector3(
                    Mathf.MoveTowards(transform.position.x, wanderWayPoint.Value, Time.deltaTime * Speed * wanderSpeedModifier),
                    transform.position.y);
                if (Mathf.Abs(transform.position.x - wanderWayPoint.Value) < Mathf.Epsilon)
                {
                    wanderWaitTimer = Random.Range(wanderWaitMin, wanderWaitMax);
                    wanderWayPoint = null;
                }
            }
            else
            {
                if (wanderWaitTimer > 0)
                {
                    wanderWaitTimer -= Time.deltaTime;
                    return;
                }

                wanderWayPoint = Random.Range(pivotX - radius, pivotX + radius);
            }
        }

        public virtual void Die()
        {
            CancelInvoke(nameof(Activate));
            IsDead = true;
            Animator.SetTrigger(animatorDieParam);
            Destroy(Instantiate(DetransformEffect, transform.position, Quaternion.identity), 2F);
        }
    }
}
