using System;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Whale
{
    public abstract class WhaleCombat : MonoBehaviour
    {
        public event Action EventSkillPerformed;
        public event Action EventSkillEnd;

        protected IEnemySkill currentSkill;
        protected int currentPhase;

        protected BaseWhale baseWhale;
        protected PlayerController player => baseWhale.Player;

        public virtual void Initialize(BaseWhale _baseWhale)
        {
            baseWhale = _baseWhale;
        }

        public virtual void Dispose()
        {
            baseWhale = null;
        }

        public virtual void PhysicsUpdate(WhaleState state)
        {
            currentSkill?.UpdateLogic();
        }

        public virtual void EnterPhase(int phase)
        {
            currentPhase = phase;
        }

        #region subscribe events
        protected void OnSkillPerformed()
        {
            EventSkillPerformed?.Invoke();
        }

        protected void OnSkillEnd()
        {
            currentSkill = null;

            EventSkillEnd?.Invoke();
        }
        #endregion
    }
}