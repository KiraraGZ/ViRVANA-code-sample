using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Skills
{
    public class SpiritFireSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private SpiritFireSkillSO data;

        private float nextAvailableTime;
        private float nextFireTime;
        private int projectileAmount;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private ProjectilePoolManager projectilePool;

        public void Initialize(SpiritFireSkillSO _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;
            projectilePool = new(data.ProjectilePrefab);
            nextAvailableTime = Time.time + Random.Range(data.Downtime / 2, data.Downtime);
        }

        public void Dispose()
        {
            data = null;
            owner = null;

            if (projectilePool != null)
            {
                projectilePool.Dispose();
                projectilePool = null;
            }
        }

        public void UpdateLogic()
        {
            if (Time.time < nextFireTime) return;

            if (projectileAmount > 0)
            {
                projectileAmount--;
                Fire();
                nextFireTime = Time.time + data.Interval;
            }

            if (projectileAmount <= 0)
            {
                nextAvailableTime = Time.time + data.Downtime;
                EventSkillEnd?.Invoke();
            }
        }

        public bool IsAvailable()
        {
            return Time.time >= nextAvailableTime;
        }

        public void Cast()
        {
            projectileAmount = data.Amount;
            nextFireTime = Time.time + data.Interval;
        }

        private void Fire()
        {
            var direction = data.FireDirection switch
            {
                HomingFireSkillDirection.FORWARD_DIRECTION => owner.transform.forward,
                HomingFireSkillDirection.BACKWARD_DIRECTION => -owner.transform.forward,
                HomingFireSkillDirection.TARGET_DIRECTION => player.transform.position - owner.transform.position,
                _ => owner.transform.forward,
            };
            direction = GetRandomDirection(direction);

            SpiritProjectile projectile = projectilePool.Rent(owner.transform.position, Quaternion.LookRotation(direction)) as SpiritProjectile;
            projectile.Initialize(data.SpiritData, data.HomingData, direction, owner, player.gameObject);
        }

        private Vector3 GetRandomDirection(Vector3 direction)
        {
            float randomAngleX = Random.Range(-data.SpreadAngle / 2, data.SpreadAngle / 2);
            float randomAngleY = Random.Range(-data.SpreadAngle / 2, data.SpreadAngle / 2);
            Quaternion randomRotation = Quaternion.Euler(randomAngleY, randomAngleX, 0);

            return randomRotation * direction;
        }
    }
}
