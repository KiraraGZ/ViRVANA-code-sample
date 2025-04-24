using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.MovementStates.AerialStates
{
    public class PlayerAerialState : PlayerMovementState
    {
        public PlayerAerialState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.Player.EnterAerialState();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            DetectGround();
        }

        #region reusable methods
        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Move.canceled += OnMovementCanceled;
            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Move.canceled -= OnMovementCanceled;
            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
        }
        #endregion

        private void DetectGround()
        {
            if (stateMachine.Player.transform.position.y < 0)
            {
                stateMachine.ChangeState(stateMachine.PullupState);
            }
        }

        #region input methods
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.HoverState);
        }

        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            if (stateMachine.Player.ReusableData.IsPerformingSkill) return;

            stateMachine.ChangeState(stateMachine.DashState);
        }
        #endregion
    }
}
