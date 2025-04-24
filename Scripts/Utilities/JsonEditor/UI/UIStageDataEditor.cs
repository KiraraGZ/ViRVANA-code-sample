using System;
using System.Collections.Generic;
using Magia.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditor : MonoBehaviour
    {
        public event Action EventAddStageButtonClicked;
        public event Action EventRemoveStageButtonClicked;
        public event Action EventShiftNextStageButtonClicked;
        public event Action EventShiftPrevStageButtonClicked;
        public event Action EventAddModeButtonClicked;
        public event Action EventRemoveModeButtonClicked;

        public event Action<int> EventSelectStage;
        public event Action<int> EventSelectMode;

        public event Action EventRefreshButtonClicked;
        public event Action EventSaveButtonClicked;

        public event Action<StageSettingData> EventStageSettingChanged;
        public event Action<int, int> EventRewardChanged;
        public event Action<int> EventObjectiveModeChanged;
        public event Action<ObjectiveData> EventObjectiveChanged;

        public event Action EventAddBossButtonClicked;
        public event Action<int, BossSpawnData> EventUpdateBoss;
        public event Action<int> EventRemoveBoss;

        public event Action EventAddEnemyButtonClicked;
        public event Action<int, EnemySpawnData> EventUpdateEnemy;
        public event Action<int> EventRemoveEnemy;

        public event Action EventDialogueButtonClicked;
        public event Action<DialogueCollection> EventDialogueChanged;

        [Header("Top Left Button")]
        [SerializeField] private Button addStageButton;
        [SerializeField] private Button removeStageButton;
        [SerializeField] private Button shiftNextStageButton;
        [SerializeField] private Button shiftPrevStageButton;
        [SerializeField] private Button addModeButton;
        [SerializeField] private Button removeModeButton;
        [SerializeField] private Button dialogueButton;

        [Header("Top Right")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button addEnemyButton;

        [Header("Setting")]
        [SerializeField] private UIStageDataEditorSetting setting;

        [Header("Reward")]
        [SerializeField] private UIStageDataEditorReward[] expRewardInput;
        [SerializeField] private UIStageDataEditorReward firstTimeRewardInput;

        [Header("Stage")]
        [SerializeField] private UIStageDataEditorStage stagePrefab;
        private List<UIStageDataEditorStage> stageList;
        private List<UIStageDataEditorStage> modeList;
        [SerializeField] private RectTransform stageContainer;
        [SerializeField] private RectTransform modeContainer;
        [SerializeField] private Color selectColor;
        [SerializeField] private Color deselectColor;

        [Header("Objective")]
        [SerializeField] private UIStageDataEditorObjective objective;

        [Header("Spawn")]
        [SerializeField] private UIStageDataEditorEnemy enemyPrefab;
        private List<UIStageDataEditorEnemy> enemyList;
        [SerializeField] private RectTransform enemyContainer;

        [Header("Dialogue")]
        [SerializeField] private UIStageDataEditorDialoguePanel dialoguePanel;

        public void Initialize()
        {
            enemyList = new();
            stageList = new();
            modeList = new();

            setting.Initialize();
            objective.Initialize();

            AddListener();
        }

        public void Dispose()
        {
            enemyList = null;
            stageList = null;
            modeList = null;

            setting.Dispose();
            objective.Dispose();

            RemoveListener();
        }

        private void AddListener()
        {
            addStageButton.onClick.AddListener(OnAddStageButtonClicked);
            removeStageButton.onClick.AddListener(OnRemoveStageButtonClicked);
            shiftNextStageButton.onClick.AddListener(OnShiftNextStageButtonClicked);
            shiftPrevStageButton.onClick.AddListener(OnShiftPrevStageButtonClicked);
            addModeButton.onClick.AddListener(OnAddModeButtonClicked);
            removeModeButton.onClick.AddListener(OnRemoveModeButtonClicked);
            dialogueButton.onClick.AddListener(OnDialogueButtonClicked);

            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            saveButton.onClick.AddListener(OnSaveButtonClicked);

            addEnemyButton.onClick.AddListener(OnAddEnemyButtonClicked);
            setting.EventStageSettingChanged += OnStageSettingChanged;
            objective.EventObjectiveModeChanged += OnObjectiveModeChanged;
            // objective.EventObjectiveChanged += OnObjectiveChanged;
            objective.EventAddBossButtonClicked += OnAddBossButtonClicked;
            objective.EventUpdateBoss += OnUpdateBoss;
            objective.EventRemoveBoss += OnRemoveBoss;

            firstTimeRewardInput.Initialize(-1);
            firstTimeRewardInput.EventRewardChanged += OnRewardChanged;

            dialoguePanel.EventDialogueChanged += OnDialogueChanged;

            for (int i = 0; i < expRewardInput.Length; i++)
            {
                expRewardInput[i].Initialize(i);
                expRewardInput[i].EventRewardChanged += OnRewardChanged;
            }
        }

        //TODO: copy all the listeners from above, when this script need a dispose method.
        private void RemoveListener()
        {
            refreshButton.onClick.RemoveAllListeners();
            saveButton.onClick.RemoveAllListeners();

            setting.EventStageSettingChanged -= OnStageSettingChanged;

            addEnemyButton.onClick.RemoveAllListeners();
        }

        #region visualize stage collection
        public void UpdateStageList(int amount, int index)
        {
            SetupNode(stageList, stageContainer, amount, index, OnSelectStage);
        }

        public void UpdateModeList(int amount, int index)
        {
            SetupNode(modeList, modeContainer, amount, index, OnSelectMode);
        }

        private void SetupNode(List<UIStageDataEditorStage> list, RectTransform container, int amount, int index, Action<int> action)
        {
            for (int i = list.Count; i < amount; i++)
            {
                var stage = stagePrefab.Rent(container);
                stage.Initialize(i);

                stage.EventSelect += action;
                list.Add(stage);
            }

            if (amount < list.Count)
            {
                for (int i = amount; i < list.Count; i++)
                {
                    list[i].EventSelect -= action;
                    list[i].Dispose();
                }

                list.RemoveRange(amount, list.Count - amount);
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (i == index)
                {
                    list[i].SelectColor(selectColor);
                }
                else
                {
                    list[i].SelectColor(deselectColor);
                }
            }
        }

        // public void UpdateCurrentStage(StageModeData modeData)
        // {
        //     setting.UpdateInfo(modeData.Setting);
        //     UpdateStageInfo(modeData);
        // }
        #endregion

        #region visualize stage mode
        // public void UpdateStageInfo(StageModeData data)
        // {
        //     UpdateEnemySpawnInfo(data.SpawnDatas);
        //     UpdateRewardInfo(data);
        //     UpdateObjectiveInfo(data.Objective);
        // }

        private void UpdateEnemySpawnInfo(List<EnemySpawnData> spawnList)
        {
            for (int i = enemyList.Count; i < spawnList.Count; i++)
            {
                var enemy = enemyPrefab.Rent(enemyContainer);
                enemy.Initialize(i);
                enemy.EventUpdate += OnUpdateEnemy;
                enemy.EventRemove += OnRemoveEnemy;
                enemyList.Add(enemy);
            }

            if (spawnList.Count < enemyList.Count)
            {
                for (int i = spawnList.Count; i < enemyList.Count; i++)
                {
                    enemyList[i].Dispose();
                }

                enemyList.RemoveRange(spawnList.Count, enemyList.Count - spawnList.Count);
            }

            for (int i = 0; i < spawnList.Count; i++)
            {
                enemyList[i].UpdateInfo(spawnList[i]);
            }
        }

        // private void UpdateRewardInfo(StageModeData data)
        // {
        //     firstTimeRewardInput.UpdateInfo(data.FirstTimeReward);

        //     for (int i = 0; i < data.ExperienceRewards.Length; i++)
        //     {
        //         expRewardInput[i].UpdateInfo(data.ExperienceRewards[i]);
        //     }
        // }

        public void UpdateObjectiveInfo(ObjectiveData data)
        {
            objective.UpdateInfo(data);
        }

        public void UpdateDialogueInfo(DialogueCollection dialogues)
        {
            dialoguePanel.UpdateInfo(dialogues);
        }
        #endregion

        #region subscribe events
        private void OnAddStageButtonClicked()
        {
            EventAddStageButtonClicked?.Invoke();
        }

        private void OnRemoveStageButtonClicked()
        {
            EventRemoveStageButtonClicked?.Invoke();
        }

        private void OnShiftNextStageButtonClicked()
        {
            EventShiftNextStageButtonClicked?.Invoke();
        }

        private void OnShiftPrevStageButtonClicked()
        {
            EventShiftPrevStageButtonClicked?.Invoke();
        }

        private void OnAddModeButtonClicked()
        {
            EventAddModeButtonClicked?.Invoke();
        }

        private void OnRemoveModeButtonClicked()
        {
            EventRemoveModeButtonClicked?.Invoke();
        }

        private void OnRefreshButtonClicked()
        {
            EventRefreshButtonClicked?.Invoke();
        }

        private void OnSaveButtonClicked()
        {
            EventSaveButtonClicked?.Invoke();
        }

        private void OnSelectStage(int index)
        {
            EventSelectStage?.Invoke(index);
        }

        private void OnSelectMode(int index)
        {
            EventSelectMode?.Invoke(index);
        }

        private void OnStageSettingChanged(StageSettingData data)
        {
            EventStageSettingChanged?.Invoke(data);
        }

        private void OnRewardChanged(int index, int amount)
        {
            EventRewardChanged?.Invoke(index, amount);
        }

        private void OnObjectiveModeChanged(int index)
        {
            EventObjectiveModeChanged?.Invoke(index);
        }

        private void OnObjectiveChanged(ObjectiveData data)
        {
            EventObjectiveChanged?.Invoke(data);
        }

        private void OnAddEnemyButtonClicked()
        {
            EventAddEnemyButtonClicked?.Invoke();
        }

        private void OnUpdateEnemy(int index, EnemySpawnData data)
        {
            EventUpdateEnemy?.Invoke(index, data);
        }

        private void OnRemoveEnemy(int index)
        {
            EventRemoveEnemy?.Invoke(index);
        }

        private void OnAddBossButtonClicked()
        {
            EventAddBossButtonClicked?.Invoke();
        }

        private void OnUpdateBoss(int index, BossSpawnData data)
        {
            EventUpdateBoss?.Invoke(index, data);
        }

        private void OnRemoveBoss(int index)
        {
            EventRemoveBoss?.Invoke(index);
        }

        private void OnDialogueButtonClicked()
        {
            dialoguePanel.Initialize();
            EventDialogueButtonClicked?.Invoke();
        }

        private void OnDialogueChanged(DialogueCollection data)
        {
            EventDialogueChanged?.Invoke(data);
        }
        #endregion
    }
}
