using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UIStageSelectPopup : MonoBehaviour
    {
        public event Action EventChangeModeButtonClicked;
        public event Action EventStartButtonClicked;

        [SerializeField] private Button startButton;
        [SerializeField] private Button changeModeButton;
        [SerializeField] private Button backButton;

        [Header("Display")]
        [SerializeField] private TMP_Text stageNameText;

        [Header("Mode")]
        [SerializeField] private Image normalModeBg;
        [SerializeField] private Image extremeModeBg;

        [Header("Rating")]
        [SerializeField] private Image[] ratingImages;

        [Header("Enemy")]
        [SerializeField] private Image[] bossIcons;
        [SerializeField] private Image[] enemyIcons;

        [Header("Reward")]
        [SerializeField] private GameObject firstTimeBox;
        [SerializeField] private TMP_Text firstTimeText;
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] private Image firstTimeCurrencyImage;
        [SerializeField] private Sprite[] currencySprites;

        public void Initialize()
        {
            gameObject.SetActive(false);

            AddListener();
        }

        public void Dispose()
        {
            Hide();

            RemoveListener();
        }

        private void AddListener()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            changeModeButton.onClick.AddListener(OnChangeModeButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void RemoveListener()
        {
            startButton.onClick.RemoveAllListeners();
            changeModeButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        #region info visualize methods
        public void Display(UIStageSelectPopupData data)
        {
            gameObject.SetActive(true);

            stageNameText.text = $"{data.StageId}";

            // UpdateMode(data.Mode);
            UpdateRating(data.Status);
            UpdateEnemyInfo(data.EnemyDict);
            UpdateRewardInfo(data.StageMap, data.Status);

            var bossObjectives = data.StageMap.GetBossObjectives();
            UpdateBossInfo(bossObjectives, data.EnemyDict);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateMode(int mode)
        {
            normalModeBg.enabled = mode == 0;
            extremeModeBg.enabled = mode >= 1;
        }

        private void UpdateRating(StageStatus status)
        {
            for (int i = 0; i < 3; i++)
            {
                ratingImages[i].enabled = status.Rating > i;
            }
        }

        private void UpdateEnemyInfo(Dictionary<string, EnemySO> enemyDict)
        {
            int idx = 0;

            foreach (var pair in enemyDict)
            {
                enemyIcons[idx].sprite = pair.Value.Icon;
                enemyIcons[idx].enabled = true;
                if (idx >= enemyIcons.Length) return;
            }

            for (; idx < enemyIcons.Length; idx++)
            {
                enemyIcons[idx].enabled = false;
            }
        }

        private void UpdateBossInfo(List<ObjectiveData> objectives, Dictionary<string, EnemySO> enemyDict)
        {
            if (objectives.Count == 0)
            {
                foreach (var bossIcon in bossIcons)
                {
                    bossIcon.enabled = false;
                }
                return;
            }

            foreach (var objective in objectives)
            {
                for (int i = 0; i < objective.Boss.Bosses.Length; i++)
                {
                    bossIcons[i].enabled = true;
                    bossIcons[i].sprite = objective.Boss.Bosses[i].Icon;
                }

                for (int i = objective.Boss.Bosses.Length; i < bossIcons.Length; i++)
                {
                    bossIcons[i].enabled = false;
                }
            }
        }

        private void UpdateRewardInfo(StageMapData data, StageStatus status)
        {
            experienceText.text = data.ExperienceRewards[^1].ToString();
            firstTimeText.text = data.FirstTimeReward.ToString();
            firstTimeCurrencyImage.sprite = currencySprites[0];
            firstTimeBox.SetActive(status.Rating < 3);
        }
        #endregion

        #region subscribe events

        private void OnStartButtonClicked()
        {
            EventStartButtonClicked?.Invoke();

            Dispose();
        }

        private void OnChangeModeButtonClicked()
        {
            EventChangeModeButtonClicked?.Invoke();
        }

        private void OnBackButtonClicked()
        {
            Hide();
        }
        #endregion
    }

    public class UIStageSelectPopupData
    {
        public string StageId;
        public StageMapData StageMap;
        public Dictionary<string, EnemySO> EnemyDict;
        public StageStatus Status;

        public UIStageSelectPopupData(string stageId, StageMapData stageMap, Dictionary<string, EnemySO> enemyDict, StageStatus status)
        {
            StageId = stageId;
            EnemyDict = enemyDict;
            StageMap = stageMap;
            Status = status;
        }
    }
}
