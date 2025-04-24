using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.AerialStates
{
    public class PlayerFlyState : PlayerAerialState
    {
        public PlayerFlyState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(movementData.AerialData.FlyData.SpeedModifier);
            stateMachine.Player.SetEnableAttack(true);
        }

        protected override void RotateTowardTargetRotation()
        {
            RotateTowardCameraDirection();
        }

        #region input methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            base.OnMovementCanceled(context);
        }

        protected override void OnRushStarted(InputAction.CallbackContext context)
        {
            if (stateMachine.Player.ReusableData.IsPerformingSkill) return;

            base.OnRushStarted(context);

            stateMachine.ChangeState(stateMachine.RushState);
        }
        #endregion
    }
}
