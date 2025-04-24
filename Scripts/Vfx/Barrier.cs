using System.Collections;
using UnityEngine;

namespace Magia.Vfx
{
    public class Barrier : MonoBehaviour
    {
        private const string DISPLACEMENT_KEY = "_Displacement";
        private const string DISSOLVE_KEY = "_Clip";
        private const string CORRUPTED_KEY = "_IsCorrupted";

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material material;
        [SerializeField] private float buildUpTime;
        [SerializeField] private float dissolveTime;
        [SerializeField] private float corruptTime;

        [Header("Unimplement")]
        [SerializeField] private AnimationCurve displacementCurve;
        [SerializeField] private float displacementTime;

        private Coroutine dissolveCoroutine;

        public void Initialize()
        {
            _renderer.material = new(material);
        }

        public void Dispose()
        {
            Destroy(_renderer.material);
        }

        public void Hit()
        {
            StartCoroutine(HitDisplacement());
        }

        public void LifeTrigger(float duration)
        {
            TemporaryShow(duration);
        }

        public void BuildUp()
        {
            SetTargetDissolve(1, buildUpTime, false);
        }

        public void Dissolve()
        {
            SetTargetDissolve(0, dissolveTime, false);
        }

        public void Corrupt()
        {
            SetTargetDissolve(0, corruptTime, true);
        }

        public void SetTargetDissolve(float target, float duration, bool isCorrupted)
        {
            _renderer.material.SetFloat(CORRUPTED_KEY, isCorrupted ? 1f : 0f);
            if (dissolveCoroutine != null)
            {
                StopCoroutine(dissolveCoroutine);
            }

            dissolveCoroutine = StartCoroutine(Dissolve(target, duration));
        }

        private IEnumerator HitDisplacement()
        {
            float lerp = 0;

            while (lerp < 1)
            {
                _renderer.material.SetFloat(DISPLACEMENT_KEY, displacementCurve.Evaluate(lerp));
                lerp += Time.deltaTime / displacementTime;
                yield return null;
            }
        }

        private IEnumerator Dissolve(float target, float duration)
        {
            float start = _renderer.material.GetFloat(DISSOLVE_KEY);
            float lerp = 0;

            while (lerp < 1)
            {
                _renderer.material.SetFloat(DISSOLVE_KEY, Mathf.Lerp(start, target, lerp));
                lerp += Time.deltaTime / duration;
                yield return null;
            }
        }

        private IEnumerator TemporaryShow(float duration)
        {
            BuildUp();
            yield return new WaitForSeconds(duration);
            Corrupt();
        }
    }
}
