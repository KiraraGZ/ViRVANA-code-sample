using Magia.StateMachines;

namespace Magia.Buddy.StateMachines.CombatState
{
    public class BuddyIdleState : IState
    {
        private BuddyCombatStateMachine stateMachine;

        public BuddyIdleState(BuddyCombatStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {

        }

        public void Exit()
        {

        }

        public void HandleInput()
        {

        }

        public void OnAnimationEndEvent()
        {

        }

        public void OnAnimationEnterEvent()
        {

        }

        public void OnAnimationTransitionEvent()
        {

        }

        public void Update()
        {

        }

        public void PhysicsUpdate()
        {

        }
    }
}
