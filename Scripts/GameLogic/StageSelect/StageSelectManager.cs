using System;
using System.Collections;
using System.Collections.Generic;
using Magia.UI;
using Magia.Enemy;
using Magia.Utilities.Json;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Magia.Environment;

namespace Magia.GameLogic
{
    public class StageSelectManager : MonoBehaviour
    {
        public event Action<UIStageSelectPopupData> EventStageSelect;

        [SerializeField] private StageSelectScene scene;
        public StageSelectScene Scene => scene;

        #region stage data
        private StageData[] stageDatas;
        private StageStatus[] stageStatus;
        public int StageIndex { get; private set; }

        private StageMap stageMap;
        private StageMapData stageMapData;
        private Dictionary<string, EnemySO> enemyDict;

        public string jsonPath = "GameDatas/StageDatas";
        #endregion

        public void Initialize()
        {
            stageDatas = JsonHelper<StageCollection>.LoadData(jsonPath).StageDatas;

            scene.Initialize();

            AddListener();

            // CreateExpeditionNodes();
        }

        public void Dispose()
        {
            scene.Dispose();

            RemoveListener();
        }

        private void AddListener()
        {
            scene.EventStageNodeSelect += OnStageSelect;
        }

        private void RemoveListener()
        {
            scene.EventStageNodeSelect -= OnStageSelect;
        }

        #region story
        public void SetStoryMode()
        {
            stageStatus = new StageStatus[stageDatas.Length];
            Dictionary<string, StageStatus> stageStatusDict = new();

            for (int i = 0; i < stageDatas.Length; i++)
            {
                string mapName = stageDatas[i].MapName;
                int rating = PlayerPrefsHelper.GetStageRating(mapName);
                bool isUnlocked = rating > 0 || string.IsNullOrEmpty(stageDatas[i].RequiredMap);
                var status = new StageStatus(isUnlocked, rating);

                stageStatus[i] = status;
                stageStatusDict.Add(mapName, status);
            }

            for (int i = 0; i < stageDatas.Length; i++)
            {
                string mapName = stageDatas[i].MapName;
                string requiredMap = stageDatas[i].RequiredMap;

                if (stageStatusDict[mapName].IsUnlocked) continue;

                if (string.IsNullOrEmpty(requiredMap))
                {
                    stageStatus[i].IsUnlocked = true;
                    stageStatusDict[mapName].IsUnlocked = true;
                    continue;
                }

                stageStatusDict.TryGetValue(requiredMap, out StageStatus requiredMapStatus);

                if (requiredMapStatus.Rating > 0)
                {
                    stageStatus[i].IsUnlocked = true;
                    stageStatusDict[mapName].IsUnlocked = true;
                }
            }

            scene.DisplayStoryNode(stageStatus);
        }

        public void ClearScene()
        {
            scene.HideStoryNode();
        }
        #endregion

        #region on menu
        //TODO: Implement stage mode back later, once design is done.
        public void ChangeMode()
        {
            OnUpdateStageData();
        }
        #endregion

        #region on start stage
        public StageInitialData GetStageInitialData()
        {
            return new(stageDatas[StageIndex].MapName,
                       enemyDict,
                       stageMapData);
        }

        public string GetCurrentStageMapName()
        {
            return stageDatas[StageIndex].MapName;
        }

        public string GetCurrentStageMapId()
        {
            return stageMapData.Setting.StageId;
        }

        private void OnUpdateStageData()
        {
            StartCoroutine(LoadStageMap(() =>
            {
                stageMapData = stageMap.GetStageMapData();
                LoadEnemy();

                EventStageSelect?.Invoke(new UIStageSelectPopupData(
                    stageMapData.Setting.StageId,
                    stageMapData,
                    enemyDict,
                    new StageStatus(PlayerPrefsHelper.GetStageRating(stageDatas[StageIndex].MapName))));
            }));
        }

        private IEnumerator LoadStageMap(Action callback = null)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(stageDatas[StageIndex].MapName);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                stageMap = handle.Result.GetComponent<StageMap>();
            }

            callback?.Invoke();
        }

        private void LoadEnemy()
        {
            enemyDict = new();
            var enemyDatas = stageMap.GetSpawnerEnemyDatas();

            for (int i = 0; i < enemyDatas.Length; i++)
            {
                if (enemyDict.ContainsKey(enemyDatas[i].Name)) continue;

                enemyDict.Add(enemyDatas[i].Name, enemyDatas[i]);
            }
        }
        #endregion

        #region on stage end
        public CurrencyData GetReward(int rating)
        {
            int currentRating = PlayerPrefsHelper.GetStageRating(GetCurrentStageMapName());

            var softCurrency = currentRating < 3 && rating == 3 ?
                               stageMapData.FirstTimeReward : 0;

            return new CurrencyData(stageMapData.ExperienceRewards[rating - 1], softCurrency, 0);
        }

        public void RefreshUnlockStages()
        {
            for (int i = 0; i < stageDatas.Length; i++)
            {
                if (stageDatas[i].RequiredMap != stageDatas[StageIndex].RequiredMap) continue;

                stageStatus[i].IsUnlocked = true;
            }

            scene.DisplayStoryNode(stageStatus);
        }
        #endregion

        [ContextMenu("Unlock All Stages")]
        private void UnlockAllStages()
        {
            for (int i = 0; i < stageDatas.Length; i++)
            {
                stageStatus[i].IsUnlocked = true;

                if (PlayerPrefsHelper.GetStageRating(stageDatas[i].MapName) > 0) continue;

                stageStatus[i].Rating = 1;
                PlayerPrefsHelper.SetStageRating(stageDatas[i].MapName, 1);
            }

            scene.DisplayStoryNode(stageStatus);
        }

        #region subscribe events
        private void OnStageSelect(int index)
        {
            StageIndex = index;
            OnUpdateStageData();
        }
        #endregion
    }
}
