using Magia.GameLogic;
using Magia.Player;
using Magia.Skills;
using Magia.UI.Gameplay;

namespace Magia.Environment.Tutorial
{
    public class TutorialAttack : TutorialSequence
    {
        private const int ATTACK_COUNT = 12;
        private const int FORTH_ATTACK_COUNT = 3;

        private int attackCount;
        private int forthAttackCount;

        private GameplayController gameplayController;
        private SkillHandler skillHandler;

        public override void Initialize(PlayerController player)
        {
            base.Initialize(player);

            gameplayController = GameplayController.Instance;
            skillHandler = player.SkillHandler;

            skillHandler.EventAttackComboPerformed += OnAttackComboPerformed;

            var datas = new UIObjectiveInfoData[]
            {
                new("Objective_Tutorial_Info_Attack", UIObjectiveType.PROGRESS),
                new("Objective_Tutorial_Info_Forth_Attack", UIObjectiveType.PROGRESS),
            };
            gameplayController.InitializeTutorialObjective("Objective_Tutorial_Attack", datas);
        }

        public override void Dispose()
        {
            skillHandler.EventAttackComboPerformed -= OnAttackComboPerformed;
        }

        private void UpdateObjective()
        {
            if (attackCount >= ATTACK_COUNT && forthAttackCount >= FORTH_ATTACK_COUNT)
            {
                OnObjectiveProgress();
            }

            var datas = new (int, int)[]
            {
                new(attackCount, ATTACK_COUNT),
                new(forthAttackCount, FORTH_ATTACK_COUNT),
            };
            gameplayController.UpdateTutorialObjective(datas);
        }

        #region subscribe events
        private void OnAttackComboPerformed(int comboNumber)
        {
            attackCount++;

            if (comboNumber == 0)
            {
                forthAttackCount++;
            }
            else
            {
                if (attackCount > ATTACK_COUNT) return;
            }

            UpdateObjective();
        }
        #endregion
    }
}
