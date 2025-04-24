using UnityEngine;

namespace Magia.Buddy.StateMachines.MovementStates
{
    public class BuddyGuideState : BuddyMovementState
    {
        public BuddyGuideState(BuddyMovementStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Vector3 distance = stateMachine.Buddy.ReusableData.Destination - stateMachine.Buddy.transform.position;
            float speedMultiplier = Mathf.Clamp01((distance.magnitude - movementData.SlowDownRadius * 0.5f) / movementData.SlowDownRadius);

            if (speedMultiplier <= 0.1f)
            {
                HoverInPlace();
                return;
            }

            MoveToward(stateMachine.Buddy.ReusableData.Destination, speedMultiplier);
        }
    }
}
