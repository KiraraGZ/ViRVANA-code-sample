using UnityEngine;
using Magia.Projectiles;
using System;

namespace Magia.Skills
{
    public class SupportExplodeSkill : SupportSkill
    {
        [SerializeField] protected SupportExplodeSkillData data;
        private ProjectilePoolManager projectilePool;

        public override void Initialize(SupportSkillData _data)
        {
            base.Initialize(_data);

            projectilePool = new(data.ProjectilePrefab);
        }

        public override void Dispose()
        {
            base.Dispose();

            projectilePool.Dispose();
            projectilePool = null;
        }

        public override void PerformSkill()
        {
            ShootProjectile();

            base.PerformSkill();
        }

        protected void ShootProjectile()
        {
            var explodeProjectileData = data.ExplodeProjectileData;

            var direction = GetCameraDirection();
            var projectile = projectilePool.Rent(transform.position + transform.forward, Quaternion.identity) as ExplodeProjectile;
            projectile.Initialize(explodeProjectileData, direction, skillData.Player);
        }
    }

    [Serializable]
    public class SupportExplodeSkillData
    {
        public ExplodeProjectile ProjectilePrefab;
        public ExplodeProjectileData ExplodeProjectileData;
    }
}