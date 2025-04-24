using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.AerialStates
{
    public class PlayerHoverState : PlayerAerialState
    {
        public PlayerHoverState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        private float verticalDirection;

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(0f);
            stateMachine.Player.SetEnableAttack(true);
            verticalDirection = 0;

            // ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            verticalDirection = 0;
        }

        public override void Update()
        {
            base.Update();

            if (stateMachine.Player.ReusableData.MovementInput == Vector2.zero) return;

            if (stateMachine.Player.ReusableData.IsRush)
            {
                stateMachine.ChangeState(stateMachine.RushState);
                return;
            }

            stateMachine.ChangeState(stateMachine.FlyState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            RotateTowardCameraDirection();
            VerticalMove();
        }

        #region reusable methods
        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Jump.started += OnFloatUpStarted;
            stateMachine.Player.Input.PlayerActions.Jump.canceled += OnFloatUpCanceled;
            stateMachine.Player.Input.PlayerActions.Crouch.started += OnFloatDownStarted;
            stateMachine.Player.Input.PlayerActions.Crouch.canceled += OnFloatDownCanceled;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Jump.started -= OnFloatUpStarted;
            stateMachine.Player.Input.PlayerActions.Jump.canceled -= OnFloatUpCanceled;
            stateMachine.Player.Input.PlayerActions.Crouch.started -= OnFloatDownStarted;
            stateMachine.Player.Input.PlayerActions.Crouch.canceled -= OnFloatDownCanceled;
        }
        #endregion

        protected void VerticalMove()
        {
            if (verticalDirection == 0)
            {
                stateMachine.Player.Rigidbody.AddForce(-stateMachine.Player.Rigidbody.velocity);
                return;
            }

            Vector3 moveDirection = Vector3.up * verticalDirection;
            Vector3 velocity = moveDirection * movementData.AerialData.VelocitySpeed - GetPlayerVelocity();
            stateMachine.Player.Rigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }

        #region input methods
        protected void OnFloatUpStarted(InputAction.CallbackContext context)
        {
            if (verticalDirection < 0) return;

            verticalDirection = 1;
        }

        protected void OnFloatUpCanceled(InputAction.CallbackContext context)
        {
            if (verticalDirection <= 0) return;

            verticalDirection = 0;
        }

        protected void OnFloatDownStarted(InputAction.CallbackContext context)
        {
            if (verticalDirection > 0) return;

            verticalDirection = -1;
        }

        protected void OnFloatDownCanceled(InputAction.CallbackContext context)
        {
            if (verticalDirection >= 0) return;

            verticalDirection = 0;
        }
        #endregion
    }
}
