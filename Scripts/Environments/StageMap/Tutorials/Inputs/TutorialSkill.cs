using Magia.GameLogic;
using Magia.Player;
using Magia.Skills;
using Magia.UI.Gameplay;

namespace Magia.Environment.Tutorial
{
    public class TutorialSkill : TutorialSequence
    {
        private bool dashSkill;
        private bool primarySkill;
        private bool secondarySkill;

        private int currentSkillIndex;

        private GameplayController gameplayController;
        private SkillHandler skillHandler;

        public override void Initialize(PlayerController player)
        {
            base.Initialize(player);

            gameplayController = GameplayController.Instance;
            skillHandler = player.SkillHandler;
            currentSkillIndex = -1;

            skillHandler.EventDashPerformed += OnDashPerformed;
            skillHandler.EventSkillPerformed += OnSkillPerformed;
            skillHandler.EventSkillEnd += OnSkillEnd;

            var datas = new UIObjectiveInfoData[]
            {
                new("Objective_Tutorial_Info_Dash", UIObjectiveType.CHECK),
                new("Objective_Tutorial_Info_First_Skill", UIObjectiveType.CHECK),
                new("Objective_Tutorial_Info_Second_Skill", UIObjectiveType.CHECK),
            };
            gameplayController.InitializeTutorialObjective("Objective_Tutorial_Skills", datas);
        }

        public override void Dispose()
        {
            skillHandler.EventDashPerformed -= OnDashPerformed;
            skillHandler.EventSkillPerformed -= OnSkillPerformed;
            skillHandler.EventSkillEnd -= OnSkillEnd;
        }

        private void UpdateObjective()
        {
            if (dashSkill && primarySkill && secondarySkill)
            {
                OnObjectiveProgress();
            }

            var datas = new (int, int)[]
            {
                new(dashSkill? 1 : 0, 1),
                new(primarySkill? 1 : 0, 1),
                new(secondarySkill? 1 : 0, 1),
            };
            gameplayController.UpdateTutorialObjective(datas);
        }

        #region subscribe events
        private void OnDashPerformed()
        {
            if (dashSkill == true) return;

            dashSkill = true;
            UpdateObjective();
        }

        private void OnSkillPerformed(int index)
        {
            currentSkillIndex = index;
        }

        private void OnSkillEnd()
        {
            switch (currentSkillIndex)
            {
                case 0:
                    if (primarySkill == true) return;
                    primarySkill = true;
                    break;

                case 1:
                    if (secondarySkill == true) return;
                    secondarySkill = true;
                    break;
            }

            UpdateObjective();
        }
        #endregion
    }
}
