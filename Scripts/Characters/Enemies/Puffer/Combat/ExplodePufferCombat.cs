using Magia.Enemy.Skills;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class ExplodePufferCombat : BasicPufferCombat
    {
        [SerializeField] private DirectExplodeSkillData explode;
        protected DirectExplodeSkill explodeSkill;

        private int phase;

        public override void Initialize(BasePuffer basePuffer)
        {
            base.Initialize(basePuffer);

            explodeSkill = new();
            explodeSkill.Initialize(explode, firingPoint, basePuffer);
            explodeSkill.EventSkillEnd += OnFiringEnd;

            skills.Insert(0, explodeSkill);
            phase = 0;
        }

        public override void Dispose()
        {
            if (explodeSkill != null)
            {
                explodeSkill.EventSkillEnd -= OnFiringEnd;
                explodeSkill.Dispose();
                explodeSkill = null;
            }

            base.Dispose();
        }

        public void EnterPhase(int _phase)
        {
            phase = _phase;
        }

        public void FireAtOnce()
        {
            switch (phase)
            {
                case 0:
                    skill.FireAtOnce(5);
                    break;
                case 1:
                case 2:
                    if (!explodeSkill.IsAvailable())
                    {
                        explodeSkill.FireAtOnce(1);
                    }
                    else
                    {
                        skill.FireAtOnce(8);
                    }
                    break;
            }
        }
    }
}
