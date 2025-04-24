using Magia.Skills;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.CombatStates
{
    public class PlayerAttackState : PlayerCombatState
    {
        private SkillHandler skillHandler;

        public PlayerAttackState(PlayerCombatStateMachine playerCombatStateMachine) : base(playerCombatStateMachine)
        {
            skillHandler = stateMachine.Player.SkillHandler;
        }

        public override void Enter()
        {
            AddInputActionCallbacks();

            NormalAttackCalled();
        }

        public override void Exit()
        {
            RemoveInputActionCallbacks();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        #region reusable methods
        protected virtual void AddInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Attack.canceled += OnAttackRelease;
            skillHandler.EventSkillEnd += OnAttackEnd;
        }

        protected virtual void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Attack.canceled -= OnAttackRelease;
            skillHandler.EventSkillEnd -= OnAttackEnd;
        }
        #endregion

        #region input methods
        private void OnAttackRelease(InputAction.CallbackContext context)
        {
            skillHandler.ReleaseAttack();
        }

        private void OnAttackEnd()
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

        private void NormalAttackCalled()
        {
            stateMachine.Player.SkillHandler.PerformAttack();
        }
        #endregion
    }
}