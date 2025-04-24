using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.UI.Menu
{
    public class UIMenuManager : MonoBehaviour
    {
        public event Action EventStartButtonClicked;
        public event Action<float> EventHorizontalSensitivityChanged;
        public event Action<float> EventVerticalSensitivityChanged;
        public event Action<float> EventMasterLevelChanged;
        public event Action<float> EventMusicLevelChanged;
        public event Action<float> EventSFXLevelChanged;
        public event Action<bool> EventUIToggleChanged;
        public event Action<int> EventLanguageChanged;

        [SerializeField] private UITitlePanel titlePanel;
        [SerializeField] private UISettingPanel settingPanel;
        [SerializeField] private UICreditPanel creditPanel;

        private GameplayController gameplayController;

        public void Initialize(GameplayController _gameplayController)
        {
            gameplayController = _gameplayController;

            ShowTitlePanel();

            AddListener();
        }

        public void Dispose()
        {
            gameplayController = null;

            RemoveListener();
        }

        private void AddListener()
        {
            titlePanel.EventStartButtonClicked += OnMenuStartButtonClicked;
            titlePanel.EventSettingButtonClicked += ShowSettingPanel;
            titlePanel.EventCreditButtonClicked += ShowCreditPanel;

            settingPanel.EventBackButtonClicked += ShowTitlePanel;
            settingPanel.EventHorizontalSensitivityChanged += OnHorizontalSensitivityChanged;
            settingPanel.EventVerticalSensitivityChanged += OnVerticalSensitivityChanged;
            settingPanel.EventMasterLevelChanged += OnMasterLevelChanged;
            settingPanel.EventMusicLevelChanged += OnMusicLevelChanged;
            settingPanel.EventSFXLevelChanged += OnSFXLevelChanged;
            settingPanel.EventUIToggleChanged += OnUIToggleChanged;
            settingPanel.EventLanguageChanged += OnLanguageChanged;

            creditPanel.EventBackButtonClicked += ShowTitlePanel;
        }

        private void RemoveListener()
        {
            titlePanel.EventStartButtonClicked -= OnMenuStartButtonClicked;
            titlePanel.EventSettingButtonClicked -= ShowSettingPanel;
            titlePanel.EventCreditButtonClicked -= ShowCreditPanel;

            settingPanel.EventBackButtonClicked -= ShowTitlePanel;
            settingPanel.EventHorizontalSensitivityChanged -= OnHorizontalSensitivityChanged;
            settingPanel.EventVerticalSensitivityChanged -= OnVerticalSensitivityChanged;
            settingPanel.EventMasterLevelChanged -= OnMasterLevelChanged;
            settingPanel.EventMusicLevelChanged -= OnMusicLevelChanged;
            settingPanel.EventSFXLevelChanged -= OnSFXLevelChanged;
            settingPanel.EventUIToggleChanged -= OnUIToggleChanged;
            settingPanel.EventLanguageChanged -= OnLanguageChanged;

            creditPanel.EventBackButtonClicked -= ShowTitlePanel;
        }

        private void ShowTitlePanel()
        {
            titlePanel.Initialize();
        }

        private void ShowSettingPanel()
        {
            settingPanel.Initialize(gameplayController.SettingManager);
        }

        private void ShowCreditPanel()
        {
            creditPanel.Initialize();
        }

        #region subscribe events
        private void OnMenuStartButtonClicked()
        {
            EventStartButtonClicked?.Invoke();
        }

        private void OnHorizontalSensitivityChanged(float value)
        {
            EventHorizontalSensitivityChanged?.Invoke(value);
        }

        private void OnVerticalSensitivityChanged(float value)
        {
            EventVerticalSensitivityChanged?.Invoke(value);
        }

        private void OnMasterLevelChanged(float value)
        {
            EventMasterLevelChanged?.Invoke(value);
        }

        private void OnMusicLevelChanged(float value)
        {
            EventMusicLevelChanged?.Invoke(value);
        }

        private void OnSFXLevelChanged(float value)
        {
            EventSFXLevelChanged?.Invoke(value);
        }

        private void OnUIToggleChanged(bool value)
        {
            EventUIToggleChanged?.Invoke(value);
        }

        public void OnLanguageChanged(int index)
        {
            EventLanguageChanged?.Invoke(index);
        }
        #endregion
    }
}
