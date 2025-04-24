using System;
using Magia.Skills;
using UnityEngine.InputSystem;

namespace Magia.Player.StateMachines.CombatStates
{
    public class PlayerSkillPerformedState : PlayerCombatState
    {
        protected int currentSkillIdx;

        private SkillHandler skillHandler;

        public Action OnSkillActivate;

        public PlayerSkillPerformedState(PlayerCombatStateMachine playerCombatStateMachine) : base(playerCombatStateMachine)
        {
            skillHandler = stateMachine.Player.SkillHandler;
        }

        public override void Enter()
        {
            AddInputActionCallbacks();

            currentSkillIdx = stateMachine.Player.ReusableData.CurrentSkillIndex;
            stateMachine.Player.ReusableData.IsPerformingSkill = true;

            skillHandler.PerformSkill(currentSkillIdx);
        }

        public override void Exit()
        {
            RemoveInputActionCallbacks();

            stateMachine.Player.ReusableData.IsPerformingSkill = false;
        }

        #region subscribe events
        /// <summary>
        /// Being called when player perform more input during skill sequence.
        /// </summary>
        private void OnSkillRepeat(InputAction.CallbackContext context)
        {
            skillHandler.RepeatSkill(currentSkillIdx);
        }

        /// <summary>
        /// Being called when player release current holding input during skill sequence.
        /// </summary>
        //TODO: BUG - method not being called when player's holding a skill.
        private void OnSkillRelease(InputAction.CallbackContext context)
        {
            stateMachine.Player.AnimatorController.ReleaseSkill();
            skillHandler.ReleaseSkill(currentSkillIdx);
        }

        /// <summary>
        /// Being called when current skill sequence end.
        /// </summary>
        private void OnSkillEnd()
        {
            stateMachine.Player.AnimatorController.ReleaseSkill();
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        #endregion

        protected virtual void AddInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.SkillPerformed.performed += OnSkillRepeat;
            stateMachine.Player.Input.PlayerActions.SkillPerformed.canceled += OnSkillRelease;

            skillHandler.EventSkillEnd += OnSkillEnd;
        }

        protected virtual void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.SkillPerformed.performed -= OnSkillRepeat;
            stateMachine.Player.Input.PlayerActions.SkillPerformed.canceled -= OnSkillRelease;

            skillHandler.EventSkillEnd -= OnSkillEnd;
        }
    }
}