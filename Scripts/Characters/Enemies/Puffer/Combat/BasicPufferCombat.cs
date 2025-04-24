using Magia.Enemy.Skills;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class BasicPufferCombat : PufferCombat
    {
        [SerializeField] protected Transform firingPoint;
        [SerializeField] private DirectFireSkillData data;
        protected DirectFireSkill skill;

        public override void Initialize(BasePuffer basePuffer)
        {
            base.Initialize(basePuffer);

            skill = new();
            skill.Initialize(data, firingPoint, basePuffer);
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
