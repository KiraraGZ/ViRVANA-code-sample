using UnityEngine;
using Magia.GameLogic;
using System;

namespace Magia.Utilities.Json.Editor
{
    public class StageDataEditor : MonoBehaviour
    {
        [SerializeField] private StageDataJsonEditor jsonEditor;
        [SerializeField] private UIStageDataEditor ui;
        [SerializeField] private StageDataEditorVisual visual;

        private StageData StageData => jsonEditor.EditStageData;

        private int StageIndex => jsonEditor.StageIndex;
        private int ModeIndex;

        // private void Awake()
        // {
        //     jsonEditor.ReadJsonFile();
        //     jsonEditor.GetStageData();

        //     ui.Initialize();
        //     RefreshUI();

        //     visual.Initialize();
        //     RefreshVisual();

        //     AddListener();
        // }

        // private void AddListener()
        // {
        //     ui.EventAddStageButtonClicked += OnAddStage;
        //     ui.EventRemoveStageButtonClicked += OnRemoveStage;
        //     ui.EventShiftNextStageButtonClicked += OnShiftNextStage;
        //     ui.EventShiftPrevStageButtonClicked += OnShiftPrevStage;
        //     ui.EventAddModeButtonClicked += OnAddMode;
        //     ui.EventRemoveModeButtonClicked += OnRemoveMode;

        //     ui.EventSelectStage += OnSelectStage;
        //     ui.EventSelectMode += OnSelectMode;

        //     ui.EventRefreshButtonClicked += OnRefreshButtonClicked;
        //     ui.EventSaveButtonClicked += OnSaveButtonClicked;

        //     ui.EventStageSettingChanged += OnStageSettingChanged;
        //     ui.EventRewardChanged += OnRewardChanged;
        //     ui.EventObjectiveModeChanged += OnObjectiveModeChanged;
        //     ui.EventObjectiveChanged += OnObjectiveChanged;

        //     ui.EventAddEnemyButtonClicked += OnAddEnemyButtonClicked;
        //     ui.EventUpdateEnemy += OnUpdateEnemy;
        //     ui.EventRemoveEnemy += OnRemoveEnemy;

        //     ui.EventDialogueButtonClicked += OnDialogueButtonClicked;
        //     ui.EventDialogueChanged += OnDialogueChanged;

        //     ui.EventAddBossButtonClicked += OnAddBossButtonClicked;
        //     ui.EventUpdateBoss += OnUpdateBoss;
        //     ui.EventRemoveBoss += OnRemoveBoss;
        // }

        // #region private methods
        // private void RefreshUI()
        // {
        //     ui.UpdateStageList(jsonEditor.StageCollection.Count, StageIndex);
        //     ui.UpdateModeList(StageData.Modes.Count, ModeIndex);
        //     ui.UpdateCurrentStage(StageData.Modes[ModeIndex]);
        //     ui.UpdateStageInfo(StageData.Modes[ModeIndex]);
        //     ui.UpdateDialogueInfo(StageData.Modes[ModeIndex].Dialogues);
        // }

        // private void RefreshVisual()
        // {
        //     visual.TryLoadStageScene(StageData.Modes[ModeIndex].Setting.MapName);
        //     visual.ShowPlayerSpawn(StageData.Modes[ModeIndex].Setting);
        //     visual.ShowSpawnRadius(StageData.Modes[ModeIndex].SpawnDatas);
        // }

        // private void TryLoadStageScene(string name, Action callback = null)
        // {
        //     visual.TryLoadStageScene(name, () =>
        //     {
        //         callback.Invoke();
        //     });
        // }
        // #endregion

        // #region state events
        // private void OnAddStage()
        // {
        //     jsonEditor.InsertStageData();

        //     OnSelectStage(StageIndex);
        // }

        // private void OnRemoveStage()
        // {
        //     jsonEditor.RemoveStageData();

        //     OnSelectStage(StageIndex);
        // }

        // private void OnShiftNextStage()
        // {
        //     if (jsonEditor.StageIndex >= jsonEditor.StageCollection.Count - 1) return;

        //     jsonEditor.SwapStageData(StageIndex, StageIndex + 1);
        //     jsonEditor.StageIndex++;

        //     OnSelectStage(StageIndex);
        // }

        // private void OnShiftPrevStage()
        // {
        //     if (jsonEditor.StageIndex <= 0) return;

        //     jsonEditor.SwapStageData(StageIndex, StageIndex - 1);
        //     jsonEditor.StageIndex--;

        //     OnSelectStage(StageIndex);
        // }

        // private void OnAddMode()
        // {
        //     StageData.Modes.Add(new(StageData.Modes[ModeIndex]));

        //     ModeIndex++;

        //     OnSelectMode(ModeIndex);
        // }

        // private void OnRemoveMode()
        // {
        //     if (StageData.Modes.Count <= 1) return;

        //     StageData.Modes.RemoveAt(ModeIndex);

        //     if (ModeIndex >= StageData.Modes.Count)
        //     {
        //         ModeIndex = StageData.Modes.Count - 1;
        //     }

        //     OnSelectMode(ModeIndex);
        // }

        // private void OnSelectStage(int index)
        // {
        //     jsonEditor.StageIndex = index;
        //     jsonEditor.GetStageData();

        //     OnSelectMode(0);
        // }

        // private void OnSelectMode(int index)
        // {
        //     ModeIndex = index;

        //     RefreshUI();
        //     RefreshVisual();
        // }
        // #endregion

        // #region file config events
        // private void OnRefreshButtonClicked()
        // {
        //     jsonEditor.ReadJsonFile();
        //     jsonEditor.GetStageData();

        //     RefreshUI();
        //     RefreshVisual();
        // }

        // private void OnSaveButtonClicked()
        // {
        //     jsonEditor.SaveStageData();
        // }
        // #endregion

        // #region data change events
        // private void OnStageSettingChanged(StageSettingData data)
        // {
        //     TryLoadStageScene(data.MapName, () =>
        //     {
        //         StageData.Modes[ModeIndex].Setting = data;

        //         RefreshVisual();

        //         jsonEditor.SaveStageData();
        //     });
        // }

        // private void OnRewardChanged(int index, int amount)
        // {
        //     if (index == -1)
        //     {
        //         StageData.Modes[ModeIndex].FirstTimeReward = amount;
        //         return;
        //     }

        //     StageData.Modes[ModeIndex].ExperienceRewards[index] = amount;
        //     jsonEditor.SaveStageData();
        // }

        // private void OnObjectiveModeChanged(int index)
        // {
        //     switch (index)
        //     {
        //         case 0:
        //             StageData.Modes[ModeIndex].Objective.Mode = ObjectiveData.ObjectiveMode.BOSS;
        //             break;

        //         case 1:
        //             StageData.Modes[ModeIndex].Objective.Mode = ObjectiveData.ObjectiveMode.HUNT;
        //             break;

        //         default:
        //             break;
        //     }

        //     jsonEditor.SaveStageData();

        //     ui.UpdateObjectiveInfo(StageData.Modes[ModeIndex].Objective);
        // }

        // private void OnObjectiveChanged(ObjectiveData data)
        // {
        //     StageData.Modes[ModeIndex].Objective = new(data);
        //     jsonEditor.SaveStageData();
        // }

        // private void OnAddEnemyButtonClicked()
        // {
        //     StageData.Modes[ModeIndex].SpawnDatas.Add(new());

        //     ui.UpdateStageInfo(StageData.Modes[ModeIndex]);
        // }

        // private void OnUpdateEnemy(int index, EnemySpawnData data)
        // {
        //     StageData.Modes[ModeIndex].SpawnDatas[index] = data;
        //     jsonEditor.SaveStageData();

        //     visual.ShowSpawnRadius(StageData.Modes[ModeIndex].SpawnDatas);
        // }

        // private void OnRemoveEnemy(int index)
        // {
        //     StageData.Modes[ModeIndex].SpawnDatas.RemoveAt(index);
        //     jsonEditor.SaveStageData();

        //     ui.UpdateStageInfo(StageData.Modes[ModeIndex]);
        // }

        // private void OnAddBossButtonClicked()
        // {
        //     StageData.Modes[ModeIndex].Objective.Boss.Bosses.Add(new());

        //     ui.UpdateStageInfo(StageData.Modes[ModeIndex]);
        // }

        // private void OnUpdateBoss(int index, BossSpawnData data)
        // {
        //     StageData.Modes[ModeIndex].Objective.Boss.Bosses[index] = data;
        //     jsonEditor.SaveStageData();
        // }

        // private void OnRemoveBoss(int index)
        // {
        //     StageData.Modes[ModeIndex].Objective.Boss.Bosses.RemoveAt(index);
        //     jsonEditor.SaveStageData();

        //     ui.UpdateStageInfo(StageData.Modes[ModeIndex]);
        // }

        // private void OnDialogueButtonClicked()
        // {
        //     ui.UpdateDialogueInfo(StageData.Modes[ModeIndex].Dialogues);
        // }

        // private void OnDialogueChanged(DialogueCollection dialogues)
        // {
        //     StageData.Modes[ModeIndex].Dialogues = new(dialogues);
        //     jsonEditor.SaveStageData();
        // }
        // #endregion
    }
}