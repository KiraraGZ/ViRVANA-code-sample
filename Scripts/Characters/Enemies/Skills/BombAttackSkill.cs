using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class BombAttackSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private BombAttackSkillSO data;

        private float nextAvailableTime;
        private float nextFireTime;
        private int projectileAmount;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private ProjectilePoolManager projectilePool;

        public void Initialize(BombAttackSkillSO _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            nextAvailableTime = Time.time + data.Downtime;

            projectilePool = new(data.BombProjectilePrefab);
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
            if (Time.time < nextAvailableTime) return false;

            float distance = new Vector3(player.transform.position.x - owner.transform.position.x, 0, player.transform.position.z - owner.transform.position.z).magnitude;
            return distance <= data.DecisionDistance;
        }

        public void Cast()
        {
            projectileAmount = data.MaxProjectileAmount;
            nextFireTime = Time.time + data.Interval;
        }

        private void Fire()
        {
            float distanceToPlayer = Mathf.Abs(player.transform.position.y - owner.transform.position.y);

            float acceleration = 2 * distanceToPlayer / Mathf.Pow(data.ReachPlayerDuration, 2);
            float initialVerticalSpeed = acceleration * data.ReachPlayerDuration;

            Vector3 horizonToPlayer = new(player.transform.position.x - owner.transform.position.x, 0, player.transform.position.z - owner.transform.position.z);

            Vector3 initialSpeed1 = new(horizonToPlayer.x / data.ReachPlayerDuration, initialVerticalSpeed, horizonToPlayer.z / data.ReachPlayerDuration);
            Vector3 initialSpeed2 = Quaternion.Euler(0, 180, 0) * initialSpeed1;

            ExplodeProjectileData data1 = new(data.ExplodeProjectileData);
            ExplodeProjectileData data2 = new(data.ExplodeProjectileData);

            data1.ProjectileData.Speed = initialSpeed1.magnitude;
            data1.ProjectileData.Lifetime = data.ReachPlayerDuration;
            data1.VerticalAcceleration = acceleration;

            data2.ProjectileData.Speed = initialSpeed2.magnitude;
            data2.ProjectileData.Lifetime = data.ReachPlayerDuration;
            data2.VerticalAcceleration = acceleration;

            ExplodeProjectile Projectile1 = projectilePool.Rent(owner.transform.position, Quaternion.identity) as ExplodeProjectile;
            Projectile1.Initialize(data1, initialSpeed1.normalized, owner);

            ExplodeProjectile Projectile2 = projectilePool.Rent(owner.transform.position, Quaternion.identity) as ExplodeProjectile;
            Projectile2.Initialize(data2, initialSpeed2.normalized, owner);
        }
    }
}
