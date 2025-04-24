using Magia.GameLogic;
using Magia.Player;
using Magia.Skills;
using Magia.UI.Gameplay;

namespace Magia.Environment.Tutorial
{
    public class TutorialUltimateSkill : TutorialSequence
    {
        private const int ATTACK_COUNT = 8;

        private bool ultimateSkill;
        private int attackCount;
        private bool equipSkillSet;

        private int currentSkillIndex;

        private GameplayController gameplayController;
        private SkillHandler skillHandler;

        public override void Initialize(PlayerController player)
        {
            base.Initialize(player);

            gameplayController = GameplayController.Instance;
            skillHandler = player.SkillHandler;

            skillHandler.PauseSwitchSkillSet();
            player.RecoverSkills();

            skillHandler.EventSkillPerformed += OnSkillPerformed;
            skillHandler.EventSkillEnd += OnSkillEnd;
            skillHandler.EventAttackComboPerformed += OnAttackComboPerformed;
            skillHandler.EventSkillSetEquipped += OnSkillSetEquipped;

            var datas = new UIObjectiveInfoData[]
            {
                new("Objective_Tutorial_Info_Ultimate_Skill", UIObjectiveType.CHECK),
                new("Objective_Tutorial_Info_Ultimate_Passive_Attack", UIObjectiveType.PROGRESS),
                new("Objective_Tutorial_Info_Switch", UIObjectiveType.CHECK),
            };
            gameplayController.InitializeTutorialObjective("Objective_Tutorial_Ultimate", datas);
        }

        public override void Dispose()
        {
            skillHandler.EventSkillPerformed -= OnSkillPerformed;
            skillHandler.EventSkillEnd -= OnSkillEnd;
            skillHandler.EventAttackComboPerformed -= OnAttackComboPerformed;
            skillHandler.EventSkillSetEquipped -= OnSkillSetEquipped;
        }

        private void UpdateObjective()
        {
            if (ultimateSkill && attackCount >= ATTACK_COUNT && equipSkillSet)
            {
                OnObjectiveProgress();
            }

            var datas = new (int, int)[]
            {
                new(ultimateSkill ? 1 : 0, 1),
                new(attackCount, ATTACK_COUNT),
                new(equipSkillSet ? 1 : 0, 1),
            };
            gameplayController.UpdateTutorialObjective(datas);
        }

        #region subscribe events
        private void OnSkillPerformed(int index)
        {
            currentSkillIndex = index;
        }

        private void OnSkillEnd()
        {
            if (ultimateSkill == true) return;

            if (currentSkillIndex == 2)
            {
                ultimateSkill = true;
            }

            UpdateObjective();
        }

        private void OnAttackComboPerformed(int comboNumber)
        {
            if (attackCount >= ATTACK_COUNT) return;
            if (ultimateSkill == false) return;

            attackCount++;

            if (attackCount == ATTACK_COUNT)
            {
                skillHandler.UnpauseSwitchSkillSet(true);
            }

            UpdateObjective();
        }

        private void OnSkillSetEquipped()
        {
            if (equipSkillSet == true) return;

            equipSkillSet = true;
            UpdateObjective();
        }
        #endregion
    }
}
