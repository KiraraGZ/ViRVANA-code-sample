using Magia.Player.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.AerialStates
{
    public class PlayerRushState : PlayerAerialState
    {
        private readonly PlayerRushData rushData;

        private float startTime;
        private bool holdState;

        public PlayerRushState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            rushData = movementData.AerialData.RushData;
        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(rushData.SpeedModifier);
            stateMachine.Player.StartMoveAlongCamera();
            stateMachine.Player.SetEnableAttack(false);

            startTime = Time.time;
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.StopMoveAlongCamera();
            stateMachine.Player.ReusableData.IsRush = false;

            holdState = false;
        }

        public override void Update()
        {
            base.Update();

            if (GetMovementSpeed() < movementData.BaseSpeed * rushData.EnterHoverSpeedRatio)
            {
                StopRush();
                return;
            }

            if (holdState) return;
            if (stateMachine.Player.ReusableData.IsRush) return;
            if (Time.time < startTime + rushData.DashToRushTime) return;

            StopRush();
        }

        private void StopRush()
        {
            stateMachine.Player.ReusableData.IsRush = false;
            stateMachine.ChangeState(stateMachine.FlyState);
        }

        #region reusable methods
        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.HoldDash.performed += OnHoldDashPerformed;
            stateMachine.Player.Input.PlayerActions.HoldDash.canceled += OnHoldDashCanceled;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.HoldDash.performed -= OnHoldDashPerformed;
            stateMachine.Player.Input.PlayerActions.HoldDash.canceled -= OnHoldDashCanceled;
        }
        #endregion

        #region input methods
        private void OnHoldDashPerformed(InputAction.CallbackContext context)
        {
            holdState = true;
        }

        private void OnHoldDashCanceled(InputAction.CallbackContext context)
        {
            holdState = false;
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            base.OnMovementCanceled(context);

            stateMachine.Player.AnimatorController.StartBrake();
        }

        protected override void OnRushStopped(InputAction.CallbackContext context)
        {
            base.OnRushStopped(context);

            StopRush();
        }
        #endregion
    }
}
