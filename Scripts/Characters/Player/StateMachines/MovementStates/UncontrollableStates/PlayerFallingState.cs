using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.UncontrollableStates
{
    public class PlayerFallingState : PlayerUncontrollableState
    {
        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.Player.Rigidbody.useGravity = true;
            stateMachine.Player.AnimatorController.EnterFallingState();

            AddInputActionCallbacks();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.Rigidbody.useGravity = false;
            stateMachine.Player.AnimatorController.ExitFallingState();

            RemoveInputActionCallbacks();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            DetectGround();
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

        private void DetectGround()
        {
            Ray ray = new(stateMachine.Player.transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit _, 2))
            {
                stateMachine.ChangeState(stateMachine.IdleState);
                return;
            }

            if (stateMachine.Player.transform.position.y < 0)
            {
                stateMachine.ChangeState(stateMachine.PullupState);
            }
        }

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
            stateMachine.ChangeState(stateMachine.HoverState);
        }
        #endregion
    }
}
