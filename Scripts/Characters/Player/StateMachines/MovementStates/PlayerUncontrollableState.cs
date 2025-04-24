using Magia.Player.Data;
using Magia.StateMachines;

namespace Magia.Player.StateMachines.MovementStates
{
    public class PlayerUncontrollableState : IState
    {
        protected PlayerMovementStateMachine stateMachine;

        protected PlayerUncontrollableData uncontrollableData;

        public PlayerUncontrollableState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;

            uncontrollableData = stateMachine.Player.Data.UncontrollableData;
        }

        public virtual void Enter()
        {

        }

        public virtual void Exit()
        {

        }

        public virtual void HandleInput()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void PhysicsUpdate()
        {
            Brake();
        }

        public virtual void OnAnimationEnterEvent()
        {

        }

        public virtual void OnAnimationEndEvent()
        {

        }

        public virtual void OnAnimationTransitionEvent()
        {

        }

        #region main method
        private void Brake()
        {
            if (stateMachine.Player.Rigidbody.velocity.magnitude <= 0.01f) return;

            stateMachine.Player.Rigidbody.AddForce(-stateMachine.Player.Rigidbody.velocity / 2);
        }
        #endregion
    }
}
