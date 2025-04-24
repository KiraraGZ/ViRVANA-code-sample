using Magia.Player.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.WallStates
{
    public class PlayerWallSlideState : PlayerUncontrollableState
    {
        protected PlayerWallData wallData;

        private Vector3 wallNormal;

        public PlayerWallSlideState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            wallData = stateMachine.Player.Data.MovementData.WallData;
        }

        public override void Enter()
        {
            base.Enter();

            wallNormal = stateMachine.Player.ReusableData.DetectedWallNormal;

            stateMachine.Player.Rigidbody.useGravity = true;

            AddInputActionCallbacks();
        }

        public override void Exit()
        {
            base.Exit();


            stateMachine.Player.Rigidbody.useGravity = false;

            RemoveInputActionCallbacks();
        }

        #region reusable methods
        private void AddInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
            stateMachine.Player.Input.PlayerActions.Rush.started += OnRushStarted;
            stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        private void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
            stateMachine.Player.Input.PlayerActions.Rush.started -= OnRushStarted;
            stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }
        #endregion

        #region input methods
        private void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashState);
        }

        private void OnRushStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.RushState);
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            Vector3 jumpDirection = (wallNormal + Vector3.up).normalized;
            stateMachine.Player.Rigidbody.AddForce(jumpDirection * wallData.JumpForce, ForceMode.VelocityChange);

            stateMachine.ChangeState(stateMachine.HoverState);
        }
        #endregion
    }
}
