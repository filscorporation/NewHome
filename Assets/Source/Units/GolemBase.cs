using Assets.Source.Objects;
using Assets.Source.Objects.Interactable;
using Assets.Source.UpgradeManagement;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Base logic for all golem units
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class GolemBase : MonoBehaviour, IHitHandler
    {
        private const string animatorDieParam = "Die";
        protected Animator Animator;
        protected bool IsActive = false;
        public bool IsDead { private set; get; } = false;
        [SerializeField] public UpgradeType Specialization;
        public GameObject DetransformEffect;
        public GameObject CrystalPrefab;

        /// <summary>
        /// Scales some parameters for randomness
        /// </summary>
        protected const float randomiseFactor = 0.7F;

        [SerializeField] protected float Speed = 1F;
        [Range(0, 5F)] [SerializeField] private float sizeX = 0.5F;
        protected Transform center;
        protected float targetPoint;
        private const float wanderWaitMin = 3F;
        private const float wanderWaitMax = 10F;
        private float wanderWaitTimer = wanderWaitMin;
        private float wanderSpeedModifier = 0.5F;
        private float? wanderWayPoint;
        protected int direction = 1;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            center = FindObjectOfType<Spaceship>().transform;
        }

        private void Start()
        {
            Initialize();
            Invoke(nameof(Activate), 2F);
            MapManager.Instance.Targets.Add(transform, sizeX);
        }

        protected virtual void Initialize()
        {

        }

        private void OnDestroy()
        {
            MapManager.Instance.Targets.Remove(transform);
        }

        protected void Activate()
        {
            IsActive = true;
        }

        protected void MoveToTarget()
        {
            direction = transform.position.x < targetPoint ? 1 : -1;
            transform.localScale = new Vector3(direction, 1, 1);
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, targetPoint, Time.deltaTime * Speed),
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
                direction = transform.position.x < wanderWayPoint.Value ? 1 : -1;
                transform.localScale = new Vector3(direction, 1, 1);
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
            MapManager.Instance.Targets.Remove(transform);
            Animator.SetTrigger(animatorDieParam);
            Destroy(Instantiate(DetransformEffect, transform.position, Quaternion.identity), 2F);
        }
        
        /// <summary>
        /// Takes a hit
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="buildingDamage"></param>
        /// <returns>True if need to bounce</returns>
        public bool TakeHit(int damage, int buildingDamage)
        {
            Die();

            return false;
        }
    }
}
