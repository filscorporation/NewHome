using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Assets.Source.UpgradeManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source.Units
{
    /// <summary>
    /// Golem that can shoot enemies from the distance to protect base
    /// </summary>
    public class DefenderGolem : GolemBase
    {
        private const string animatorShootParam = "Shoot";
        private const float toCenterDist = 2F;
        private float toCenterDistRnd = 2F;
        private const float toDefendDistMin = 1.0F;
        private const float toDefendDistMax = 1.8F;
        private float toDefendDistRnd;
        private const float wanderOutsideRadius = 6F;
        
        [SerializeField] [Range(0, 10F)] private float shootMaxRange = 2.5F;
        [SerializeField] [Range(0, 1F)] private float shootAccuracy = 0.8F;
        [SerializeField] [Range(1, 10)] private int shootDamage = 1;
        private const float shootAngleTan = 0.25F;
        private float shootTimeout = 0.2F;
        private float shootAfterTimeout = 0.1F;
        [SerializeField] [Range(0, 8F)] private float chargeTimeout = 3F;
        [SerializeField] private Transform shootStartPoint;
        [SerializeField] private GameObject projectilePrefab;
        protected static int leftSideDefendersCount = 0;
        protected static int rightSideDefendersCount = 0;
        private int defendSide = 0;

        private enum State
        {
            /// <summary>
            /// Golem just spawned and has no tasks to do
            /// </summary>
            None,

            /// <summary>
            /// Moving to center of the base to wait here for tasks
            /// </summary>
            ToCenter,

            /// <summary>
            /// In the night stands near far left or right wall to defend base
            /// </summary>
            Defend,

            /// <summary>
            /// When all walls destroyed - defend center
            /// </summary>
            DefendCenter,

            /// <summary>
            /// At daytime just wanders around outside of the walls
            /// </summary>
            WanderOutside,

            /// <summary>
            /// When wall was destroyed, backs up to next closest wall
            /// </summary>
            Backup,

            /// <summary>
            /// Moving to attack alien spawners
            /// </summary>
            Attack,
        }
        private State state = State.None;

        private enum SubState
        {
            /// <summary>
            /// Idle
            /// </summary>
            None,

            /// <summary>
            /// Is shooting some target
            /// </summary>
            Shoot,

            /// <summary>
            /// Charging next attack
            /// </summary>
            Charge,
        }
        private SubState substate = SubState.None;

        protected static System.Random rnd;

        protected override void Initialize()
        {
            if (rnd == null)
                rnd = new System.Random();
            Random.InitState(rnd.Next());
            toCenterDistRnd = Random.Range(toCenterDist * randomiseFactor, toCenterDist / randomiseFactor);
            toDefendDistRnd = Random.Range(toDefendDistMin, toDefendDistMax);
            Debug.Log(toDefendDistRnd);
        }

        private void Update()
        {
            Transform wall;
            switch (state)
            {
                case State.None:
                    if (!TryShoot())
                    {
                        targetPoint = center.position.x;
                        state = State.ToCenter;
                    }
                    break;
                case State.ToCenter:
                    if (substate != SubState.Shoot)
                        MoveToTarget();
                    if (Mathf.Abs(transform.position.x - targetPoint) < toCenterDistRnd)
                    {
                        if (DayNightCycleController.CycleState == CycleState.Day)
                        {
                            state = State.WanderOutside;
                        }
                        else
                        {
                            ChooseSideToDefend();
                            wall = defendSide == 1
                                ? MapManager.Instance.Walls.FarRight()
                                : MapManager.Instance.Walls.FarLeft();
                            if (wall == null || defendSide == -1 && wall.position.x > center.position.x
                                             || defendSide == 1 && wall.position.x < center.position.x)
                            {
                                // No walls left
                                targetPoint = center.position.x + defendSide * toDefendDistRnd * Random.Range(randomiseFactor, 1 / randomiseFactor);
                                state = State.DefendCenter;
                                break;
                            }
                            targetPoint = wall.position.x - defendSide * toDefendDistRnd * Random.Range(randomiseFactor, 1 / randomiseFactor);
                            state = State.Defend;
                        }
                    }
                    break;
                case State.Defend:
                    bool anyEnemy = TryShoot();

                    if (!anyEnemy && DayNightCycleController.CycleState == CycleState.Day)
                    {
                        state = State.WanderOutside;
                        break;
                    }

                    if (substate != SubState.Shoot)
                    {
                        MoveToTarget();
                        if (Mathf.Abs(transform.position.x - targetPoint) < Mathf.Epsilon)
                        {
                            // When reached position to defend - rotate to the wall
                            direction = defendSide;
                            transform.localScale = new Vector3(direction, 1, 1);
                        }
                    }
                    break;
                case State.DefendCenter:
                    bool anyEnemy1 = TryShoot();

                    if (!anyEnemy1 && DayNightCycleController.CycleState == CycleState.Day)
                    {
                        state = State.WanderOutside;
                        break;
                    }

                    if (substate != SubState.Shoot)
                    {
                        MoveToTarget();
                        if (Mathf.Abs(transform.position.x - targetPoint) < Mathf.Epsilon)
                        {
                            // When reached position to defend - rotate to defend side
                            direction = defendSide;
                            transform.localScale = new Vector3(direction, 1, 1);
                        }
                    }
                    break;
                case State.WanderOutside:
                    if (TryShoot())
                    {
                        state = State.Defend;
                        break;
                    }
                    ChooseSideToDefend();
                    if (DayNightCycleController.CycleState == CycleState.Night)
                    {
                        wall = defendSide == 1
                            ? MapManager.Instance.Walls.FarRight()
                            : MapManager.Instance.Walls.FarLeft();
                        if (wall == null || defendSide == -1 && wall.position.x > center.position.x
                                         || defendSide == 1 && wall.position.x < center.position.x)
                        {
                            targetPoint = center.position.x + defendSide * toDefendDistRnd * Random.Range(randomiseFactor, 1 / randomiseFactor);
                            state = State.DefendCenter;
                            break;
                        }
                        targetPoint = wall.position.x - defendSide * toDefendDistRnd * Random.Range(randomiseFactor, 1 / randomiseFactor);
                        state = State.Defend;
                        break;
                    }

                    // TODO: precalc
                    wall = defendSide == 1
                        ? MapManager.Instance.Walls.FarRight()
                        : MapManager.Instance.Walls.FarLeft();
                    if (wall == null || defendSide == -1 && wall.position.x > center.position.x
                                     || defendSide == 1 && wall.position.x < center.position.x)
                    {
                        if (substate != SubState.Shoot)
                            WanderAround(center.position.x + defendSide * wanderOutsideRadius, wanderOutsideRadius);
                        break;
                    }
                    if (substate != SubState.Shoot)
                        WanderAround(wall.position.x + defendSide * wanderOutsideRadius, wanderOutsideRadius);
                    break;
                case State.Backup:
                    throw new NotImplementedException();
                case State.Attack:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Makes golem to choose which side to defend for even distribution
        /// </summary>
        private void ChooseSideToDefend()
        {
            if (defendSide != 0)
            {
                if (Mathf.Abs(leftSideDefendersCount - rightSideDefendersCount) <= 1)
                    return;
            }

            if (leftSideDefendersCount == rightSideDefendersCount)
            {
                defendSide = Random.Range(0, 1F) > 0.5F ? 1 : -1;
            }
            else
            {
                defendSide = leftSideDefendersCount > rightSideDefendersCount ? 1 : -1;
            }
            if (defendSide == 1)
                rightSideDefendersCount++;
            else
                leftSideDefendersCount++;
        }

        private void RemoveFromSideDefendCount()
        {
            switch (defendSide)
            {
                case 1:
                    rightSideDefendersCount--;
                    break;
                case -1:
                    leftSideDefendersCount--;
                    break;
            }
        }

        private bool TryShoot()
        {
            Transform shootTarget = direction == 1
                ? MapManager.Instance.Enemies.FirstToTheRight(transform.position.x, shootMaxRange)
                : MapManager.Instance.Enemies.FirstToTheLeft(transform.position.x, shootMaxRange);
            if (shootTarget != null)
            {
                // Still possible target - golem will wait for charge or shoot and not move
                switch (substate)
                {
                    case SubState.None:
                        StartCoroutine(Shoot(shootTarget.position.x));
                        break;
                    case SubState.Shoot:
                    case SubState.Charge:
                        // Waiting
                    break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;
            }

            return false;
        }

        private IEnumerator Charge()
        {
            substate = SubState.Charge;
            yield return new WaitForSeconds(chargeTimeout);
            substate = SubState.None;
        }

        private IEnumerator Shoot(float x)
        {
            substate = SubState.Shoot;
            Animator.SetTrigger(animatorShootParam);
            yield return new WaitForSeconds(shootTimeout);

            GameObject go = Instantiate(projectilePrefab, shootStartPoint.position, Quaternion.identity);
            float shootRange = Mathf.Abs(shootStartPoint.position.x - x);
            shootRange = Mathf.Min(shootRange, shootMaxRange) * Random.Range(1, 1 / shootAccuracy);
            (float dx, float dy) speed = GetSpeed(
                shootRange * shootAngleTan,
                shootStartPoint.position.y - Projectile.GroundLevel,
                shootRange,
                Projectile.Gravity);
            go.GetComponent<Projectile>().Initialize(speed.dx, speed.dy, direction, shootDamage);
            yield return new WaitForSeconds(shootAfterTimeout);

            StartCoroutine(Charge());
        }

        private (float dx, float dy) GetSpeed(float h1, float h2, float s, float g)
        {
            float speedY = Mathf.Sqrt(2 * Projectile.Gravity * h1);
            float t = (speedY + Mathf.Sqrt(Mathf.Pow(speedY, 2) + 2 * g * h2)) / g;
            float speedX = s / t;
            return (speedX, speedY);
        }

        public override void Die()
        {
            IsActive = false;

            RemoveFromSideDefendCount();
            Instantiate(
                UpgradePool.Instance.BaseGolemPrefab,
                new Vector3(transform.position.x, CrystalPrefab.transform.position.y), 
                Quaternion.identity);
            Destroy(gameObject);

            base.Die();
        }
    }
}
