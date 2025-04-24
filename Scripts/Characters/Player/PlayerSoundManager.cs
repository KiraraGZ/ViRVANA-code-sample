using Magia.Audio;
using Magia.Skills;
using UnityEngine;

namespace Magia.Player
{
    public class PlayerSoundManager : MonoBehaviour
    {

        [SerializeField] private AudioSource clothSound;
        [SerializeField] private AudioSource sfxSource;

        [Header("SFX")]
        [SerializeField] private AudioClip attack;
        [SerializeField] private AudioClip spreadAttack;
        [SerializeField] private AudioClip clockSkillPerformed;
        [SerializeField] private AudioClip coneSkillPerformed;

        public void UpdateVelocity(Vector3 velocity)
        {
            clothSound.volume = MapVolumeRange(velocity.magnitude, 0, 60, 0.25f, 1f);
        }

        public void PlayAttackSound()
        {
            sfxSource.pitch = SoundHelper.GetRandomPitch(sfxSource.pitch);
            sfxSource.PlayOneShot(attack);
        }

        public void PlaySpreadAttackSound()
        {
            sfxSource.pitch = 1;
            sfxSource.PlayOneShot(spreadAttack);
        }

        public void PlayOneShotSound(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public void PerformSkillSound(int number)
        {
            switch ((SkillType)number)
            {
                case SkillType.CLOCK:
                    sfxSource.PlayOneShot(clockSkillPerformed);
                    break;
                case SkillType.BARRIER:
                    sfxSource.clip = coneSkillPerformed;
                    sfxSource.Play();
                    break;
                default:
                    sfxSource.PlayOneShot(clockSkillPerformed);
                    break;
            }
        }

        public void ReleaseSkillSound()
        {
            sfxSource.Stop();
        }

        private float MapVolumeRange(float value, float minInputRange, float maxInputRange, float minResultRange, float maxResultRange)
        {
            return Mathf.Clamp(minResultRange + (value - minInputRange) * (maxResultRange - minResultRange) / (maxInputRange - minInputRange), 0, 1);
        }
    }
}
