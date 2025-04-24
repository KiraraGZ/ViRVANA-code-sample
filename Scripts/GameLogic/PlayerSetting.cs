using Magia.Audio;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Magia.GameLogic
{
    public class SettingManager
    {
        #region sensitivity
        private const string HORIZONTAL_SENSITIVITY_KEY = "HorizontalSensitivity";
        private const string VERTICAL_SENSITIVITY_KEY = "VerticalSensitivity";

        private const float HORIZONTAL_SENSITIVITY_MAX = 1;
        private const float HORIZONTAL_SENSITIVITY_MIN = 0.05f;
        private const float HORIZONTAL_SENSITIVITY_DEFAULT = 0.3f;
        private const float VERTICAL_SENSITIVITY_MAX = 600;
        private const float VERTICAL_SENSITIVITY_MIN = 100;
        private const float VERTICAL_SENSITIVITY_DEFAULT = 300;
        public float MaxHorizontalSensitivity => HORIZONTAL_SENSITIVITY_MAX;
        public float MinHorizontalSensitivity => HORIZONTAL_SENSITIVITY_MIN;
        public float MaxVerticalSensitivity => VERTICAL_SENSITIVITY_MAX;
        public float MinVerticalSensitivity => VERTICAL_SENSITIVITY_MIN;

        public float HorizontalSensitivity
        {
            get
            {
                if (!PlayerPrefs.HasKey(HORIZONTAL_SENSITIVITY_KEY))
                {
                    PlayerPrefs.SetFloat(HORIZONTAL_SENSITIVITY_KEY, HORIZONTAL_SENSITIVITY_DEFAULT);
                }

                return PlayerPrefs.GetFloat(HORIZONTAL_SENSITIVITY_KEY);
            }
            set
            {
                PlayerPrefs.SetFloat(HORIZONTAL_SENSITIVITY_KEY, value);
            }
        }

        public float VerticalSensitivity
        {
            get
            {
                if (!PlayerPrefs.HasKey(VERTICAL_SENSITIVITY_KEY))
                {
                    PlayerPrefs.SetFloat(VERTICAL_SENSITIVITY_KEY, VERTICAL_SENSITIVITY_DEFAULT);
                }

                return PlayerPrefs.GetFloat(VERTICAL_SENSITIVITY_KEY);
            }
            set
            {
                PlayerPrefs.SetFloat(VERTICAL_SENSITIVITY_KEY, value);
            }
        }
        #endregion

        #region sound
        private const string MASTER_LEVEL_KEY = "MasterVolume";
        private const string MUSIC_LEVEL_KEY = "MusicVolume";
        private const string SFX_LEVEL_KEY = "SFXVolume";

        public float MasterLevel
        {
            get
            {
                if (!PlayerPrefs.HasKey(MASTER_LEVEL_KEY))
                {
                    PlayerPrefs.SetFloat(MASTER_LEVEL_KEY, 0.5f);
                }

                return PlayerPrefs.GetFloat(MASTER_LEVEL_KEY);
            }
            set
            {
                PlayerPrefs.SetFloat(MASTER_LEVEL_KEY, value);
                SoundSetting.MasterVolume = SoundSetting.GetVolume(value);
            }
        }

        public float MusicLevel
        {
            get
            {
                if (!PlayerPrefs.HasKey(MUSIC_LEVEL_KEY))
                {
                    PlayerPrefs.SetFloat(MUSIC_LEVEL_KEY, 0.5f);
                }

                return PlayerPrefs.GetFloat(MUSIC_LEVEL_KEY);
            }
            set
            {
                PlayerPrefs.SetFloat(MUSIC_LEVEL_KEY, value);
                SoundSetting.MusicVolume = SoundSetting.GetVolume(value);
            }
        }

        public float SFXLevel
        {
            get
            {
                if (!PlayerPrefs.HasKey(SFX_LEVEL_KEY))
                {
                    PlayerPrefs.SetFloat(SFX_LEVEL_KEY, 0.5f);
                }

                return PlayerPrefs.GetFloat(SFX_LEVEL_KEY);
            }
            set
            {
                PlayerPrefs.SetFloat(SFX_LEVEL_KEY, value);
                SoundSetting.SFXVolume = SoundSetting.GetVolume(value);
            }
        }

        public SoundSetting SoundSetting { get; private set; }
        #endregion

        private const string LANGUAGE_KEY = "LanguageValue";

        public int LanguageIndex
        {
            get
            {
                if (!PlayerPrefs.HasKey(LANGUAGE_KEY))
                {
                    PlayerPrefs.SetInt(LANGUAGE_KEY, 0);
                }

                return PlayerPrefs.GetInt(LANGUAGE_KEY);
            }
            set
            {
                PlayerPrefs.SetInt(LANGUAGE_KEY, value);
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
            }
        }

        public SettingManager()
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageIndex];
            SoundSetting = new(MasterLevel, MusicLevel, SFXLevel);
        }
    }
}
