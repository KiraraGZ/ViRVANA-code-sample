using System;
using System.Collections.Generic;
using Magia.GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UISettingBox : MonoBehaviour
    {
        public event Action<float> EventHorizontalSensitivityChanged;
        public event Action<float> EventVerticalSensitivityChanged;
        public event Action<float> EventMasterLevelChanged;
        public event Action<float> EventMusicLevelChanged;
        public event Action<float> EventSFXLevelChanged;
        public event Action<bool> EventUIToggleChanged;
        public event Action<int> EventLanguageChanged;

        [SerializeField] private Slider horizontalSensitivitySlider;
        [SerializeField] private Slider verticalSensitivitySlider;
        [SerializeField] private TMP_Text horizontalSensitivityText;
        [SerializeField] private TMP_Text verticalSensitivityText;

        [SerializeField] private Slider masterLevelSlider;
        [SerializeField] private Slider musicLevelSlider;
        [SerializeField] private Slider sfxLevelSlider;

        [SerializeField] private Toggle uiToggle;

        [SerializeField] private TMP_Dropdown languageDropdown;

        public void Initialize(SettingManager settingManager)
        {
            ShowSensitivityInfo(settingManager);
            ShowSoundInfo(settingManager);
            ShowLanguageInfo(settingManager.LanguageIndex);

            AddListener();
        }

        public void Dispose()
        {
            HideSensitivityInfo();
            HideSoundInfo();
            HideLanguageInfo();

            RemoveListener();
        }

        private void ShowSensitivityInfo(SettingManager setting)
        {
            horizontalSensitivitySlider.maxValue = setting.MaxHorizontalSensitivity;
            horizontalSensitivitySlider.minValue = setting.MinHorizontalSensitivity;
            horizontalSensitivitySlider.value = setting.HorizontalSensitivity;
            horizontalSensitivityText.text = setting.HorizontalSensitivity.ToString("F2");
            verticalSensitivitySlider.maxValue = setting.MaxVerticalSensitivity;
            verticalSensitivitySlider.minValue = setting.MinVerticalSensitivity;
            verticalSensitivitySlider.value = setting.VerticalSensitivity;
            verticalSensitivityText.text = setting.VerticalSensitivity.ToString("F0");
        }

        private void HideSensitivityInfo()
        {

        }

        private void ShowSoundInfo(SettingManager setting)
        {
            masterLevelSlider.value = setting.MasterLevel;
            musicLevelSlider.value = setting.MusicLevel;
            sfxLevelSlider.value = setting.SFXLevel;
        }

        private void HideSoundInfo()
        {

        }

        private void ShowLanguageInfo(int index)
        {
            var options = new List<string>();

            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                options.Add(locale.Identifier.CultureInfo.NativeName);
            }

            languageDropdown.AddOptions(options);
            languageDropdown.value = index;
        }

        private void HideLanguageInfo()
        {
            languageDropdown.ClearOptions();
        }

        private void AddListener()
        {
            horizontalSensitivitySlider.onValueChanged.AddListener(OnHorizontalSensitivityValueChanged);
            verticalSensitivitySlider.onValueChanged.AddListener(OnVerticalSensitivityValueChanged);

            masterLevelSlider.onValueChanged.AddListener(OnMasterLevelChanged);
            musicLevelSlider.onValueChanged.AddListener(OnMusicLevelChanged);
            sfxLevelSlider.onValueChanged.AddListener(OnSFXLevelChanged);

            uiToggle.onValueChanged.AddListener(OnUIToggleChanged);

            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        private void RemoveListener()
        {
            horizontalSensitivitySlider.onValueChanged.RemoveListener(OnHorizontalSensitivityValueChanged);
            verticalSensitivitySlider.onValueChanged.RemoveListener(OnVerticalSensitivityValueChanged);

            masterLevelSlider.onValueChanged.RemoveListener(OnMasterLevelChanged);
            musicLevelSlider.onValueChanged.RemoveListener(OnMusicLevelChanged);
            sfxLevelSlider.onValueChanged.RemoveListener(OnSFXLevelChanged);

            uiToggle.onValueChanged.RemoveListener(OnUIToggleChanged);

            languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
        }

        #region subscribe events
        private void OnHorizontalSensitivityValueChanged(float value)
        {
            horizontalSensitivityText.text = value.ToString("F2");
            EventHorizontalSensitivityChanged?.Invoke(value);
        }

        private void OnVerticalSensitivityValueChanged(float value)
        {
            verticalSensitivityText.text = value.ToString("F0");
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
