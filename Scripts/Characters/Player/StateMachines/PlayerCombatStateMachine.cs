using Magia.StateMachines;
using Magia.Player.StateMachines.CombatStates;

namespace Magia.Player.StateMachines
{
    public class PlayerCombatStateMachine : StateMachine
    {
        public PlayerController Player { get; }

        public PlayerIdleState IdleState { get; }
        public PlayerAttackState AttackState { get; }
        public PlayerSkillPerformedState SkillPerformedState { get; }
        public PlayerSupportSkillPerformedState SupportSkillPerformedState { get; }

        public PlayerCombatStateMachine(PlayerController player)
        {
            Player = player;

            IdleState = new PlayerIdleState(this);
            AttackState = new PlayerAttackState(this);
            SkillPerformedState = new PlayerSkillPerformedState(this);
            SupportSkillPerformedState = new PlayerSupportSkillPerformedState(this);
        }

        public override void ChangeState(IState newState)
        {
            base.ChangeState(newState);
        }
    }
}