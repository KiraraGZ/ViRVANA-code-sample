using Magia.Enemy.Skills;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Seraph
{
    public class MichaelCombat : SeraphCombat
    {
        private HomingPillarSkill homingPillarSkill;
        [SerializeField] private HomingPillarSkillData normalData;
        [SerializeField] private HomingPillarSkillData deathDoorData;

        public override void Initialize(BaseSeraph _baseSeraph, PlayerController _player)
        {
            base.Initialize(_baseSeraph, _player);

            homingPillarSkill = new();
            homingPillarSkill.Initialize(normalData, _baseSeraph);
            homingPillarSkill.EventSkillEnd += OnChantingStopped;
        }

        public override void Dispose()
        {
            homingPillarSkill.Dispose();
            homingPillarSkill.EventSkillEnd -= OnChantingStopped;
            homingPillarSkill = null;

            base.Dispose();
        }

        public override void PhysicsUpdate()
        {
            if (currentSkill == null && homingPillarSkill.IsAvailable())
            {
                CastSkill(homingPillarSkill);
            }

            base.PhysicsUpdate();
        }

        public override void EnterDeadState()
        {
            homingPillarSkill.Dispose();
            homingPillarSkill.Initialize(deathDoorData, baseSeraph);
            CastSkill(homingPillarSkill);
        }
    }
}