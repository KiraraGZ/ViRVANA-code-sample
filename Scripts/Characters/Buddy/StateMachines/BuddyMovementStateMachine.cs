using Magia.Buddy.StateMachines.MovementStates;
using Magia.StateMachines;
using UnityEngine;

namespace Magia.Buddy
{
    public class BuddyMovementStateMachine : StateMachine
    {
        public BuddyController Buddy;

        public BuddyMovementIdleState IdleState { get; }
        public BuddyFollowPlayerState FollowPlayerState { get; }
        public BuddyRepositionState RepositionState { get; }
        public BuddyGuideState GuideState { get; }

        public BuddyMovementStateMachine(BuddyController buddy)
        {
            Buddy = buddy;
            IdleState = new BuddyMovementIdleState(this);
            FollowPlayerState = new BuddyFollowPlayerState(this);
            RepositionState = new BuddyRepositionState(this);
            GuideState = new BuddyGuideState(this);
        }

        public void Move(float lerp)
        {
            Vector3 updatedPosition = Vector3.Lerp(Buddy.Rigidbody.transform.position, Buddy.Player.Rigidbody.transform.position, lerp * Time.deltaTime);
            Buddy.Rigidbody.position = updatedPosition;
        }
    }
}