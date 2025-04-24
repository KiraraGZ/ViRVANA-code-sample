using UnityEngine;

namespace Magia.Enemy.Angel
{
    public class AngelAnimatorController : MonoBehaviour
    {
        private const string ATTACK_KEY = "IsAttack";

        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem[] particles;

        [Header("Sounds")]
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip chantSound;
        [SerializeField] protected AudioClip attackSound;

        public void StartChant()
        {
            animator.SetBool(ATTACK_KEY, true);
            PlayAudioLoop(chantSound);

            foreach (var particle in particles)
            {
                particle.Play();
            }
        }

        public void StopChant()
        {
            animator.SetBool(ATTACK_KEY, false);
            StopAudioLoop();

            foreach (var particle in particles)
            {
                particle.Stop();
            }
        }

        public void StartAttack()
        {
            animator.SetBool(ATTACK_KEY, true);
            PlayAudioOneShot(attackSound);

            foreach (var particle in particles)
            {
                particle.Play();
            }
        }

        public void StopAttack()
        {
            animator.SetBool(ATTACK_KEY, false);

            foreach (var particle in particles)
            {
                particle.Stop();
            }
        }

        private void PlayAudioOneShot(AudioClip audioClip)
        {
            if (audioSource == null || audioClip == null) return;

            audioSource.PlayOneShot(audioClip);
        }

        private void PlayAudioLoop(AudioClip audioClip)
        {
            if (audioSource == null || audioClip == null) return;

            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void StopAudioLoop()
        {
            if (audioSource == null) return;

            audioSource.Stop();
        }
    }
}
