using Magia.Player.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.WallStates
{
    public class PlayerWallRunState : PlayerMovementState
    {
        protected PlayerWallData wallData;

        private Vector3 wallNormal;
        private Vector3 wallMoveDirection;

        public PlayerWallRunState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            wallData = stateMachine.Player.Data.MovementData.WallData;
        }

        public override void Enter()
        {
            base.Enter();

            wallNormal = stateMachine.Player.ReusableData.DetectedWallNormal;
            wallMoveDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
            bool isRight = false;

            if (Vector3.Dot(wallMoveDirection, stateMachine.Player.transform.forward) < 0)
            {
                wallMoveDirection = -wallMoveDirection;
                isRight = true;
            }

            SetMovementSpeedModifier(wallData.SpeedModifier);
            stateMachine.Player.AnimatorController.EnterWallState(isRight);
            stateMachine.Player.SetEnableAttack(false);
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.AnimatorController.ExitWallState();
            stateMachine.Player.SetEnableAttack(true);
        }

        public override void PhysicsUpdate()
        {
            WallRun();
            DetectWall();
        }

        private void WallRun()
        {
            var velocity = wallMoveDirection * GetMovementSpeed();
            stateMachine.Player.Rigidbody.velocity = velocity;
            stateMachine.Player.Rigidbody.rotation = Quaternion.LookRotation(wallMoveDirection, Vector3.up);
        }

        private void DetectWall()
        {
            if (!Physics.Raycast(stateMachine.Player.transform.position, -wallNormal, out _, stateMachine.Player.Data.MovementData.GroundDetectRange))
            {
                stateMachine.ChangeState(stateMachine.RushState);
            }
        }

        #region reusable methods
        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
            stateMachine.Player.Input.PlayerActions.Move.canceled += OnMovementCanceled;
            stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
            stateMachine.Player.Input.PlayerActions.Move.canceled -= OnMovementCanceled;
            stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }
        #endregion

        #region input methods
        //TODO: Create wall dash/jump state that will transition player into rush state after that. (to avoid dash wall detect logic.)
        private void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashState);
        }

        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            // stateMachine.ChangeState(stateMachine.WallSlideState);
        }

        protected virtual void OnJumpStarted(InputAction.CallbackContext context)
        {
            Vector3 jumpDirection = (wallNormal + Vector3.up).normalized;
            stateMachine.Player.Rigidbody.AddForce(jumpDirection * wallData.JumpForce, ForceMode.VelocityChange);

            stateMachine.ChangeState(stateMachine.JumpState);
        }
        #endregion
    }
}
