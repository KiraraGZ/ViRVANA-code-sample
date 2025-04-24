using System;
using Magia.GameLogic;
using TMPro;
using UnityEngine;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorSetting : MonoBehaviour
    {
        public event Action<StageSettingData> EventStageSettingChanged;

        [SerializeField] private TMP_InputField mapName;
        [SerializeField] private TMP_InputField mapTime;

        [SerializeField] private TMP_InputField playerPosX;
        [SerializeField] private TMP_InputField playerPosY;
        [SerializeField] private TMP_InputField playerPosZ;
        [SerializeField] private TMP_InputField playerRotX;
        [SerializeField] private TMP_InputField playerRotY;
        [SerializeField] private TMP_InputField playerRotZ;

        private bool isUpdateInfo;

        public void Initialize()
        {
            AddListener();
        }

        public void Dispose()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            mapName.onValueChanged.AddListener(OnInputChanged);
            mapTime.onValueChanged.AddListener(OnInputChanged);
            playerPosX.onValueChanged.AddListener(OnInputChanged);
            playerPosY.onValueChanged.AddListener(OnInputChanged);
            playerPosZ.onValueChanged.AddListener(OnInputChanged);
            playerRotX.onValueChanged.AddListener(OnInputChanged);
            playerRotY.onValueChanged.AddListener(OnInputChanged);
            playerRotZ.onValueChanged.AddListener(OnInputChanged);
        }

        private void RemoveListener()
        {
            mapName.onValueChanged.RemoveAllListeners();
            mapTime.onValueChanged.RemoveAllListeners();
            playerPosX.onValueChanged.RemoveAllListeners();
            playerPosY.onValueChanged.RemoveAllListeners();
            playerPosZ.onValueChanged.RemoveAllListeners();
            playerRotX.onValueChanged.RemoveAllListeners();
            playerRotY.onValueChanged.RemoveAllListeners();
            playerRotZ.onValueChanged.RemoveAllListeners();
        }

        public void UpdateInfo(StageSettingData data)
        {
            isUpdateInfo = true;

            // mapName.text = data.MapName;
            mapTime.text = data.MapTime.ToString();
            playerPosX.text = data.PlayerSpawnPosition.x.ToString();
            playerPosY.text = data.PlayerSpawnPosition.y.ToString();
            playerPosZ.text = data.PlayerSpawnPosition.z.ToString();
            playerRotX.text = data.PlayerSpawnRotation.x.ToString();
            playerRotY.text = data.PlayerSpawnRotation.y.ToString();
            playerRotZ.text = data.PlayerSpawnRotation.z.ToString();

            isUpdateInfo = false;
        }

        private void UpdateData()
        {
            if (isUpdateInfo) return;

            EventStageSettingChanged?.Invoke(new()
            {
                // MapName = mapName.text,
                MapTime = float.Parse(mapTime.text),
                PlayerSpawnPosition = new(float.Parse(playerPosX.text), float.Parse(playerPosY.text), float.Parse(playerPosZ.text)),
                PlayerSpawnRotation = new(float.Parse(playerRotX.text), float.Parse(playerRotY.text), float.Parse(playerRotZ.text)),
            });
        }

        #region subscribe events
        private void OnInputChanged(string _)
        {
            UpdateData();
        }
        #endregion
    }
}
