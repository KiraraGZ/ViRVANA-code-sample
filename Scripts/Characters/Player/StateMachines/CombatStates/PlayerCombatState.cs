using Magia.Player.Data;
using Magia.StateMachines;

namespace Magia.Player.StateMachines.CombatStates
{
    public class PlayerCombatState : IState
    {
        public PlayerCombatStateMachine stateMachine;

        protected PlayerCombatData combatData;

        public PlayerCombatState(PlayerCombatStateMachine playerCombatStateMachine)
        {
            stateMachine = playerCombatStateMachine;

            combatData = stateMachine.Player.Data.CombatData;
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
    }
}