using UnityEngine;
using UnityEngine.Audio;

namespace Magia.Audio
{
    public class SoundManager : MonoBehaviour
    {
        private const string MIXER_MASTER = "MasterVolume";
        private const string MIXER_MUSIC = "MusicVolume";
        private const string MIXER_SFX = "SFXVolume";

        [SerializeField] private AudioMixer audioMixer;

        public void SetVolume(SoundSetting setting)
        {
            audioMixer.SetFloat(MIXER_MASTER, setting.MasterVolume);
            audioMixer.SetFloat(MIXER_MUSIC, setting.MusicVolume);
            audioMixer.SetFloat(MIXER_SFX, setting.SFXVolume);
        }
    }

    public class SoundSetting
    {
        public float MasterVolume;
        public float MusicVolume;
        public float SFXVolume;

        public SoundSetting(float masterLevel, float musicLevel, float sfxLevel)
        {
            MasterVolume = GetVolume(masterLevel);
            MusicVolume = GetVolume(musicLevel);
            SFXVolume = GetVolume(sfxLevel);
        }

        public float GetVolume(float level)
        {
            if (level <= 0) level = 0.0001f;

            return Mathf.Log10(level) * 20;
        }
    }
}