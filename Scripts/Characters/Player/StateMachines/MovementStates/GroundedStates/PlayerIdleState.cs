using UnityEngine;

namespace Magia.Player.StateMachines.MovementStates.GroundedStates
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            SetMovementSpeedModifier(0);
            stateMachine.Player.SetEnableAttack(true);

            ResetVelocity();
        }

        public override void Update()
        {
            base.Update();

            if (stateMachine.Player.ReusableData.MovementInput == Vector2.zero) return;

            if (stateMachine.Player.ReusableData.IsRush)
            {
                stateMachine.ChangeState(stateMachine.RunState);
                return;
            }

            stateMachine.ChangeState(stateMachine.WalkState);
        }

        public override void PhysicsUpdate()
        {
            RotateTowardCameraDirection();

            base.PhysicsUpdate();
        }
    }
}
