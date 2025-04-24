using System.Collections;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class PufferAnimatorController : MonoBehaviour
    {
        private const string ATTACK_KEY = "IsAttack";
        private const string SCALE_KEY = "_ModelScale";
        private const string DAMAGE_KEY = "_Damage";
        private const string CLIP_KEY = "_Clip";
        private const string COLOR_KEY = "_ColorClip";
        private const float DEAD_TIME = 1f;

        [Header("Model")]
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject mouthHitbox;
        [SerializeField] private ParticleSystem particle;

        [Header("Materials")]
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private Material[] materials;

        [Header("Sounds")]
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip holdSound;
        [SerializeField] protected AudioClip strafeSound;

        private Coroutine damageCoroutine;
        private Coroutine clipCoroutine;

        public void Initialize()
        {
            var newMaterials = new Material[materials.Length];

            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                newMaterials[i] = new Material(materials[i]);
            }

            newMaterials[0].SetFloat(SCALE_KEY, transform.localScale.x);

            meshRenderer.materials = newMaterials;

            CloseMouth();
        }

        public void Dispose()
        {
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Destroy(meshRenderer.materials[i]);
            }

            StopAllCoroutines();
        }

        public void OpenMouth()
        {
            animator.SetBool(ATTACK_KEY, true);
            mouthHitbox.SetActive(true);
            particle.Play();
            StartCoroutine(InternalColor(1, 0.25f));
            PlayAudioOneShot(holdSound);
        }

        public void CloseMouth()
        {
            animator.SetBool(ATTACK_KEY, false);
            mouthHitbox.SetActive(false);
            particle.Stop();
            StartCoroutine(InternalColor(0, 0.25f));
        }

        public void TakeDamage(float duration)
        {
            SetTargetDamage(duration / 2);
        }

        public void Dead()
        {
            if (gameObject == null || gameObject.activeInHierarchy == false) return;

            CloseMouth();
            SetTargetDamage(0);
        }

        #region sfx
        public void PlayStrafeSound()
        {
            PlayAudioOneShot(strafeSound);
        }

        public void PlayAudioOneShot(AudioClip audioClip)
        {
            if (audioSource == null || audioClip == null) return;

            audioSource.PlayOneShot(audioClip);
        }
        #endregion

        #region materials
        public void SetTargetDamage(float duration)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }

            if (duration == 0)
            {
                StartCoroutine(Damage(DEAD_TIME, 0));
                return;
            }

            damageCoroutine = StartCoroutine(Damage(duration * 0.37f, duration * 0.63f));
        }

        public void SetTargetClip(float target, float duration)
        {
            if (clipCoroutine != null)
            {
                StopCoroutine(clipCoroutine);
            }

            clipCoroutine = StartCoroutine(Transforming(target, duration));
        }

        private IEnumerator Damage(float fadeInDuration, float fadeOutDuration)
        {
            float start = meshRenderer.material.GetFloat(DAMAGE_KEY);
            float lerp = 0;

            while (lerp < 1)
            {
                lerp += Time.deltaTime / fadeInDuration;
                SetMaterialsFloat(DAMAGE_KEY, start, 0, lerp);
                yield return null;
            }

            if (fadeOutDuration == 0) yield break;

            lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.deltaTime / fadeOutDuration;
                SetMaterialsFloat(DAMAGE_KEY, 0, 1, lerp);
                yield return null;
            }
        }

        private IEnumerator Transforming(float target, float duration)
        {
            float start = meshRenderer.material.GetFloat(CLIP_KEY);
            float lerp = 0;

            while (lerp < 1)
            {
                lerp += Time.deltaTime / duration;
                SetMaterialsFloat(CLIP_KEY, start, target, lerp);
                yield return null;
            }
        }

        private IEnumerator InternalColor(float target, float duration)
        {
            float start = meshRenderer.materials[1].GetFloat(COLOR_KEY);
            float lerp = 0;

            while (lerp < 1)
            {
                lerp += Time.deltaTime / duration;
                meshRenderer.materials[1].SetFloat(COLOR_KEY, Mathf.Lerp(start, target, lerp));
                yield return null;
            }
        }

        private void SetMaterialsFloat(string key, float start, float target, float lerp)
        {
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                meshRenderer.materials[i].SetFloat(key, Mathf.Lerp(start, target, lerp));
            }
        }
        #endregion

        public float GetMeshWidth()
        {
            return meshRenderer.bounds.size.x;
        }
    }
}
