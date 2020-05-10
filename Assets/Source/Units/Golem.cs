using System;
using System.Collections;
using System.Linq;
using Assets.Source.Objects.Interactable;
using Assets.Source.UpgradeManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source.Units
{
    /// <summary>
    /// Golem without specialization
    /// </summary>
    public class Golem : GolemBase
    {
        private const string animatorCrystalParam = "FromCrystal";
        [SerializeField] private Transform dieEffectTransform;
        private Upgrade targetUpgrade;
        private const float toCenterDist = 2F;
        private float toCenterDistRnd = 2F;
        private const float toUpgradeDist = 0.5F;
        private const float checkUpgradeTimeout = 2F;
        private float checkUpgradeTimer = 0F;
        private const float wanderRadius = 5F;

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
            /// Golem just wanders around center waiting for upgrades
            /// </summary>
            Idle,

            /// <summary>
            /// Got info about available upgrade, on his way to it
            /// </summary>
            ToUpgrade,
        }
        private State state = State.None;

        protected override void Initialize()
        {
            toCenterDistRnd = Random.Range(toCenterDist * randomiseFactor, toCenterDist / randomiseFactor);
        }

        private void Update()
        {
            if (!IsActive)
                return;

            switch (state)
            {
                case State.None:
                    targetUpgrade = UpgradePool.Instance.TryGet();
                    if (targetUpgrade == null)
                    {
                        targetPoint = center.position.x;
                        state = State.ToCenter;
                    }
                    else
                    {
                        targetPoint = targetUpgrade.Holder.GetTransform().position.x;
                        state = State.ToUpgrade;
                    }
                    break;
                case State.ToCenter:
                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - targetPoint) < toCenterDistRnd)
                        state = State.Idle;
                    break;
                case State.Idle:
                    checkUpgradeTimer -= Time.deltaTime;
                    if (checkUpgradeTimer < 0)
                    {
                        targetUpgrade = UpgradePool.Instance.TryGet();
                        if (targetUpgrade == null)
                        {
                            checkUpgradeTimer = checkUpgradeTimeout;
                        }
                        else
                        {
                            targetPoint = targetUpgrade.Holder.GetTransform().position.x;
                            state = State.ToUpgrade;
                        }
                    }
                    WanderAround(center.position.x, wanderRadius);
                    break;
                case State.ToUpgrade:
                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - targetPoint) < toUpgradeDist)
                        TakeUpgrade();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TakeUpgrade()
        {
            if (!targetUpgrade.Holder.TryGetItem())
            {
                state = State.None;
                return;
            }

            IsActive = false;
            GameObject prefab = UpgradePool.Instance.GetPrefab(targetUpgrade.Type);
            Vector2 dieEffectPos = dieEffectTransform.position;
            Instantiate(prefab, dieEffectPos, Quaternion.identity);
            Destroy(gameObject);
        }

        public override void Die()
        {
            IsActive = false;

            if (state == State.ToUpgrade)
            {
                UpgradePool.Instance.Add(targetUpgrade);
            }

            state = State.None;
            
            Instantiate(CrystalPrefab, new Vector3(transform.position.x, CrystalPrefab.transform.position.y), Quaternion.identity);
            Destroy(gameObject, 2F);

            base.Die();
        }

        /// <summary>
        /// Animates golem as made from crystal
        /// </summary>
        public IEnumerator FromCrystal()
        {
            Animator.SetTrigger(animatorCrystalParam);
            yield break;
        }
    }
}
