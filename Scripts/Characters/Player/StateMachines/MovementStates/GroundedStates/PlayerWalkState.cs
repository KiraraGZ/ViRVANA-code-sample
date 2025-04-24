using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.GroundedStates
{
    public class PlayerWalkState : PlayerGroundedState
    {
        public PlayerWalkState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(groundedData.WalkData.SpeedModifier);
            stateMachine.Player.SetEnableAttack(true);

            ResetVelocity();
        }

        protected override void RotateTowardTargetRotation()
        {
            RotateTowardCameraDirection();
        }

        #region input methods
        protected override void OnRushStarted(InputAction.CallbackContext context)
        {
            if (stateMachine.Player.ReusableData.IsPerformingSkill) return;

            base.OnRushStarted(context);

            stateMachine.ChangeState(stateMachine.RunState);
        }
        #endregion
    }
}
