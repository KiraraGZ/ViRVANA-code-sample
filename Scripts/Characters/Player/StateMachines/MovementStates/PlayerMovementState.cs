using UnityEngine;
using UnityEngine.InputSystem;
using Magia.Player.Data;
using Magia.StateMachines;
using Magia.GameLogic;
using Magia.Utilities.Tools;

namespace Magia.Player.StateMachines.MovementStates
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine stateMachine;
        protected PlayerMovementData movementData;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;
            movementData = stateMachine.Player.Data.MovementData;

            stateMachine.Player.ReusableData.TimeToReachTargetRotation = movementData.TargetRotationReachTime;
        }

        public virtual void Enter()
        {
            AddInputActionCallbacks();
        }

        public virtual void Exit()
        {
            RemoveInputActionCallbacks();
        }

        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void Update()
        {

        }

        public virtual void PhysicsUpdate()
        {
            if (stateMachine.Player.ReusableData.MovementInput != Vector2.zero)
            {
                Move();
            }
            else
            {
                HandleIdle();
            }

            stateMachine.Player.UpdateVelocity(stateMachine.Player.transform.InverseTransformDirection(stateMachine.Player.Rigidbody.velocity));
            stateMachine.Player.Data.ColliderUtility.UpdateCollider(stateMachine.Player.ReusableData, GetMovementSpeed() / movementData.BaseSpeed);
        }

        public virtual void OnAnimationEnterEvent()
        {

        }

        public virtual void OnAnimationEndEvent()
        {

        }

        public virtual void OnAnimationTransitionEvent()
        {

        }

        #region main method
        private void ReadMovementInput()
        {
            stateMachine.Player.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Move.ReadValue<Vector2>();
        }

        private void Move()
        {
            Vector3 moveDirection = new(stateMachine.Player.ReusableData.MovementInput.x,
                                        0,
                                        stateMachine.Player.ReusableData.MovementInput.y);
            Vector2 directionAngle = Rotate(moveDirection);
            Vector3 targetRotationDirection = Quaternion.Euler(directionAngle.x, directionAngle.y, 0f) * Vector3.forward;

            Vector3 velocity = targetRotationDirection * GetMovementSpeed() - GetPlayerVelocity();
            stateMachine.Player.Rigidbody.AddForce(velocity, ForceMode.VelocityChange);

            if (stateMachine.Player.Rigidbody.velocity.magnitude > 0.01f) return;
            
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        private void HandleIdle()
        {
            if (stateMachine.Player.ReusableData.MovementSpeedModifier > 0) return;

            Vector3 targetRotationDirection = stateMachine.Player.Rigidbody.velocity.normalized;
            Vector3 velocity = targetRotationDirection * GetMovementSpeed() - GetPlayerVelocity();
            stateMachine.Player.Rigidbody.AddForce(velocity, ForceMode.VelocityChange);

            if (stateMachine.Player.Rigidbody.velocity.magnitude > 0.01f) return;
            
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        private Vector2 Rotate(Vector3 direction)
        {
            Vector2 moveAngle = UpdateMoveAngle(direction);

            RotateTowardTargetRotation();

            return moveAngle;
        }

        private Vector2 UpdateMoveAngle(Vector3 direction)
        {
            float directionAngleX = MathHelper.GetDirectionAngleX(direction) + stateMachine.Player.CameraTransform.eulerAngles.x;
            float directionAngleY = MathHelper.GetDirectionAngleY(direction) + stateMachine.Player.CameraTransform.eulerAngles.y;

            if (!stateMachine.Player.ReusableData.MoveAlongCamera) directionAngleX = 0;

            directionAngleX = MathHelper.ClampAngle(directionAngleX);

            if (directionAngleX != stateMachine.Player.ReusableData.CurrentTargetRotation.x)
            {
                stateMachine.Player.ReusableData.CurrentTargetRotation.x = directionAngleX;
                stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.x = 0f;
            }

            directionAngleY = MathHelper.ClampAngle(directionAngleY);

            if (directionAngleY != stateMachine.Player.ReusableData.CurrentTargetRotation.y)
            {
                stateMachine.Player.ReusableData.CurrentTargetRotation.y = directionAngleY;
                stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.y = 0f;
            }

            return new Vector2(directionAngleX, directionAngleY);
        }

        protected virtual void RotateTowardTargetRotation()
        {
            float currentPlayerXAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.x;
            float currentPlayerYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            float smoothXAngle = currentPlayerXAngle;
            float smoothYAngle = currentPlayerYAngle;

            if (currentPlayerXAngle != stateMachine.Player.ReusableData.CurrentTargetRotation.x)
            {
                smoothXAngle = Mathf.SmoothDampAngle(currentPlayerXAngle,
                                                     stateMachine.Player.ReusableData.CurrentTargetRotation.x,
                                                     ref stateMachine.Player.ReusableData.DampedTargetRotationCurrentVelocity.x,
                                                     stateMachine.Player.ReusableData.TimeToReachTargetRotation.x - stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.x);

                stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.x += Time.fixedDeltaTime;
            }

            if (currentPlayerYAngle != stateMachine.Player.ReusableData.CurrentTargetRotation.y)
            {
                smoothYAngle = Mathf.SmoothDampAngle(currentPlayerYAngle,
                                                     stateMachine.Player.ReusableData.CurrentTargetRotation.y,
                                                     ref stateMachine.Player.ReusableData.DampedTargetRotationCurrentVelocity.y,
                                                     stateMachine.Player.ReusableData.TimeToReachTargetRotation.y - stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.y);

                stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.y += Time.fixedDeltaTime;
            }

            stateMachine.Player.Rigidbody.MoveRotation(Quaternion.Euler(smoothXAngle, smoothYAngle, 0f));
        }

        protected void RotateTowardCameraDirection()
        {
            float currentPlayerXAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.x;
            float currentPlayerYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            float smoothXAngle = currentPlayerXAngle;
            float smoothYAngle = currentPlayerYAngle;

            if (currentPlayerXAngle != 0)
            {
                smoothXAngle = Mathf.SmoothDampAngle(currentPlayerXAngle,
                                                     0,
                                                     ref stateMachine.Player.ReusableData.DampedTargetRotationCurrentVelocity.x,
                                                     stateMachine.Player.ReusableData.TimeToReachTargetRotation.x - stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.x);

                stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.x += Time.fixedDeltaTime;
            }

            if (currentPlayerYAngle != stateMachine.Player.CameraTransform.eulerAngles.y)
            {
                smoothYAngle = Mathf.SmoothDampAngle(currentPlayerYAngle,
                                                     stateMachine.Player.CameraTransform.eulerAngles.y,
                                                     ref stateMachine.Player.ReusableData.DampedTargetRotationCurrentVelocity.y,
                                                     stateMachine.Player.ReusableData.TimeToReachTargetRotation.y - stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.y);

                stateMachine.Player.ReusableData.DampedTargetRotationPassedTime.y += Time.fixedDeltaTime;
            }

            stateMachine.Player.Rigidbody.MoveRotation(Quaternion.Euler(smoothXAngle, smoothYAngle, 0f));
        }

        #endregion

        protected void SetMovementSpeedModifier(float speedModifier)
        {
            stateMachine.Player.ReusableData.MovementSpeedModifier = speedModifier;
        }

        protected float GetMovementSpeed()
        {
            var currentSpeed = GetPlayerVelocity().magnitude;
            var targetSpeed = movementData.BaseSpeed * stateMachine.Player.ReusableData.MovementSpeedModifier * stateMachine.Player.ReusableData.MovementSpeedOnSlopeModifier;
            float speed;

            if (targetSpeed == 0)
            {
                speed = currentSpeed - movementData.BaseSpeed * Time.deltaTime / movementData.TargetSpeedReachTime;
            }
            else
            {
                speed = currentSpeed + (targetSpeed - currentSpeed) * Time.deltaTime / movementData.TargetSpeedReachTime;
            }

            if (speed < 0.01f) speed = 0;

            return speed;
        }

        protected Vector3 GetPlayerVelocity()
        {
            return stateMachine.Player.Rigidbody.velocity;
        }

        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
        }

        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        protected virtual void AddInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Rush.started += OnRushStarted;
            stateMachine.Player.Input.PlayerActions.Rush.canceled += OnRushStopped;
        }

        protected virtual void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Rush.started -= OnRushStarted;
            stateMachine.Player.Input.PlayerActions.Rush.canceled -= OnRushStopped;
        }

        #region input methods
        protected virtual void OnRushStarted(InputAction.CallbackContext context)
        {
            stateMachine.Player.ReusableData.IsRush = true;
        }

        protected virtual void OnRushStopped(InputAction.CallbackContext context)
        {

        }
        #endregion
    }
}
