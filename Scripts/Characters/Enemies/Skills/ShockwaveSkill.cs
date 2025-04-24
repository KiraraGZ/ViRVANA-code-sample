using System;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class ShockwaveSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private ShockwaveSkillSO data;

        private float nextAvailableTime;
        private float nextShockwaveTime;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private AudioSource audio;

        public void Initialize(ShockwaveSkillSO _data, BaseEnemy _owner, AudioSource _audio)
        {
            data = _data;
            owner = _owner;
            audio = _audio;

            nextAvailableTime = Time.time + data.Downtime;
        }

        public void Dispose()
        {
            data = null;
            owner = null;
        }

        public void UpdateLogic()
        {
            if (Time.time < nextShockwaveTime) return;

            Shockwave();
            audio.Stop();

            nextAvailableTime = Time.time + data.Downtime;
            EventSkillEnd?.Invoke();
        }

        public bool IsAvailable()
        {
            if (Time.time < nextAvailableTime) return false;

            float distance = (owner.transform.position - player.transform.position).magnitude;

            return distance <= data.MaxRange;
        }

        public void Cast()
        {
            audio.clip = data.EarthShakeSound;
            audio.Play();
            player.CameraHandler.StartCameraShake(2.5f, data.Delay);

            nextShockwaveTime = Time.time + data.Delay;
        }

        private void Shockwave()
        {
            Vector3 playerDirection = (player.transform.position - owner.transform.position).normalized;

            if (Physics.Raycast(owner.transform.position, playerDirection, out RaycastHit hit, data.MaxRange))
            {
                if (hit.transform == null) return;

                if (hit.transform.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(new(data.Damage), player.transform.position, playerDirection, null);

                    if (damageable as PlayerController != null)
                    {
                        GameObject.Instantiate(data.ImpactPlayerPrefab, hit.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        GameObject.Instantiate(data.ImpactBuildingPrefab, hit.transform.position, Quaternion.identity);
                    }
                }
            }

            nextAvailableTime = Time.time + data.Downtime;
        }
    }
}
