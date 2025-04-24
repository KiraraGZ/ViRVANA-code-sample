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
    public class UIStageDataEditorBoss : PoolObject<UIStageDataEditorBoss>
    {
        public event Action<int, BossSpawnData> EventUpdate;
        public event Action<int> EventRemove;

        [SerializeField] private Button removeButton;
        [SerializeField] private TMP_InputField boss;
        [SerializeField] private TMP_InputField posX;
        [SerializeField] private TMP_InputField posY;
        [SerializeField] private TMP_InputField posZ;
        [SerializeField] private TMP_InputField rotX;
        [SerializeField] private TMP_InputField rotY;
        [SerializeField] private TMP_InputField rotZ;
        [SerializeField] private Image image;
        [SerializeField] private Sprite emptySprite;

        private GameObject bossObject;
        private EnemySO bossSO;
        private Vector3 spawnPosition;
        private Vector3 spawnRotation;
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
            boss.onValueChanged.AddListener(OnEnemyUpdate);
            posX.onValueChanged.AddListener(OnPositionUpdate);
            posY.onValueChanged.AddListener(OnPositionUpdate);
            posZ.onValueChanged.AddListener(OnPositionUpdate);
            rotX.onValueChanged.AddListener(OnRotationUpdate);
            rotY.onValueChanged.AddListener(OnRotationUpdate);
            rotZ.onValueChanged.AddListener(OnRotationUpdate);
        }

        private void RemoveListener()
        {
            removeButton.onClick.RemoveAllListeners();
            boss.onValueChanged.RemoveAllListeners();
            posX.onValueChanged.RemoveAllListeners();
            posY.onValueChanged.RemoveAllListeners();
            posZ.onValueChanged.RemoveAllListeners();
            rotX.onValueChanged.RemoveAllListeners();
            rotY.onValueChanged.RemoveAllListeners();
            rotZ.onValueChanged.RemoveAllListeners();
        }

        public void UpdateInfo(BossSpawnData data)
        {
            isUpdateInfo = true;

            boss.text = data.Boss;
            posX.text = data.SpawnPosition.x.ToString();
            posY.text = data.SpawnPosition.y.ToString();
            posZ.text = data.SpawnPosition.z.ToString();
            rotX.text = data.SpawnRotation.x.ToString();
            rotY.text = data.SpawnRotation.y.ToString();
            rotZ.text = data.SpawnRotation.z.ToString();

            TryLoadEnemy();

            isUpdateInfo = false;
        }

        private void UpdateData()
        {
            if (isUpdateInfo) return;

            EventUpdate?.Invoke(index,
                new BossSpawnData
                {
                    Boss = name,
                    SpawnPosition = spawnPosition,
                    SpawnRotation = spawnRotation,
                }
            );
        }

        private void TryLoadEnemy(Action callback = null)
        {
            name = boss.text;
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
                    bossSO = handle.Result;
                    image.sprite = bossSO.Icon;

                    if (bossObject != null)
                    {
                        Destroy(bossObject);
                    }

                    bossObject = Instantiate(bossSO.Prefab, spawnPosition, Quaternion.Euler(spawnRotation)).gameObject;

                    callback?.Invoke();
                    return;
                }

                SetEnemyEmpty();
            };
        }

        private void SetEnemyEmpty()
        {
            bossSO = null;
            image.sprite = emptySprite;
        }

        #region subscribe events
        private void OnRemove()
        {
            EventRemove?.Invoke(index);
        }

        private void OnEnemyUpdate(string _)
        {
            if (isUpdateInfo) return;

            TryLoadEnemy(() => { UpdateData(); });
        }

        private void OnPositionUpdate(string _)
        {
            spawnPosition = new(TryParse(posX.text), TryParse(posY.text), TryParse(posZ.text));

            if (bossObject != null)
            {
                bossObject.transform.position = spawnPosition;
            }

            UpdateData();
        }

        private void OnRotationUpdate(string _)
        {
            spawnRotation = new(TryParse(rotX.text), TryParse(rotY.text), TryParse(rotZ.text));

            if (bossObject != null)
            {
                bossObject.transform.rotation = Quaternion.Euler(spawnRotation);
            }

            UpdateData();
        }

        private float TryParse(string input)
        {
            return float.TryParse(input, out float result) ? result : 0f;
        }
        #endregion
    }
}