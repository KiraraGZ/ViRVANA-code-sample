using Magia.StateMachines;
using Magia.Player.StateMachines.MovementStates.AerialStates;
using Magia.Player.StateMachines.MovementStates.GroundedStates;
using Magia.Player.StateMachines.MovementStates.UncontrollableStates;
using Magia.Player.StateMachines.MovementStates.WallStates;
using Magia.Utilities.Tools;

namespace Magia.Player.StateMachines
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public PlayerController Player { get; }

        public PlayerHoverState HoverState { get; }
        public PlayerFlyState FlyState { get; }
        public PlayerRushState RushState { get; }
        public PlayerDashState DashState { get; }

        public PlayerIdleState IdleState { get; }
        public PlayerWalkState WalkState { get; }
        public PlayerRunState RunState { get; }
        public PlayerSprintState SprintState { get; }
        public PlayerJumpState JumpState { get; }

        public PlayerWallRunState WallRunState { get; }
        public PlayerWallSlideState WallSlideState { get; }

        public PlayerFallingState FallingState { get; }
        public PlayerStaggeredState StaggeredState { get; }
        public PlayerKnockbackState KnockbackState { get; }
        public PlayerPullupState PullupState { get; }

        public PlayerMovementStateMachine(PlayerController playerController)
        {
            Player = playerController;

            HoverState = new PlayerHoverState(this);
            FlyState = new PlayerFlyState(this);
            RushState = new PlayerRushState(this);
            DashState = new PlayerDashState(this);

            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            RunState = new PlayerRunState(this);
            SprintState = new PlayerSprintState(this);
            JumpState = new PlayerJumpState(this);

            WallRunState = new PlayerWallRunState(this);
            WallSlideState = new PlayerWallSlideState(this);

            FallingState = new PlayerFallingState(this);
            StaggeredState = new PlayerStaggeredState(this);
            KnockbackState = new PlayerKnockbackState(this);
            PullupState = new PlayerPullupState(this);
        }

        public override void ChangeState(IState newState)
        {
            base.ChangeState(newState);

            // DebugVisualizer.Instance.UpdateLog("MOVEMENT STATE", $"{newState.GetType().Name}");
        }

        public void StopRush()
        {
            if (Player.ReusableData.IsGrounded)
            {
                ChangeState(WalkState);
                return;
            }

            ChangeState(FlyState);
        }
    }
}
