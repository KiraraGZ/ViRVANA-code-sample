using UnityEngine;
using UnityEngine.InputSystem;
using Magia.Skills;

namespace Magia.Player.StateMachines.MovementStates.AerialStates
{
    public class PlayerDashState : PlayerAerialState
    {
        private readonly DashSkill dashSkill;

        private float endTime;

        public PlayerDashState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashSkill = stateMachine.Player.SkillHandler.DashSkill;
        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(dashSkill.Data.SpeedModifier);
            stateMachine.Player.ReusableData.IsInvulnerable = true;
            stateMachine.Player.ReusableData.DashDirection = DetermineDashDirection();
            stateMachine.Player.SetEnableAttack(false);
            stateMachine.Player.Input.DisableActionForSeconds(stateMachine.Player.Input.PlayerActions.Dash, dashSkill.Data.DashDowntime);

            dashSkill.PerformSkill();

            endTime = Time.time + dashSkill.Data.DashUptime;
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.ReusableData.IsInvulnerable = false;
            stateMachine.Player.ReusableData.DashDirection = Vector2.zero;
            stateMachine.Player.StopMoveAlongCamera();
        }

        public override void Update()
        {
            base.Update();

            DetectGround();
            DetectWall();

            if (stateMachine.Player.ReusableData.DashDirection != Vector2.down && stateMachine.Player.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HoverState);
                stateMachine.Player.AnimatorController.StopDash();
                dashSkill.ReleaseSkill();
                return;
            }

            if (Time.time < endTime) return;

            stateMachine.Player.AnimatorController.StopDash();
            dashSkill.ReleaseSkill();

            if (stateMachine.Player.ReusableData.DashDirection == Vector2.down)
            {
                stateMachine.ChangeState(stateMachine.HoverState);
                return;
            }

            if (stateMachine.Player.ReusableData.DashDirection == Vector2.up)
            {
                stateMachine.ChangeState(stateMachine.RushState);
                return;
            }

            stateMachine.ChangeState(stateMachine.FlyState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            dashSkill.PhysicsUpdate();
        }

        #region main methods
        private Vector2 DetermineDashDirection()
        {
            Vector2 direction = stateMachine.Player.ReusableData.MovementInput;

            if (direction == Vector2.zero || direction.y < 0)
            {
                PerformBackwardDash();
                return Vector2.down;
            }

            if (direction.x != 0)
            {
                PerformSideDash(direction.x);
                return new Vector2(direction.x, 0);
            }

            PerformForwardDash();
            return Vector2.up;
        }

        private void PerformBackwardDash()
        {
            stateMachine.Player.AnimatorController.StartBackwardDash();

            Vector3 characterRotationDirection = -stateMachine.Player.transform.forward;
            stateMachine.Player.Rigidbody.velocity = dashSkill.Data.SpeedModifier * movementData.BaseSpeed * characterRotationDirection;
        }

        private void PerformSideDash(float direction)
        {
            stateMachine.Player.AnimatorController.StartSideDash(direction == 1);

            Vector3 characterRotationDirection = direction * stateMachine.Player.transform.right;
            stateMachine.Player.Rigidbody.velocity = dashSkill.Data.SpeedModifier * movementData.BaseSpeed * characterRotationDirection;
        }

        private void PerformForwardDash()
        {
            stateMachine.Player.StartMoveAlongCamera();
            stateMachine.Player.AnimatorController.StartForwardDash();

            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;
            stateMachine.Player.Rigidbody.velocity = dashSkill.Data.SpeedModifier * movementData.BaseSpeed * characterRotationDirection;
        }

        private void DetectGround()
        {
            Ray ray = new(stateMachine.Player.transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit _, movementData.GroundDetectRange))
            {
                stateMachine.ChangeState(stateMachine.SprintState);
                stateMachine.Player.AnimatorController.StopDash();
            }
        }

        private void DetectWall()
        {
            Vector3 dashDirection = stateMachine.Player.transform.forward;
            Ray ray = new(stateMachine.Player.transform.position, dashDirection);

            if (Physics.Raycast(ray, out RaycastHit hit, movementData.GroundDetectRange))
            {
                Vector3 surfaceNormal = hit.normal;

                float angle = Vector3.Angle(surfaceNormal, Vector3.up);

                if (angle > 60 && angle < 100)
                {
                    stateMachine.Player.AnimatorController.StopDash();
                    stateMachine.Player.ReusableData.DetectedWallNormal = hit.normal;
                    stateMachine.ChangeState(stateMachine.WallRunState);
                }
            }
        }
        #endregion

        #region input methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //Leave this blank to prevent state from entering the Hover State.
        }

        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
            //Leave this blank to prevent state from repeating enter the Dash State.
        }
        #endregion
    }
}
