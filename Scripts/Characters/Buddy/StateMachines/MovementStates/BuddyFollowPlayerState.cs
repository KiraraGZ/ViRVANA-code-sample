using Magia.Buddy;
using Magia.StateMachines;
using UnityEngine;

namespace Magia.Buddy.StateMachines.MovementStates
{
    public class BuddyFollowPlayerState : IState
    {
        private BuddyMovementStateMachine stateMachine;
        private BuddyController buddy;

        private float targetDistance;

        public BuddyFollowPlayerState(BuddyMovementStateMachine movementStateMachine)
        {
            stateMachine = movementStateMachine;
            buddy = stateMachine.Buddy;
        }

        public void Enter()
        {
            targetDistance = stateMachine.Buddy.Data.MovementData.StopDistance;
        }

        public void Exit()
        {

        }

        public void HandleInput() { }

        public void Update()
        {
            if ((buddy.transform.position - buddy.Player.transform.position).magnitude <= targetDistance)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }

        public void PhysicsUpdate()
        {
            FollowPlayer();
        }

        private void FollowPlayer()
        {
            stateMachine.Move(stateMachine.Buddy.Data.MovementData.FollowPlayerLerp);
        }

        public void OnAnimationEnterEvent() { }
        public void OnAnimationEndEvent() { }
        public void OnAnimationTransitionEvent() { }
    }
}