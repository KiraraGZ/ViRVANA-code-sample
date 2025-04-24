using System;
using Magia.Buddy.Data;
using Magia.Enemy;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Buddy
{
    public class BuddySkillHandler : MonoBehaviour
    {
        public event Action EventUseAttack;
        public event Action EventUseMark;

        [SerializeField] private BuddyMarkSkill markSkill;
        [SerializeField] private Transform firePoint;

        private BuddyCombatData data;
        private BuddyController buddy;
        private ProjectilePoolManager projectilePool;

        public void Initialize(BuddyController _buddy)
        {
            buddy = _buddy;
            data = _buddy.Data.CombatData;

            projectilePool = new ProjectilePoolManager(data.ProjectilePrefab);

            markSkill.Initialize(buddy);
        }

        public void Dispose()
        {
            projectilePool.Dispose();
            projectilePool = null;

            markSkill.Dispose();
        }

        public void CastSkill(BaseEnemy target)
        {
            if (markSkill.IsAvailable())
            {
                markSkill.MarkEnemy(target);
                EventUseMark?.Invoke();

                return;
            }

            FireProjectile(target);
        }

        private void FireProjectile(BaseEnemy target)
        {
            Vector3 directionToEnemy = target.transform.position - firePoint.position;

            var projectile = projectilePool.Rent(firePoint.position, firePoint.rotation);
            projectile.Initialize(data.ProjectileData, directionToEnemy, buddy);

            EventUseAttack?.Invoke();
        }
    }
}