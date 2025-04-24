using UnityEngine;
using Magia.Player.Data;

namespace Magia.Player.StateMachines.MovementStates.UncontrollableStates
{
    public class PlayerJumpState : PlayerUncontrollableState
    {
        private PlayerJumpData jumpData;

        private float startTime;

        public PlayerJumpState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            jumpData = uncontrollableData.JumpData;
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.Player.AnimatorController.EnterJumpState();

            startTime = Time.time;

            Jump();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.AnimatorController.ExitJumpState();
        }

        public override void Update()
        {
            base.Update();

            if (Time.time < startTime + jumpData.JumpTime) return;

            stateMachine.ChangeState(stateMachine.HoverState);
        }

        #region main methods
        private void Jump()
        {
            Vector3 jumpForce = jumpData.JumpForce;
            Vector3 playerForward = stateMachine.Player.transform.forward;

            jumpForce.x *= playerForward.x;
            jumpForce.z *= playerForward.z;

            stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }
        #endregion
    }
}
