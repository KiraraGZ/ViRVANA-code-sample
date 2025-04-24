using Magia.GameLogic;
using Magia.Player;
using Magia.UI.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Environment.Tutorial
{
    public class TutorialMove : TutorialSequence
    {
        private bool isMove;
        private bool isLook;

        private GameplayController gameplayController;

        public override void Initialize(PlayerController player)
        {
            base.Initialize(player);

            gameplayController = GameplayController.Instance;

            actions.Move.started += OnPlayerMoveStarted;
            actions.Look.performed += OnPlayerLookPerformed;

            var datas = new UIObjectiveInfoData[]
            {
                new("Objective_Tutorial_Info_Move", UIObjectiveType.CHECK),
                new("Objective_Tutorial_Info_Look", UIObjectiveType.CHECK),
            };
            gameplayController.InitializeTutorialObjective("Objective_Tutorial_Movement_1", datas);
        }

        public override void Dispose()
        {
            actions.Move.started -= OnPlayerMoveStarted;
            actions.Look.performed -= OnPlayerLookPerformed;
        }

        private void UpdateObjective()
        {
            if (isMove && isLook)
            {
                OnObjectiveProgress();
            }

            var datas = new (int, int)[]
            {
                new(isMove ? 1 : 0, 1),
                new(isLook ? 1 : 0, 1),
            };
            gameplayController.UpdateTutorialObjective(datas);
        }

        #region subscribe events
        private void OnPlayerMoveStarted(InputAction.CallbackContext context)
        {
            if (isMove == true) return;

            isMove = true;
            UpdateObjective();
        }

        private void OnPlayerLookPerformed(InputAction.CallbackContext context)
        {
            if (isLook == true) return;
            if (context.ReadValue<Vector2>() == Vector2.zero) return;

            isLook = true;
            UpdateObjective();
        }
        #endregion
    }
}
