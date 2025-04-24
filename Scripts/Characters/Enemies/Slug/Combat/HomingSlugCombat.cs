using Magia.Enemy.Skills;
using UnityEngine;

namespace Magia.Enemy.Slug
{
    public class HomingSlugCombat : SlugCombat
    {
        [SerializeField] private JuggleHomingSkillData juggleData;
        [SerializeField] private Transform centerPoint;

        private JuggleHomingSkill skill;

        public override void Initialize(BaseSlug _basePuffer)
        {
            base.Initialize(_basePuffer);

            skill = new();
            skills = new() { skill };

            skill.Initialize(juggleData, centerPoint, slug);
            ChangeSkill(skill);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            float distanceToPlayer = (transform.position - Player.transform.position).magnitude;

            if (distanceToPlayer > attackRange) return;
        }

        public override void EnterIdleState()
        {
            skill.EnterIdle();
        }

        public override void EnterAlertState()
        {
            skill.EnterActive();
        }

        public override void EnterDeadState()
        {
            if (skill != null)
            {
                skill.Dispose();
                skill = null;
            }

            currentSkill = null;
        }
    }
}