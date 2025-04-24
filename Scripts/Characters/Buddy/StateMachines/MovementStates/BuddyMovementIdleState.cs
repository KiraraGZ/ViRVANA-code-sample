using UnityEngine;
using Magia.StateMachines;

namespace Magia.Buddy.StateMachines.MovementStates
{
    public class BuddyMovementIdleState : IState
    {
        private BuddyMovementStateMachine stateMachine;
        private float limitDistance;

        public BuddyMovementIdleState(BuddyMovementStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            limitDistance = stateMachine.Buddy.Data.MovementData.StopDistance;
        }

        public void Enter()
        {
            stateMachine.Buddy.ReusableData.IsFollowingPlayer = false;
            stateMachine.Buddy.ReusableData.IsRepositioning = false;
        }

        public void Exit() { }

        public void HandleInput() { }

        public void Update()
        {
            if ((stateMachine.Buddy.Player.transform.position - stateMachine.Buddy.transform.position).magnitude > limitDistance)
            {
                stateMachine.ChangeState(stateMachine.FollowPlayerState);
            }
        }

        public void PhysicsUpdate()
        {
            MaintainPosition();
        }

        public void OnAnimationEnterEvent() { }

        public void OnAnimationEndEvent() { }

        public void OnAnimationTransitionEvent() { }

        private void MaintainPosition()
        {
            if (stateMachine.Buddy.Rigidbody.position.y - stateMachine.Buddy.Player.Rigidbody.position.y - stateMachine.Buddy.Data.MovementData.HoverHeight >= stateMachine.Buddy.Data.MovementData.HoverHeightOffset)
            {
                Vector3 targetPosition = new Vector3(stateMachine.Buddy.Rigidbody.transform.position.x, stateMachine.Buddy.Player.Rigidbody.transform.position.y + stateMachine.Buddy.Data.MovementData.HoverHeight, stateMachine.Buddy.Rigidbody.position.z);
                Vector3 updatedPosition = Vector3.Lerp(stateMachine.Buddy.Rigidbody.transform.position, stateMachine.Buddy.Player.Rigidbody.transform.position, stateMachine.Buddy.Data.MovementData.VerticalLerp * Time.deltaTime);
                stateMachine.Buddy.Rigidbody.position = updatedPosition;
            }
        }
    }
}