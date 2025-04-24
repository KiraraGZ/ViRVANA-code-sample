using Magia.Enemy.Skills;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Seraph
{
    public class GabrielCombat : SeraphCombat
    {
        private GlobalGateSkill globalGateSkill;
        [SerializeField] private GlobalGateSkillData normalData;
        [SerializeField] private GlobalGateSkillData deathDoorData;

        public override void Initialize(BaseSeraph _baseSeraph, PlayerController _player)
        {
            base.Initialize(_baseSeraph, _player);

            globalGateSkill = new();
            globalGateSkill.Initialize(normalData, _baseSeraph);
            globalGateSkill.EventSkillEnd += OnChantingStopped;
        }

        public override void Dispose()
        {
            globalGateSkill.Dispose();
            globalGateSkill.EventSkillEnd -= OnChantingStopped;
            globalGateSkill = null;

            base.Dispose();
        }

        public override void PhysicsUpdate()
        {
            if (currentSkill == null && globalGateSkill.IsAvailable())
            {
                CastSkill(globalGateSkill);
            }

            base.PhysicsUpdate();
        }

        public override void EnterDeadState()
        {
            globalGateSkill.Dispose();
            globalGateSkill.Initialize(deathDoorData, baseSeraph);
            CastSkill(globalGateSkill);
        }
    }
}