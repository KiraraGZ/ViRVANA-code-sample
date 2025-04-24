using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.Utilities;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using ObjectiveMode = Magia.GameLogic.ObjectiveData.ObjectiveMode;

namespace Magia.UI.Gameplay
{
    public class UIGameplayObjective : MonoBehaviour
    {
        private const string huntKey = "Objective_Hunt";
        private const string bossKey = "Objective_Boss";
        private const string destinationKey = "Objective_Destination";

        [SerializeField] private UIGameplayBoss bossPrefab;
        [SerializeField] private UIGameplayObjectiveCapture captureObjective;
        [SerializeField] private RectTransform topContainer;

        [SerializeField] private Image[] ratings;
        [SerializeField] private UIGameplayObjectiveBar currentObjectiveBar;

        private UIGameplayBoss[] bosses;
        private ObjectiveData[] objectives;
        private StringTable localizedTable;

        private int currentBossIndex;
        private bool isHide;

        public void Initialize(ObjectiveData[] _objectives, StringTable _localizedTable, Dictionary<string, EnemySO> enemyDict)
        {
            objectives = _objectives;
            localizedTable = _localizedTable;

            captureObjective.Initialize();

            currentObjectiveBar.gameObject.SetActive(!isHide);
            UpdateRating(0);
            UpdateObjectiveBar(ref currentObjectiveBar, objectives[0]);
        }

        public void Dispose()
        {
            objectives = null;
            localizedTable = null;

            if (bosses != null)
            {
                foreach (var boss in bosses)
                {
                    boss.Dispose();
                }

                bosses = null;
            }

            captureObjective.Dispose();
            currentObjectiveBar.Dispose();
        }

        public void UpdateObjective(ObjectiveInfoData data)
        {
            isHide = false;
            currentObjectiveBar.gameObject.SetActive(true);

            UpdateRating(data.Rating);
            UpdateObjectiveBar(ref currentObjectiveBar, objectives[data.Index]);

            currentObjectiveBar.UpdateProgress(data.Progress, data.TargetNumber);
        }

        private void UpdateRating(int number)
        {
            for (int i = 0; i < ratings.Length; i++)
            {
                ratings[i].enabled = i < number;
            }
        }

        private void UpdateObjectiveBar(ref UIGameplayObjectiveBar objectiveBar, ObjectiveData objective)
        {
            switch (objective.Mode)
            {
                case ObjectiveMode.BOSS:
                    {
                        string bossName = objective.Boss.Bosses[0].Name;
                        var description = LocalizationHelper.GetLocalizedString(localizedTable, bossKey, bossName);
                        objectiveBar.Initialize(description);
                        break;
                    }
                case ObjectiveMode.HUNT:
                    {
                        string enemyName = "enemies";
                        int amount = objective.Target;
                        var description = LocalizationHelper.GetLocalizedString(localizedTable, huntKey, enemyName, amount);
                        var data = new UIObjectiveInfoData[] { new(description, UIObjectiveType.PROGRESS) };
                        objectiveBar.Initialize("", data);
                        break;
                    }
                case ObjectiveMode.DESTINATION:
                    {
                        var description = LocalizationHelper.GetLocalizedString(localizedTable, destinationKey);
                        objectiveBar.Initialize(description);
                        break;
                    }
                case ObjectiveMode.DEMOLISH:
                    {
                        int amount = objective.Target;
                        var description = $"Destroy {amount} Hives";
                        var data = new UIObjectiveInfoData[] { new(description, UIObjectiveType.PROGRESS) };
                        objectiveBar.Initialize("", data);
                        break;
                    }
                case ObjectiveMode.CAPTURE:
                    {
                        var description = "Capture the Point";
                        var data = new UIObjectiveInfoData[] { new(description, UIObjectiveType.PROGRESS) };
                        objectiveBar.Initialize("", data);
                        break;
                    }
                default:
                    break;
            }
        }

        #region tutorials
        public void InitializeTutorialObjective(string name, UIObjectiveInfoData[] datas)
        {
            if (datas == null)
            {
                isHide = true;
                currentObjectiveBar.gameObject.SetActive(false);
                return;
            }

            isHide = false;

            for (int i = 0; i < datas.Length; i++)
            {
                string description = LocalizationHelper.GetLocalizedString(localizedTable, datas[i].Description);
                datas[i].Description = description;
            }

            string objectiveName = LocalizationHelper.GetLocalizedString(localizedTable, name);
            currentObjectiveBar.Initialize(objectiveName, datas);
            currentObjectiveBar.gameObject.SetActive(true);
        }

        public void UpdateTutorialObjective((int, int)[] checklist)
        {
            currentObjectiveBar.UpdateCheckList(checklist);
        }
        #endregion

        #region boss
        public void SetBossUI(EnemySO[] datas, BaseEnemy[] enemies)
        {
            if (bosses != null)
            {
                foreach (var boss in bosses)
                {
                    boss.Dispose();
                }
            }
            currentBossIndex = 0;
            bosses = new UIGameplayBoss[datas.Length];

            for (int i = 0; i != datas.Length; i++)
            {
                bosses[i] = bossPrefab.Rent(topContainer);
                bosses[i].Initialize(datas[i], enemies[i]);
            }
        }

        public void ShowBossBar()
        {
            if (currentBossIndex >= bosses.Length) return;

            bosses[currentBossIndex].Show();
            currentBossIndex++;
        }

        #endregion

        #region capture
        public void DisplayCaptureProgress(int mode, float progress, float target)
        {
            captureObjective.DisplayProgress(mode, progress, target);
        }
        #endregion
    }
}
