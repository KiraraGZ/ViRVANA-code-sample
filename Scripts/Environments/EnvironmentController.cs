using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Magia.Environment
{
    public class EnvironmentController : MonoBehaviour
    {
        [Header("Time")]
        [SerializeField] private Volume sunset;
        [SerializeField] private Volume midnight;
        [SerializeField] private Light directionalLight;

        [Header("Weather")]
        [SerializeField] private Volume heat;

        [Header("VFX")]
        [SerializeField] private ParticleSystem smokePrefab;
        public ParticleSystem SmokePrefab => smokePrefab;

        [Header("Debug")]
        [SerializeField][Range(0f, 1f)] private float time = 1;
        [SerializeField] private bool debug = false;

        private Volume previousVolume;
        private Volume currentVolume;
        private float targetWeight;
        private float transitionTime;

        private float lightIntensity = 0.6f;

        private static EnvironmentController instance;
        public static EnvironmentController Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;

            ToggleDepthOfField(false);
        }

        public void Initialize(EnvironmentData data)
        {
            SetTimeVolume(data.Time);
            ToggleDepthOfField(true);

            time = data.Time;
        }

        public void Dispose()
        {
            sunset.weight = 1;
            midnight.weight = 0;

            if (currentVolume != null)
            {
                currentVolume.weight = 0;
                currentVolume = null;
            }

            if (previousVolume != null)
            {
                previousVolume.weight = 0;
                previousVolume = null;
            }

            ToggleDepthOfField(false);
        }

        private void Update()
        {
            if (debug)
            {
                SetTimeVolume(time);
            }

            if (currentVolume != null)
            {
                var weight = currentVolume.weight + (targetWeight > currentVolume.weight ? 1 : -1) * targetWeight * Time.deltaTime / transitionTime;
                currentVolume.weight = Mathf.Clamp(weight, 0, targetWeight);
            }

            if (previousVolume != null)
            {
                previousVolume.weight -= Time.deltaTime / transitionTime;

                if (previousVolume.weight <= 0)
                {
                    previousVolume.weight = 0;
                    previousVolume = null;
                }
            }
        }

        #region volume
        public void SetVolumeHeat(float weight, float time)
        {
            SetVolume(heat, weight, time);
        }

        private void SetTimeVolume(float time)
        {
            sunset.weight = time;
            midnight.weight = 1 - time;
            directionalLight.intensity = Mathf.Clamp(lightIntensity * time, 0.1f, lightIntensity);
        }

        private void SetVolume(Volume volume, float weight, float time)
        {
            if (currentVolume != null && currentVolume != volume)
            {
                previousVolume = currentVolume;
            }

            currentVolume = volume;
            targetWeight = weight;
            transitionTime = time;
        }

        private void ToggleDepthOfField(bool active)
        {
            if (sunset.profile.TryGet<DepthOfField>(out var depthOfField))
            {
                depthOfField.active = active;
            }

            if (midnight.profile.TryGet<DepthOfField>(out depthOfField))
            {
                depthOfField.active = active;
            }
        }
        #endregion
    }

    public class EnvironmentData
    {
        public float Time;
        public Weather Weather;

        public EnvironmentData(float time, Weather weather)
        {
            Time = time;
            Weather = weather;
        }
    }

    public enum Weather
    {
        Clear,
        Rain,
        Heat,
    }
}
