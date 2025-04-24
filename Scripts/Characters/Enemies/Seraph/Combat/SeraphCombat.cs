using System;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Seraph
{
    public class SeraphCombat : MonoBehaviour
    {
        public event Action EventChantingStarted;
        public event Action EventChantingStopped;

        [SerializeField] private SeraphGateSkill gateSkill;
        protected IEnemySkill currentSkill;

        protected BaseSeraph baseSeraph;

        public virtual void Initialize(BaseSeraph _baseSeraph, PlayerController _player)
        {
            baseSeraph = _baseSeraph;
            gateSkill.Initialize(baseSeraph, _player);
        }

        public virtual void Dispose()
        {
            baseSeraph = null;
            gateSkill.Dispose();
        }

        public virtual void PhysicsUpdate()
        {
            if (gateSkill.IsAvailable() && !baseSeraph.IsOutOfHealth)
            {
                gateSkill.PhysicsUpdate();
            }

            currentSkill?.UpdateLogic();
        }

        protected void CastSkill(IEnemySkill skill)
        {
            skill.Cast();
            currentSkill = skill;

            EventChantingStarted?.Invoke();
        }

        public virtual void EnterDeadState()
        {

        }

        #region subscribe events
        protected void OnChantingStopped()
        {
            currentSkill = null;

            EventChantingStopped?.Invoke();
        }
        #endregion
    }
}