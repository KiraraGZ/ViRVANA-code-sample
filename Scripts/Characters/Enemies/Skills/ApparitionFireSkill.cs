using System;
using Magia.Player;
using Magia.Projectiles;
using Magia.Utilities.Tools;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class ApparitionFireSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private ApparitionFireSkillSO data;

        private float nextAvailableTime;
        private float nextFireTime;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private ProjectilePoolManager projectilePool;

        public void Initialize(ApparitionFireSkillSO _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            projectilePool = new(data.ProjectilePrefab);
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
            DebugVisualizer.Instance.UpdateLog("Hover", $"{nextFireTime}");

            if (Time.time < nextFireTime) return;

            Vector3 directionToPlayer = new Vector3(player.transform.position.x - owner.transform.position.x, 0, player.transform.position.z - owner.transform.position.z).normalized;
            Vector3 ownDirection = owner.transform.forward;

            Fire();
            nextFireTime = Time.time + data.Interval;

            if (Vector3.Dot(directionToPlayer, ownDirection) > Mathf.Cos(data.MinRotateDegreesThreshold * Mathf.Deg2Rad))
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
            nextAvailableTime = Time.time + data.Downtime;
        }

        private void Fire()
        {
            Vector3 direction = (player.transform.position - owner.transform.position).normalized;
            RotatableApparitionProjectile projectile = projectilePool.Rent(owner.transform.position + (direction * data.ApparitionDistance), Quaternion.identity) as RotatableApparitionProjectile;
            projectile.Initialize(data.ProjectileData, direction, owner, player.gameObject);
        }
    }
}