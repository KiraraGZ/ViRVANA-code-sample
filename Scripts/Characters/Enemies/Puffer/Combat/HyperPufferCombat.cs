using Magia.Enemy.Skills;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class HyperPufferCombat : PufferCombat
    {
        [SerializeField] private CatapultSkillData skillData;

        private CatapultSkill skill;

        public override void Initialize(BasePuffer basePuffer)
        {
            base.Initialize(basePuffer);

            skill = new();
            skill.Initialize(skillData, player, basePuffer);
            skill.EventSkillEnd += OnFiringEnd;

            skills = new() { skill };
        }

        public override void Dispose()
        {
            if (skill != null)
            {
                skill.EventSkillEnd -= OnFiringEnd;
                skill.Dispose();
                skill = null;
            }

            skills = null;

            base.Dispose();
        }
    }
}