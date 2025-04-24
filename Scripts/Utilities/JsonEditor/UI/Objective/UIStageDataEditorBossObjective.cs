using System;
using System.Collections.Generic;
using Magia.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorBossObjective : MonoBehaviour
    {
        public event Action<int, BossSpawnData> EventUpdateBoss;
        public event Action<int> EventRemoveBoss;

        public event Action EventAddBossButtonClicked;

        [SerializeField] private Button addBossButton;

        [SerializeField] private UIStageDataEditorBoss bossPrefab;
        private List<UIStageDataEditorBoss> bossList;
        [SerializeField] private RectTransform bossContainer;

        public void Initialize()
        {
            gameObject.SetActive(true);

            bossList = new();

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            bossList = null;

            RemoveListener();
        }

        private void AddListener()
        {
            addBossButton.onClick.AddListener(OnAddBossButtonClicked);
        }

        private void RemoveListener()
        {
            addBossButton.onClick.RemoveAllListeners();
        }

        public void UpdateInfo(BossObjectiveData data)
        {
            for (int i = bossList.Count; i < data.Bosses.Length; i++)
            {
                var boss = bossPrefab.Rent(bossContainer);
                boss.Initialize(i);
                boss.EventUpdate += OnUpdateBoss;
                boss.EventRemove += OnRemoveBoss;
                bossList.Add(boss);
            }

            if (data.Bosses.Length < bossList.Count)
            {
                for (int i = data.Bosses.Length; i < bossList.Count; i++)
                {
                    bossList[i].Dispose();
                }

                bossList.RemoveRange(data.Bosses.Length, bossList.Count - data.Bosses.Length);
            }

            for (int i = 0; i < data.Bosses.Length; i++)
            {
                // bossList[i].UpdateInfo(data.Bosses[i]);
            }
        }

        private void OnUpdateBoss(int index, BossSpawnData data)
        {
            EventUpdateBoss?.Invoke(index, data);
        }

        private void OnRemoveBoss(int index)
        {
            EventRemoveBoss?.Invoke(index);
        }

        private void OnAddBossButtonClicked()
        {
            EventAddBossButtonClicked?.Invoke();
        }
    }
}
