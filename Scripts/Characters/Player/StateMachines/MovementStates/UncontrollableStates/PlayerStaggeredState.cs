using UnityEngine;

namespace Magia.Player.StateMachines.MovementStates.UncontrollableStates
{
    public class PlayerStaggeredState : PlayerUncontrollableState
    {
        private float startTime;

        public PlayerStaggeredState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            startTime = Time.time;
            stateMachine.Player.ReusableData.IsInvulnerable = true;
            stateMachine.Player.AnimatorController.EnterStaggeredState();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.Player.ReusableData.IsInvulnerable = false;
            stateMachine.Player.AnimatorController.ExitStaggeredState();
        }

        public override void Update()
        {
            base.Update();

            if (Time.time < startTime + uncontrollableData.StaggeredDuration) return;

            ExitStaggered();
        }

        private void ExitStaggered()
        {
            Ray ray = new(stateMachine.Player.transform.position, Vector3.down);

            if (!Physics.Raycast(ray, out RaycastHit _, uncontrollableData.GroundDetectRange))
            {
                stateMachine.ChangeState(stateMachine.HoverState);
                return;
            }

            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
