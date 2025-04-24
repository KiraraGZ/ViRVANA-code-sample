using System;
using Magia.GameLogic;
using TMPro;
using UnityEngine;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorObjective : MonoBehaviour
    {
        public event Action<int> EventObjectiveModeChanged;
        // public event Action<ObjectiveData> EventObjectiveChanged;

        public event Action EventAddBossButtonClicked;
        public event Action<int, BossSpawnData> EventUpdateBoss;
        public event Action<int> EventRemoveBoss;

        [Header("Input")]
        [SerializeField] private TMP_Dropdown mode;

        [SerializeField] private UIStageDataEditorBossObjective boss;
        [SerializeField] private UIStageDataEditorHuntObjective hunt;

        private ObjectiveData.ObjectiveMode currentMode;
        private bool firstUpdate = true;

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
            mode.onValueChanged.AddListener(OnModeChanged);

            // hunt.EventObjectiveChanged += OnHuntDataChanged;

            boss.EventAddBossButtonClicked += OnAddBossButtonClicked;
            boss.EventUpdateBoss += OnUpdateBoss;
            boss.EventRemoveBoss += OnRemoveBoss;
        }

        private void RemoveListener()
        {
            mode.onValueChanged.RemoveAllListeners();

            // hunt.EventObjectiveChanged -= OnHuntDataChanged;
        }

        public void UpdateInfo(ObjectiveData data)
        {
            isUpdateInfo = true;

            if (firstUpdate || currentMode != data.Mode)
            {
                switch (data.Mode)
                {
                    case ObjectiveData.ObjectiveMode.BOSS:
                        mode.value = 0;
                        boss.Initialize();
                        hunt.Dispose();
                        break;
                    case ObjectiveData.ObjectiveMode.HUNT:
                        mode.value = 1;
                        hunt.Initialize();
                        boss.Dispose();
                        break;
                }
                currentMode = data.Mode;
                firstUpdate = false;
            }

            switch (currentMode)
            {
                case ObjectiveData.ObjectiveMode.BOSS:
                    boss.UpdateInfo(data.Boss);
                    break;
                case ObjectiveData.ObjectiveMode.HUNT:
                    // hunt.UpdateInfo(data.Hunt);
                    break;
            }

            isUpdateInfo = false;
        }

        #region subscribe events
        private void OnModeChanged(int index)
        {
            if (isUpdateInfo) return;

            EventObjectiveModeChanged?.Invoke(index);
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

        // private void OnHuntDataChanged(HuntObjectiveData data)
        // {
        //     EventObjectiveChanged?.Invoke(new ObjectiveData
        //     {
        //         Mode = ObjectiveData.ObjectiveMode.HUNT,
        //         Hunt = data,
        //     });
        // }
        #endregion
    }
}
