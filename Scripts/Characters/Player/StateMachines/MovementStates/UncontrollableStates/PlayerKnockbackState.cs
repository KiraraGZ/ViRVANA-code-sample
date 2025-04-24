using UnityEngine;

namespace Magia.Player.StateMachines.MovementStates.UncontrollableStates
{
    public class PlayerKnockbackState : PlayerUncontrollableState
    {
        private float startTime;

        public PlayerKnockbackState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            startTime = Time.time;
            stateMachine.Player.ReusableData.IsInvulnerable = true;
            stateMachine.Player.Rigidbody.useGravity = true;
            stateMachine.Player.AnimatorController.EnterKnockbackState();

            PerformPushBackward();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.ReusableData.IsInvulnerable = false;
            stateMachine.Player.Rigidbody.useGravity = false;
            stateMachine.Player.AnimatorController.ExitKnockbackState();
        }

        public override void Update()
        {
            base.Update();

            if (Time.time < startTime + uncontrollableData.KnockbackDuration) return;

            stateMachine.ChangeState(stateMachine.HoverState);
        }

        private void PerformPushBackward()
        {
            stateMachine.Player.Rigidbody.velocity = stateMachine.Player.ReusableData.KnockbackDirection * uncontrollableData.KnockbackForce;
        }
    }
}
