using Magia.GameLogic;
using Magia.Player;
using Magia.UI.Gameplay;
using UnityEngine.InputSystem;

namespace Magia.Environment.Tutorial
{
    public class TutorialLevitate : TutorialSequence
    {
        private bool isMove;
        private bool isRush;
        private bool hoverUp;
        private bool hoverDown;

        private GameplayController gameplayController;

        public override void Initialize(PlayerController player)
        {
            base.Initialize(player);

            gameplayController = GameplayController.Instance;

            actions.Move.started += OnMoveStarted;
            actions.Move.canceled += OnMoveCanceled;
            actions.Rush.started += OnRushStarted;
            actions.Jump.started += OnHoverUpStarted;
            actions.Crouch.started += OnHoverDownStarted;

            var datas = new UIObjectiveInfoData[]
            {
                new("Objective_Tutorial_Info_Rush", UIObjectiveType.CHECK),
                new("Objective_Tutorial_Info_Hover_Up", UIObjectiveType.CHECK),
                new("Objective_Tutorial_Info_Hover_Down", UIObjectiveType.CHECK),
            };
            gameplayController.InitializeTutorialObjective("Objective_Tutorial_Movement_2", datas);
        }

        public override void Dispose()
        {
            actions.Move.started -= OnMoveStarted;
            actions.Move.canceled -= OnMoveCanceled;
            actions.Rush.started -= OnRushStarted;
            actions.Jump.started -= OnHoverUpStarted;
            actions.Crouch.started -= OnHoverDownStarted;
        }

        private void UpdateObjective()
        {
            if (isRush && hoverUp && hoverDown)
            {
                OnObjectiveProgress();
            }

            var datas = new (int, int)[]
            {
                new(isRush ? 1 : 0, 1),
                new(hoverUp ? 1 : 0, 1),
                new(hoverDown ? 1 : 0, 1),
            };
            gameplayController.UpdateTutorialObjective(datas);
        }

        #region subscribe events
        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            isMove = true;
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            isMove = false;
        }

        private void OnRushStarted(InputAction.CallbackContext context)
        {
            if (isRush == true) return;
            if (isMove == false) return;

            isRush = true;
            UpdateObjective();
        }

        private void OnHoverUpStarted(InputAction.CallbackContext context)
        {
            if (hoverUp == true) return;

            hoverUp = true;
            UpdateObjective();
        }

        private void OnHoverDownStarted(InputAction.CallbackContext context)
        {
            if (hoverDown == true) return;

            hoverDown = true;
            UpdateObjective();
        }
        #endregion
    }
}
