using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Seraph
{
    public class RaphaelCombat : SeraphCombat
    {
        private VortexPillarSkill vortexPillarSkill;
        [SerializeField] private VortexPillarSkillData normalData;
        [SerializeField] private VortexPillarSkillData deathDoorData;

        public override void Initialize(BaseSeraph _baseSeraph, PlayerController _player)
        {
            base.Initialize(_baseSeraph, _player);

            vortexPillarSkill = new();
            vortexPillarSkill.Initialize(normalData, _baseSeraph);
            vortexPillarSkill.EventSkillEnd += OnChantingStopped;
        }

        public override void Dispose()
        {
            vortexPillarSkill.Dispose();
            vortexPillarSkill.EventSkillEnd -= OnChantingStopped;
            vortexPillarSkill = null;

            base.Dispose();
        }

        public override void PhysicsUpdate()
        {
            if (currentSkill == null && vortexPillarSkill.IsAvailable())
            {
                CastSkill(vortexPillarSkill);
            }

            base.PhysicsUpdate();
        }

        public override void EnterDeadState()
        {
            vortexPillarSkill.Dispose();
            vortexPillarSkill.Initialize(deathDoorData, baseSeraph);
            CastSkill(vortexPillarSkill);
        }
    }
}