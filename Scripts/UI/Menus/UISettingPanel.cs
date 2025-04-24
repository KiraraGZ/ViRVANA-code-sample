using System;
using Magia.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UISettingPanel : MonoBehaviour
    {
        public event Action EventBackButtonClicked;
        public event Action<float> EventHorizontalSensitivityChanged;
        public event Action<float> EventVerticalSensitivityChanged;
        public event Action<float> EventMasterLevelChanged;
        public event Action<float> EventMusicLevelChanged;
        public event Action<float> EventSFXLevelChanged;
        public event Action<bool> EventUIToggleChanged;
        public event Action<int> EventLanguageChanged;

        [SerializeField] private Button backButton;
        [SerializeField] private UISettingBox settingBox;

        public void Initialize(SettingManager settingManager)
        {
            gameObject.SetActive(true);

            settingBox.Initialize(settingManager);

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            settingBox.Dispose();

            RemoveListener();
        }

        private void AddListener()
        {
            settingBox.EventHorizontalSensitivityChanged += OnHorizontalSensitivityValueChanged;
            settingBox.EventVerticalSensitivityChanged += OnVerticalSensitivityValueChanged;
            settingBox.EventMasterLevelChanged += OnMasterLevelChanged;
            settingBox.EventMusicLevelChanged += OnMusicLevelChanged;
            settingBox.EventSFXLevelChanged += OnSFXLevelChanged;
            settingBox.EventUIToggleChanged += OnUIToggleChanged;
            settingBox.EventLanguageChanged += OnLanguageChanged;
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void RemoveListener()
        {
            settingBox.EventHorizontalSensitivityChanged -= OnHorizontalSensitivityValueChanged;
            settingBox.EventVerticalSensitivityChanged -= OnVerticalSensitivityValueChanged;
            settingBox.EventMasterLevelChanged -= OnMasterLevelChanged;
            settingBox.EventMusicLevelChanged -= OnMusicLevelChanged;
            settingBox.EventSFXLevelChanged -= OnSFXLevelChanged;
            settingBox.EventUIToggleChanged -= OnUIToggleChanged;
            settingBox.EventLanguageChanged -= OnLanguageChanged;
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        #region subscribe events
        private void OnBackButtonClicked()
        {
            EventBackButtonClicked?.Invoke();
            Dispose();
        }

        private void OnHorizontalSensitivityValueChanged(float value)
        {
            EventHorizontalSensitivityChanged?.Invoke(value);
        }

        private void OnVerticalSensitivityValueChanged(float value)
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
