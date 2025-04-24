using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class OrbitFireSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private OrbitFireSkillSO data;

        private float nextAvailableTime;
        private float nextFireTime;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private ProjectilePoolManager homingProjectilePool;
        private ProjectilePoolManager orbitProjectilePool;

        public void Initialize(OrbitFireSkillSO _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            homingProjectilePool = new(data.CenterProjectilePrefab);
            orbitProjectilePool = new(data.SurroundProjectilePrefab);
        }

        public void Dispose()
        {
            data = null;
            owner = null;

            if (homingProjectilePool != null)
            {
                homingProjectilePool.Dispose();
                homingProjectilePool = null;
            }

            if (orbitProjectilePool != null)
            {
                orbitProjectilePool.Dispose();
                orbitProjectilePool = null;
            }
        }

        public void UpdateLogic()
        {
            if (Time.time < nextFireTime) return;

            HomingShooting();
            nextAvailableTime = Time.time + data.Downtime;

            EventSkillEnd?.Invoke();
        }

        public bool IsAvailable()
        {
            return Time.time >= nextAvailableTime;
        }

        public void Cast()
        {
            nextFireTime = Time.time + data.Duration;
        }

        private void HomingShooting()
        {
            HomingProjectile centerProjectile = homingProjectilePool.Rent(owner.transform.position, Quaternion.identity) as HomingProjectile;
            centerProjectile.Initialize(data.HomingData, player.transform.position - owner.transform.position, owner, player.gameObject);

            for (int i = 0; i != data.SurroundProjectileAmount; i++)
            {
                OrbitProjectile surroundProjectile = orbitProjectilePool.Rent(owner.transform.position, Quaternion.identity) as OrbitProjectile;
                OrbitProjectileDataInput surroundInput = new(centerProjectile, data.InitialSurroundProjectileSpeed, data.InitialSurroundProjectileRadius, i * 360f / data.SurroundProjectileAmount);
                surroundProjectile.Initialize(data.OrbitData, surroundInput, owner.transform.forward, owner, player.gameObject);
            }
        }
    }
}