using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Skills
{
    [Serializable]
    public class SkillSet
    {
        public event Action<DamageFeedback, Vector3> EventDamageDealt;
        public event Action<int, float> EventSkillEnergyChanged;
        public event Action<int, SkillType> EventSkillPerformed;
        public event Action<int, SkillType> EventSkillReleased;
        public event Action EventSkillEnd;

        [SerializeField] protected ElementType element;
        [SerializeField] protected EnergySkill primarySkill;
        [SerializeField] protected BarrierSkill barrierSkill;
        [SerializeField] protected EnergySkill ultimateSkill;

        [SerializeField] protected float downtime = 20f;

        public ElementType Element => element;
        protected bool isPostUlt;
        public bool IsPostUlt => isPostUlt;
        private float nextAvailableTime;

        public virtual void Initialize(SkillHandlerData data)
        {
            for (int i = 0; i < 3; i++)
            {
                var skill = GetSkill(i);
                skill.Initialize(new(data.UISkillIcons.Skills[i], data.UICrosshair.Crosshairs[skill.Type], data.Player));
                skill.EventSkillEnd += OnSkillEnd;
                skill.EventDamageDealt += OnDamageDealt;
            }

            AddListener();
        }

        public virtual void Dispose()
        {
            foreach (var skill in GetSkills())
            {
                skill.Dispose();
                skill.EventSkillEnd -= OnSkillEnd;
                skill.EventDamageDealt -= OnDamageDealt;
            }

            RemoveListener();
        }

        private void AddListener()
        {
            primarySkill.EventEnergyChanged += OnPrimarySkillEnergyChanged;
            primarySkill.EventSkillPerformed += OnPrimarySkillPerformed;
            primarySkill.EventSkillReleased += OnPrimarySkillReleased;

            barrierSkill.EventEnergyChanged += OnBarrerSkillEnergyChanged;
            barrierSkill.EventSkillPerformed += OnBarrierSkillPerformed;
            barrierSkill.EventSkillReleased += OnBarrierSkillReleased;

            ultimateSkill.EventEnergyChanged += OnUltSkillEnergyChanged;
            ultimateSkill.EventSkillPerformed += OnUltSkillPerformed;
            ultimateSkill.EventSkillReleased += OnUltSkillReleased;
        }

        private void RemoveListener()
        {
            primarySkill.EventEnergyChanged -= OnPrimarySkillEnergyChanged;
            primarySkill.EventSkillPerformed -= OnPrimarySkillPerformed;
            primarySkill.EventSkillReleased -= OnPrimarySkillReleased;

            barrierSkill.EventEnergyChanged -= OnBarrerSkillEnergyChanged;
            barrierSkill.EventSkillPerformed -= OnBarrierSkillPerformed;
            barrierSkill.EventSkillReleased -= OnBarrierSkillReleased;

            ultimateSkill.EventEnergyChanged -= OnUltSkillEnergyChanged;
            ultimateSkill.EventSkillPerformed -= OnUltSkillPerformed;
            ultimateSkill.EventSkillReleased -= OnUltSkillReleased;
        }

        public virtual void PhysicsUpdate(int index)
        {
            if (index < 0 || index >= GetSkills().Length) return;

            GetSkill(index).PhysicsUpdate();
        }

        public bool IsAvailable()
        {
            return Time.time >= nextAvailableTime;
        }

        public virtual void EquipActiveSkills()
        {
            isPostUlt = false;

            foreach (var skill in GetSkills())
            {
                skill.Equip();
            }
        }

        public virtual void UnequipActiveSkills()
        {
            foreach (var skill in GetSkills())
            {
                skill.Unequip();
            }
        }

        #region using skill
        public virtual bool IsSkillAvailable(int index)
        {
            if (isPostUlt) return false;

            return GetSkill(index).IsSkillAvailable();
        }

        public virtual SkillPerformedData PerformSkill(int index)
        {
            var skill = GetSkill(index);
            skill.PerformSkill();
            return new(skill.Type, false);
        }

        public virtual void PerformPassiveSkillAttack()
        {

        }

        public void RepeatSkill(int index)
        {
            GetSkill(index).RepeatSkill();
        }

        public void ReleaseSkill(int index)
        {
            GetSkill(index).ReleaseSkill();
        }
        #endregion

        public bool CheckBarrier(Damage damage, Vector3 direction)
        {
            if (barrierSkill == null) return false;

            return barrierSkill.CheckBarrier(damage, direction);
        }

        public void RefreshEnergy()
        {
            foreach (var skill in GetSkills())
            {
                skill.RecoverEnergy(1000);
            }
        }

        public void RefreshSkillCooldown()
        {
            foreach (var skill in GetSkills())
            {
                skill.RefreshCooldown();
            }
        }

        public void RefreshCooldown()
        {
            nextAvailableTime = 0f;
        }

        public virtual void AttackRecoverEnergy(int amount)
        {
            primarySkill.RecoverEnergy(amount);
            barrierSkill.RecoverEnergy(amount);
        }

        protected EnergySkill GetSkill(int index)
        {
            return index switch
            {
                0 => primarySkill,
                1 => barrierSkill,
                2 => ultimateSkill,
                _ => null,
            };
        }

        protected EnergySkill[] GetSkills()
        {
            return new[] { primarySkill, barrierSkill, ultimateSkill };
        }

        #region subscribe events
        private void OnDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            EventDamageDealt?.Invoke(feedback, hitPos);
        }

        protected virtual void OnSkillEnd()
        {
            EventSkillEnd?.Invoke();
        }

        #region primary skill
        protected void OnPrimarySkillEnergyChanged(float value)
        {
            EventSkillEnergyChanged?.Invoke(0, value);
        }

        protected void OnPrimarySkillPerformed(SkillType type)
        {
            EventSkillPerformed?.Invoke(0, type);
        }

        protected void OnPrimarySkillReleased(SkillType type)
        {
            EventSkillReleased?.Invoke(0, type);
        }
        #endregion

        #region barrier skill
        protected void OnBarrerSkillEnergyChanged(float value)
        {
            EventSkillEnergyChanged?.Invoke(1, value);
        }

        protected void OnBarrierSkillPerformed(SkillType type)
        {
            EventSkillPerformed?.Invoke(1, type);
        }

        protected void OnBarrierSkillReleased(SkillType type)
        {
            EventSkillReleased?.Invoke(1, type);
        }
        #endregion

        #region ultimate skill
        protected void OnUltSkillEnergyChanged(float value)
        {
            EventSkillEnergyChanged?.Invoke(2, value);
        }

        protected virtual void OnUltSkillPerformed(SkillType type)
        {
            EventSkillPerformed?.Invoke(2, type);
        }

        protected void OnUltSkillReleased(SkillType type)
        {
            nextAvailableTime = Time.time + downtime;
            EventSkillReleased?.Invoke(2, type);

            isPostUlt = true;

            UnequipActiveSkills();
        }
        #endregion
        #endregion
    }
}
