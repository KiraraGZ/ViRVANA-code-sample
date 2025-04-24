using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.CombatStates
{
    public class PlayerIdleState : PlayerCombatState
    {
        public PlayerIdleState(PlayerCombatStateMachine playerCombatStateMachine) : base(playerCombatStateMachine)
        {
            stateMachine = playerCombatStateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            AddInputActionCallbacks();
        }

        public override void Exit()
        {
            base.Exit();

            RemoveInputActionCallbacks();
        }

        #region reusable methods
        private void AddInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Attack.started += OnAttackStarted;
            stateMachine.Player.Input.PlayerActions.PrimarySkill.started += OnPrimarySkillPerformed;
            stateMachine.Player.Input.PlayerActions.SecondarySkill.started += OnSecondarySkillPerformed;
            stateMachine.Player.Input.PlayerActions.UltimateSkill.started += OnUltimateSkillPerformed;
            // stateMachine.Player.Input.PlayerActions.SupportSkill.started += OnSupportSkillPerformed;
            stateMachine.Player.Input.PlayerActions.Switch.performed += OnSkillSwitch;
        }

        private void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Attack.started -= OnAttackStarted;
            stateMachine.Player.Input.PlayerActions.PrimarySkill.started -= OnPrimarySkillPerformed;
            stateMachine.Player.Input.PlayerActions.SecondarySkill.started -= OnSecondarySkillPerformed;
            stateMachine.Player.Input.PlayerActions.UltimateSkill.started -= OnUltimateSkillPerformed;
            // stateMachine.Player.Input.PlayerActions.SupportSkill.started -= OnSupportSkillPerformed;
            stateMachine.Player.Input.PlayerActions.Switch.performed -= OnSkillSwitch;
        }
        #endregion

        #region input methods
        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            if (stateMachine.Player.ReusableData.EnableAttack == false) return;

            stateMachine.ChangeState(stateMachine.AttackState);
        }

        private void OnPrimarySkillPerformed(InputAction.CallbackContext context)
        {
            stateMachine.Player.ReusableData.CurrentSkillIndex = 0;

            if (!stateMachine.Player.SkillHandler.IsSkillAvailable(stateMachine.Player.ReusableData.CurrentSkillIndex)) return;

            stateMachine.ChangeState(stateMachine.SkillPerformedState);
        }

        private void OnSecondarySkillPerformed(InputAction.CallbackContext context)
        {
            stateMachine.Player.ReusableData.CurrentSkillIndex = 1;

            if (!stateMachine.Player.SkillHandler.IsSkillAvailable(stateMachine.Player.ReusableData.CurrentSkillIndex)) return;

            stateMachine.ChangeState(stateMachine.SkillPerformedState);
        }

        private void OnUltimateSkillPerformed(InputAction.CallbackContext context)
        {
            stateMachine.Player.ReusableData.CurrentSkillIndex = 2;

            if (!stateMachine.Player.SkillHandler.IsSkillAvailable(stateMachine.Player.ReusableData.CurrentSkillIndex)) return;

            stateMachine.ChangeState(stateMachine.SkillPerformedState);
        }

        private void OnSupportSkillPerformed(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.SupportSkillPerformedState);
        }

        private void OnSkillSwitch(InputAction.CallbackContext context)
        {
            stateMachine.Player.SkillHandler.SwitchSkill();
        }
        #endregion
    }
}