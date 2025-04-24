using System;
using System.Collections.Generic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Slug
{
    public abstract class SlugCombat : MonoBehaviour
    {
        public Action EventPlayerExit;
        public Action EventChargeEnough;
        public Action EventChargeFull;

        public float attackRange;

        //TODO: Remove
        // public float minChargeTime;
        public float maxChargeTime;

        // public float chargeCooldown;

        protected List<IEnemySkill> skills;
        protected IEnemySkill currentSkill;
        // protected float nextAttackTime;
        // protected float skillCastTime;

        protected BaseSlug slug;
        protected PlayerController Player => slug.Player;

        public virtual void Initialize(BaseSlug baseSlug)
        {
            slug = baseSlug;

            skills = new();
            // nextAttackTime = Time.time + chargeCooldown;
        }

        public virtual void Dispose()
        {
            slug = null;
            skills = null;
        }

        public virtual void PhysicsUpdate()
        {
            // if (Time.time < nextAttackTime) return;

            // float distanceToPlayer = (transform.position - Player.transform.position).magnitude;

            // if (distanceToPlayer > attackRange) return;

            // foreach (var skill in skills)
            // {
            //     if (skill.IsAvailable())
            //     {
            //         ChangeSkill(skill);
            //         break;
            //     }
            // }

            currentSkill?.UpdateLogic();

            // if (currentSkill != null && Time.time - skillCastTime >= maxChargeTime)
            // {
            //     EventChargeFull?.Invoke();
            //     nextAttackTime = Time.time + chargeCooldown;
            //     ChangeSkill();
            // }

            // if (currentSkill == null || distanceToPlayer < attackRange * rangeThreshold) return;

            // if (skillCastTime - Time.time < minChargeTime)
            // {
            //     EventChargeEnough?.Invoke();
            //     ChangeSkill();
            // }
            // else
            // {
            //     EventPlayerExit?.Invoke();
            //     nextAttackTime = Time.time + chargeCooldown;
            //     ChangeSkill();
            // }
        }

        public abstract void EnterIdleState();
        public abstract void EnterAlertState();
        public abstract void EnterDeadState();

        public void ChangeSkill(IEnemySkill newSkill = null)
        {
            if (newSkill == null)
            {
                currentSkill = null;
                return;
            }

            if (newSkill == currentSkill) return;

            currentSkill = newSkill;
            currentSkill.Cast();
        }
    }
}