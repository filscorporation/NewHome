using System;
using System.Collections;
using System.Linq;
using Assets.Source.Objects.Interactable;
using Assets.Source.UpgradeManagement;
using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Golem without specialization
    /// </summary>
    public class Golem : GolemBase
    {
        private const string animatorCrystalParam = "FromCrystal";
        private const string dieEffectTransformName = "DieEffectTransform";
        private Upgrade targetUpgrade;
        private const float toCenterDist = 2F;
        private const float toUpgradeDist = 0.5F;
        private const float checkUpgradeTimeout = 2F;
        private float checkUpgradeTimer = 0F;
        private const float wanderRadius = 5F;

        private enum State
        {
            None,
            ToCenter,
            Idle,
            ToUpgrade,
        }
        private State state = State.None;

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
                        target = center;
                        state = State.ToCenter;
                    }
                    else
                    {
                        target = targetUpgrade.Holder.GetTransform();
                        state = State.ToUpgrade;
                    }
                    break;
                case State.ToCenter:
                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - target.position.x) < toCenterDist)
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
                            target = targetUpgrade.Holder.GetTransform();
                            state = State.ToUpgrade;
                        }
                    }
                    WanderAround(center.position.x, wanderRadius);
                    break;
                case State.ToUpgrade:
                    MoveToTarget();
                    if (Mathf.Abs(transform.position.x - target.position.x) < toUpgradeDist)
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
            Vector2 dieEffectPos = GetComponentsInChildren<Transform>().First(c => c.name == dieEffectTransformName).position;
            Instantiate(prefab, dieEffectPos, Quaternion.identity);
            Destroy(gameObject);
        }

        public override void Die()
        {
            IsActive = false;

            if (state == State.ToUpgrade)
            {
                UpgradePool.Instance.Add(targetUpgrade);
                target = null;
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
