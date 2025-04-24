
using System;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.Utilities.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorEnemy : PoolObject<UIStageDataEditorEnemy>
    {
        public event Action<int, EnemySpawnData> EventUpdate;
        public event Action<int> EventRemove;

        [SerializeField] private Button removeButton;
        [SerializeField] private TMP_InputField enemy;
        [SerializeField] private TMP_InputField maxAmount;
        [SerializeField] private TMP_InputField amount;
        [SerializeField] private TMP_InputField n;
        [SerializeField] private TMP_InputField interval;
        [SerializeField] private TMP_InputField minRange;
        [SerializeField] private TMP_InputField maxRange;
        [SerializeField] private Image image;

        [SerializeField] private Sprite emptySprite;

        private EnemySO enemySO;
        private int index;

        private bool isUpdateInfo;

        public void Initialize(int _index)
        {
            EventRemove = null;
            EventUpdate = null;
            index = _index;

            AddListener();
        }

        public void Dispose()
        {
            EventRemove = null;
            EventUpdate = null;

            RemoveListener();

            Return();
        }

        private void AddListener()
        {
            removeButton.onClick.AddListener(OnRemove);
            enemy.onValueChanged.AddListener(OnEnemyUpdate);
            maxAmount.onValueChanged.AddListener(OnInputUpdate);
            amount.onValueChanged.AddListener(OnInputUpdate);
            interval.onValueChanged.AddListener(OnInputUpdate);
            minRange.onValueChanged.AddListener(OnInputUpdate);
            maxRange.onValueChanged.AddListener(OnInputUpdate);
        }

        private void RemoveListener()
        {
            removeButton.onClick.RemoveAllListeners();
            enemy.onValueChanged.RemoveAllListeners();
            maxAmount.onValueChanged.RemoveAllListeners();
            amount.onValueChanged.RemoveAllListeners();
            interval.onValueChanged.RemoveAllListeners();
            minRange.onValueChanged.RemoveAllListeners();
            maxRange.onValueChanged.RemoveAllListeners();
        }

        public void UpdateInfo(EnemySpawnData spawnData)
        {
            isUpdateInfo = true;

            enemy.text = spawnData.Enemy;
            maxAmount.text = spawnData.MaxAmount.ToString();
            amount.text = spawnData.SpawnAmount.ToString();
            interval.text = spawnData.SpawnInterval.ToString();
            minRange.text = spawnData.SpawnRange.x.ToString();
            maxRange.text = spawnData.SpawnRange.y.ToString();
            TryLoadEnemy();

            isUpdateInfo = false;
        }

        private void UpdateData()
        {
            if (isUpdateInfo) return;

            var data = new EnemySpawnData
            {
                Enemy = name,
                MaxAmount = int.Parse(maxAmount.text),
                SpawnAmount = int.Parse(amount.text),
                SpawnInterval = float.Parse(interval.text),
                SpawnRange = new(float.Parse(minRange.text), float.Parse(maxRange.text))
            };

            EventUpdate?.Invoke(index, data);
        }

        private void TryLoadEnemy(Action callback = null)
        {
            name = enemy.text;
            if (name == null)
            {
                SetEnemyEmpty();
                return;
            }

            var handle = Addressables.LoadAssetAsync<EnemySO>(name);

            handle.Completed += _ =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    enemySO = handle.Result;
                    image.sprite = enemySO.Icon;

                    callback?.Invoke();
                    return;
                }

                SetEnemyEmpty();
            };
        }

        private void SetEnemyEmpty()
        {
            enemySO = null;
            image.sprite = emptySprite;
        }

        #region subscribe events
        private void OnRemove()
        {
            EventRemove?.Invoke(index);
        }

        private void OnEnemyUpdate(string _)
        {
            if (isUpdateInfo)
            {
                TryLoadEnemy();
            }
            else
            {
                TryLoadEnemy(() => { UpdateData(); });
            }
        }

        private void OnInputUpdate(string _)
        {
            UpdateData();
        }
        #endregion
    }
}
