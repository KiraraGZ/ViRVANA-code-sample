using UnityEngine;
using UnityEngine.InputSystem;
using Magia.Player.Data;

namespace Magia.Player.StateMachines.MovementStates.GroundedStates
{
    public class PlayerGroundedState : PlayerMovementState
    {
        protected PlayerGroundedData groundedData;
        private SlopeData slopeData;

        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;

            groundedData = movementData.GroundedData;
            slopeData = stateMachine.Player.Data.ColliderUtility.GetSlopeData();
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.Player.Rigidbody.useGravity = true;
            stateMachine.Player.EnterGroundState();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.Rigidbody.useGravity = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            DetectGround();
        }

        private void DetectGround()
        {
            Ray ray = new(stateMachine.Player.transform.position, Vector3.down);

            if (!Physics.Raycast(ray, out RaycastHit _, movementData.GroundDetectRange))
            {
                stateMachine.ChangeState(stateMachine.FallingState);
            }
        }

        //TODO: reimplement this logic if there is rough terrain in the game.
        private void Floating()
        {
            Ray ray = new(stateMachine.Player.Data.ColliderUtility.GetCenterPosition(), Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, slopeData.FloatRayDistance, stateMachine.Player.Data.LayerData.GroundLayer))
            {
                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(Vector3.Angle(hit.normal, -ray.direction));

                if (slopeSpeedModifier == 0f) return;

                float distanceToFloatingPoint = stateMachine.Player.Data.ColliderUtility.GetLocalCenterPosition().y * stateMachine.Player.transform.localScale.y - hit.distance;
                if (distanceToFloatingPoint == 0) return;

                float amountToLift = distanceToFloatingPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;

                stateMachine.Player.Rigidbody.AddForce(Vector3.up * amountToLift, ForceMode.VelocityChange);
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            stateMachine.Player.ReusableData.MovementSpeedOnSlopeModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

            return stateMachine.Player.ReusableData.MovementSpeedOnSlopeModifier;
        }

        #region reusable methods
        protected override void AddInputActionCallbacks()
        {
            base.AddInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Move.canceled += OnMovementCanceled;
            stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionCallbacks()
        {
            base.RemoveInputActionCallbacks();

            stateMachine.Player.Input.PlayerActions.Move.canceled -= OnMovementCanceled;
            stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }
        #endregion

        #region input methods
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

        protected virtual void OnJumpStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
        #endregion
    }
}
