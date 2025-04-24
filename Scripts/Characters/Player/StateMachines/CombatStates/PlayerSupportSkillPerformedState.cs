using Magia.Skills;

namespace Magia.Player.StateMachines.CombatStates
{
    public class PlayerSupportSkillPerformedState : PlayerCombatState
    {
        private SkillHandler skillHandler;

        public PlayerSupportSkillPerformedState(PlayerCombatStateMachine playerCombatStateMachine) : base(playerCombatStateMachine)
        {
            skillHandler = stateMachine.Player.SkillHandler;
        }

        public override void Enter()
        {
            skillHandler.PerformSupportSkill();
            stateMachine.ChangeState(stateMachine.IdleState);
        }

        public override void Exit()
        {
        }
    }
}