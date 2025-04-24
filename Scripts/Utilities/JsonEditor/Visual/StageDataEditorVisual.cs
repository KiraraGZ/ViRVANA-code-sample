using System;
using System.Collections;
using System.Collections.Generic;
using Magia.GameLogic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Magia.Utilities.Json.Editor
{
    public class StageDataEditorVisual : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private StageDataEditorVisualRadius radiusPrefab;
        private GameObject stageScene;

        private List<StageDataEditorVisualRadius> radiusList;

        public void Initialize()
        {
            radiusList = new();
        }

        public void TryLoadStageScene(string name, Action callback = null)
        {
            if (stageScene != null) Destroy(stageScene);

            StartCoroutine(LoadAddressableAsset(name, () =>
            {
                callback?.Invoke();
            }));
        }

        public void ShowPlayerSpawn(StageSettingData data)
        {
            playerSpawnPoint.SetPositionAndRotation(data.PlayerSpawnPosition, Quaternion.Euler(data.PlayerSpawnRotation));
        }

        public void ShowSpawnRadius(List<EnemySpawnData> spawns)
        {
            for (int i = radiusList.Count; i < spawns.Count; i++)
            {
                var radius = radiusPrefab.Rent(transform);
                radius.Initialize();
                radiusList.Add(radius);
            }

            if (spawns.Count < radiusList.Count)
            {
                for (int i = spawns.Count; i < radiusList.Count; i++)
                {
                    radiusList[i].Dispose();
                }

                radiusList.RemoveRange(spawns.Count, radiusList.Count - spawns.Count);
            }

            for (int i = 0; i < spawns.Count; i++)
            {
                radiusList[i].UpdateInfo(spawns[i].SpawnRange);
                radiusList[i].transform.position = 20f * (spawns.Count - i) * Vector3.up;
            }
        }

        private IEnumerator LoadAddressableAsset(string key, Action callback = null)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(key);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var stage = handle.Result;
                stageScene = Instantiate(stage, transform);
                callback?.Invoke();
            }
        }
    }
}
