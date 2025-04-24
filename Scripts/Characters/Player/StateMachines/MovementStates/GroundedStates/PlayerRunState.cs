using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.GroundedStates
{
    public class PlayerRunState : PlayerGroundedState
    {
        public PlayerRunState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(groundedData.RunData.SpeedModifier);
            stateMachine.Player.SetEnableAttack(true);
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.ReusableData.IsRush = false;
        }

        private void StopRun()
        {
            stateMachine.ChangeState(stateMachine.WalkState);
        }

        protected override void OnRushStopped(InputAction.CallbackContext context)
        {
            base.OnRushStopped(context);

            StopRun();
        }
    }
}