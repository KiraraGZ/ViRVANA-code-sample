using Magia.StateMachines;
using Magia.Buddy.StateMachines.CombatState;

namespace Magia.Buddy
{
    public class BuddyCombatStateMachine : StateMachine
    {
        public BuddyController Buddy;

        public BuddyIdleState IdleState { get; }
        public BuddyFindingState FindingState { get; }
        public BuddyAttackState AttackState { get; }
        public BuddyBlockedState BlockedState { get; }

        public BuddyCombatStateMachine(BuddyController buddy)
        {
            Buddy = buddy;
            IdleState = new BuddyIdleState(this);
            FindingState = new BuddyFindingState(this);
            AttackState = new BuddyAttackState(this);
            BlockedState = new BuddyBlockedState(this);
        }

        public override void ChangeState(IState newState)
        {
            // Debug.Log($"Exit: {currentState?.GetType().Name} - Enter: {newState?.GetType().Name}");

            base.ChangeState(newState);
        }
    }
}