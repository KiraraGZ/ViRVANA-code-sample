using System;
using Magia.Player;
using UnityEngine;

namespace Magia.Skills
{
    [Serializable]
    public class MagiaSkillSet : SkillSet
    {
        [SerializeField] private MagiaPassiveSkill passiveSkill;

        private ClockPrimarySkill ClockSkill => primarySkill as ClockPrimarySkill;

        public override void Initialize(SkillHandlerData data)
        {
            base.Initialize(data);

            ClockSkill.ApplyUpgrade(data.SkillUpgrade.Clock);
            ClockSkill.EventSkillRewardStack += OnClockSkillRewardStack;
            passiveSkill.Initialize(data.Player);
        }

        public override void Dispose()
        {
            base.Dispose();

            ClockSkill.EventSkillRewardStack -= OnClockSkillRewardStack;
            passiveSkill.Dispose();
        }

        public override void PhysicsUpdate(int index)
        {
            base.PhysicsUpdate(index);

            if (!isPostUlt) return;

            passiveSkill.UpdateLogic();
        }

        public override void EquipActiveSkills()
        {
            base.EquipActiveSkills();

            passiveSkill.Unequip();
        }

        public override bool IsSkillAvailable(int index)
        {
            if (isPostUlt) return false;

            return GetSkill(index).IsSkillAvailable();
        }

        public override SkillPerformedData PerformSkill(int index)
        {
            var skill = GetSkill(index);
            skill.PerformSkill();

            var zoomOut = index != 1;
            return new(skill.Type, zoomOut);
        }

        public override void PerformPassiveSkillAttack()
        {
            if (!isPostUlt) return;

            passiveSkill.PerformPassiveAttack();
        }

        #region subscribe events
        private void OnClockSkillRewardStack(int amount)
        {
            ultimateSkill.RecoverEnergy(amount);
        }

        protected override void OnUltSkillPerformed(SkillType type)
        {
            base.OnUltSkillPerformed(type);

            passiveSkill.Equip();
        }
        #endregion
    }
}
