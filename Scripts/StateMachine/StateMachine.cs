namespace Magia.StateMachines
{
    public abstract class StateMachine
    {
        protected IState currentState;

        public virtual void ChangeState(IState newState)
        {
            currentState?.Exit();

            currentState = newState;

            currentState.Enter();
        }

        public void Dispose()
        {
            currentState?.Exit();

            currentState = null;
        }

        public void HandleInput()
        {
            currentState?.HandleInput();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void PhysicsUpdate()
        {
            currentState?.PhysicsUpdate();
        }

        public void OnAnimationEnterEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }

        public void OnAnimationEndEvent()
        {
            currentState?.OnAnimationEndEvent();
        }

        public void OnAnimationTransitionEvent()
        {
            currentState?.OnAnimationTransitionEvent();
        }
    }
}
