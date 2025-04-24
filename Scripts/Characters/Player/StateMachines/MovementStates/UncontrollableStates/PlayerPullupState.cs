using UnityEngine;

namespace Magia.Player.StateMachines.MovementStates.UncontrollableStates
{
    public class PlayerPullupState : PlayerUncontrollableState
    {
        private float startTime;

        public PlayerPullupState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            startTime = Time.time;
            stateMachine.Player.ReusableData.IsInvulnerable = true;
            stateMachine.Player.AnimatorController.ToggleBarrier(true);
            stateMachine.Player.SelfTakeDamage(uncontrollableData.PullupDamage);

            stateMachine.Player.Rigidbody.velocity = Vector3.up * uncontrollableData.PullupForce;
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.ReusableData.IsInvulnerable = false;
            stateMachine.Player.AnimatorController.ToggleBarrier(false);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            stateMachine.Player.UpdateVelocity(Vector3.zero);

            if (Time.time < startTime + uncontrollableData.PullupDuration) return;
            if (stateMachine.Player.transform.position.y < 0) return;

            stateMachine.ChangeState(stateMachine.HoverState);
        }
    }
}
