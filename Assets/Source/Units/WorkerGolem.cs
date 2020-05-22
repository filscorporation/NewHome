using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Objects;
using Assets.Source.UpgradeManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source.Units
{
    /// <summary>
    /// Golem that builds and repairs buildings
    /// </summary>
    public class WorkerGolem : GolemBase
    {
        private const string animatorSignalParam = "Signal";
        private const string animatorWorkParam = "Work";
        private const float toCenterDist = 2F;
        private float toCenterDistRnd = 2F;
        private const float toWorkDist = 0.5F;
        private float toWorkDistRnd = 0.5F;
        private float wanderInsideRadius = 3F;
        private float wanderInsideRadiusRnd = 3F;
        private bool waitLookingForJob = false;
        private const float lookingForJobDelay = 1F;
        private const float enemySpotRange = 5;

        [SerializeField] [Range(0.01F, 10F)] private float buildingEffectiveness = 1F;
        [SerializeField] [Range(0.01F, 10F)] private float repairingEffectiveness = 1F;
        private IBuildable job;

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
            /// Just wander inside while waiting for job to do
            /// </summary>
            WanderInside,

            /// <summary>
            /// State before moving to task
            /// </summary>
            GettingSignal,

            /// <summary>
            /// Got task and moving to target position
            /// </summary>
            MovingToTask,

            /// <summary>
            /// Is working on its task
            /// </summary>
            IsWorking,

            /// <summary>
            /// Hides in base center at the night
            /// </summary>
            Hide,
        }
        private State state;

        private enum SubState
        {
            None,

            IsBuilding,

            IsRepairing,
        }
        private SubState subState;
        
        protected static System.Random rnd;

        protected override void Initialize()
        {
            if (rnd == null)
                rnd = new System.Random();
            Random.InitState(rnd.Next());
            toCenterDistRnd = Random.Range(toCenterDist * randomiseFactor, toCenterDist / randomiseFactor);
            toWorkDistRnd = Random.Range(toWorkDist * randomiseFactor, toWorkDist / randomiseFactor);
            wanderInsideRadiusRnd = Random.Range(wanderInsideRadius * randomiseFactor, wanderInsideRadius / randomiseFactor);
        }

        private void Update()
        {
            switch (state)
            {
                case State.None:
                    if (!TryFindJob())
                    {
                        targetPoint = center.position.x;
                        state = State.ToCenter;
                        break;
                    }
                    break;
                case State.ToCenter:
                    if (TryFindJob())
                    {
                        break;
                    }

                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - targetPoint) < toCenterDistRnd)
                    {
                        state = State.WanderInside;
                    }
                    break;
                case State.WanderInside:
                    if (DayNightCycleController.CycleState == CycleState.Day)
                    {
                        if (TryFindJob())
                        {
                            break;
                        }
                    }

                    WanderAround(center.position.x, wanderInsideRadiusRnd);
                    break;
                case State.GettingSignal:
                    break;
                case State.MovingToTask:
                    if (TrySpotEnemy())
                        break;
                    if (job == null)
                    {
                        state = State.None;
                        break;
                    }
                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - targetPoint) < toWorkDistRnd)
                    {
                        state = State.IsWorking;
                        Animator.SetBool(animatorWorkParam, true);
                        switch (job.Type())
                        {
                            case BuildableType.BuildingBase:
                                subState = SubState.IsBuilding;
                                break;
                            case BuildableType.Building:
                                subState = SubState.IsRepairing;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;
                case State.IsWorking:
                    if (TrySpotEnemy())
                        break;
                    if (job == null)
                    {
                        Animator.SetBool(animatorWorkParam, false);
                        state = State.None;
                        break;
                    }
                    if (job.Build(subState == SubState.IsRepairing
                        ? repairingEffectiveness * Time.deltaTime
                        : buildingEffectiveness * Time.deltaTime))
                    {
                        state = State.None;
                        Animator.SetBool(animatorWorkParam, false);
                        break;
                    }
                    break;
                case State.Hide:
                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - targetPoint) < toCenterDistRnd)
                    {
                        state = State.WanderInside;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool TrySpotEnemy()
        {
            Transform rightEnemy = MapManager.Instance.Enemies.FirstToTheRight(transform.position.x, enemySpotRange);
            Transform leftEnemy = MapManager.Instance.Enemies.FirstToTheLeft(transform.position.x, enemySpotRange);
            if (rightEnemy != null || leftEnemy != null)
            {
                if (job != null)
                {
                    Animator.SetBool(animatorWorkParam, false);
                    BuildingsManager.Instance.Free(job);
                    job = null;
                }
                targetPoint = center.position.x;
                state = State.Hide;
                return true;
            }

            return false;
        }

        private bool TryFindJob()
        {
            if (waitLookingForJob)
                return false;

            if ((job = BuildingsManager.Instance.GetJob()) == null)
            {
                waitLookingForJob = true;
                StartCoroutine(StopWaitingToLookForJob());
                return false;
            }
            Animator.SetTrigger(animatorSignalParam);

            state = State.GettingSignal;
            StartCoroutine(StartMovingToTask());
            targetPoint = job.GetX();

            return true;
        }

        private IEnumerator StopWaitingToLookForJob()
        {
            yield return new WaitForSeconds(lookingForJobDelay);
            waitLookingForJob = false;
        }

        private IEnumerator StartMovingToTask()
        {
            yield return new WaitForSeconds(1F);
            state = State.MovingToTask;
        }

        public override void Die()
        {
            if (job != null)
            {
                BuildingsManager.Instance.Free(job);
                job = null;
            }

            IsActive = false;

            Instantiate(
                UpgradePool.Instance.BaseGolemPrefab,
                new Vector3(transform.position.x, CrystalPrefab.transform.position.y),
                Quaternion.identity);
            Destroy(gameObject);

            base.Die();
        }
    }
}
