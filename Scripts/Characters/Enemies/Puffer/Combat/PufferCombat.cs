using System;
using System.Collections.Generic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public abstract class PufferCombat : MonoBehaviour
    {
        public event Action EventFiringEnd;

        protected List<IEnemySkill> skills;
        protected IEnemySkill currentSkill;

        protected BasePuffer basePuffer;
        protected PlayerController player => basePuffer.Player;

        public virtual void Initialize(BasePuffer basePuffer)
        {
            this.basePuffer = basePuffer;
        }

        public virtual void Dispose()
        {
            basePuffer = null;
        }

        public virtual void PhysicsUpdate()
        {
            currentSkill?.UpdateLogic();
        }

        public bool CheckSkillToCast()
        {
            if (skills == null) return false;

            foreach (var skill in skills)
            {
                if (!skill.IsAvailable()) continue;

                currentSkill = skill;
                return true;
            }

            return false;
        }

        public void StartFiring()
        {
            currentSkill.Cast();
        }

        protected void OnFiringEnd()
        {
            currentSkill = null;

            EventFiringEnd?.Invoke();
        }
    }
}