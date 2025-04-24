using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.GroundedStates
{
    public class PlayerSprintState : PlayerGroundedState
    {
        public PlayerSprintState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(groundedData.SprintData.SpeedModifier);
            stateMachine.Player.SetEnableAttack(false);
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.ReusableData.IsRush = false;
        }

        private void StopSprint()
        {
            stateMachine.ChangeState(stateMachine.WalkState);
        }

        protected override void OnRushStopped(InputAction.CallbackContext context)
        {
            base.OnRushStopped(context);

            StopSprint();
        }
    }
}
