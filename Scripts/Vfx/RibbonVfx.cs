using System.Collections;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Vfx
{
    public class RibbonVfx : MonoBehaviour
    {
        private const string CLIP_KEY = "_Clip";
        private const string ENCHANCE_KEY = "_Enchance";

        [Header("Renderer")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material material;
        [SerializeField] private float transitionTime = 0.5f;
        [SerializeField] private float minClip = 0.3f;

        [Header("Particle")]
        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private Material particleMaterialPrefab;
        [SerializeField] private Vector2 minimumParticleSpeed;
        [SerializeField] private Vector2 maximumParticleSpeed;

        private float energy;

        private ElementType currentElement;
        private Material particleMaterial;
        private Coroutine clipCoroutine;
        private Coroutine enchanceCoroutine;

        public void Initialize()
        {
            _renderer.material = new(material);
            particleMaterial = new(particleMaterialPrefab);

            foreach (var particle in particles)
            {
                var renderer = particle.GetComponent<ParticleSystemRenderer>();
                renderer.material = particleMaterial;
                particle.Play();
            }

            energy = minClip;
        }

        public void Dispose()
        {
            Destroy(_renderer.material);
            Destroy(particleMaterial);
        }

        public void UpdatePlayerSpeed(float speed)
        {
            float ratio = Mathf.Clamp(speed - 1, 0, 1);
            float minimumSpeed = Mathf.Lerp(minimumParticleSpeed.x, maximumParticleSpeed.x, ratio);
            float maximumSpeed = Mathf.Lerp(minimumParticleSpeed.y, maximumParticleSpeed.y, ratio);

            foreach (var particle in particles)
            {
                var mainModule = particle.main;
                mainModule.startSpeed = new ParticleSystem.MinMaxCurve(minimumSpeed, maximumSpeed);
            }
        }

        public void UpdateEnergy(float lerp)
        {
            energy = Mathf.Lerp(minClip, 1f, lerp);

            if (clipCoroutine != null) return;

            clipCoroutine = StartCoroutine(TransitionClip(CLIP_KEY, energy, transitionTime));
        }

        public void SetElement(ElementType element)
        {
            currentElement = element;

            foreach (var keyword in _renderer.material.enabledKeywords)
            {
                _renderer.material.DisableKeyword(keyword);
            }

            _renderer.material.EnableKeyword($"_ELEMENT_{element.ToString().ToUpper()}");
        }

        public void StartPerformSkill()
        {
            if (clipCoroutine != null)
            {
                StopCoroutine(clipCoroutine);
            }

            clipCoroutine = StartCoroutine(TransitionClip(CLIP_KEY, 1f, transitionTime));

            if (enchanceCoroutine != null)
            {
                StopCoroutine(enchanceCoroutine);
            }

            enchanceCoroutine = StartCoroutine(TransitionClip(ENCHANCE_KEY, 1f, transitionTime));
        }

        public void ReleaseSkill()
        {
            if (clipCoroutine != null)
            {
                StopCoroutine(clipCoroutine);
            }

            clipCoroutine = StartCoroutine(TransitionClip(CLIP_KEY, energy, transitionTime));

            if (enchanceCoroutine != null)
            {
                StopCoroutine(enchanceCoroutine);
            }

            enchanceCoroutine = StartCoroutine(TransitionClip(ENCHANCE_KEY, 0f, transitionTime));
        }

        private IEnumerator TransitionClip(string key, float target, float duration)
        {
            float start = _renderer.material.GetFloat(key);
            float lerp = 0;

            while (lerp < 1)
            {
                _renderer.material.SetFloat(key, Mathf.Lerp(start, target, lerp));
                lerp += Time.deltaTime / duration;
                yield return null;
            }
        }
    }
}
