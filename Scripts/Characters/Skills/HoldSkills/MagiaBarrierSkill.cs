using System.Collections.Generic;
using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class MagiaBarrierSkill : BarrierSkill
    {
        [Header("Deflect")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private ProjectileData projectileData;

        private List<Vector3> fireDirections;

        private ProjectilePoolManager projectilePool;

        public override void Initialize(SkillData _data)
        {
            base.Initialize(_data);

            element = ElementType.Magia;
            projectilePool = new(projectilePrefab);
            fireDirections = new();
        }

        public override void Dispose()
        {
            base.Dispose();

            projectilePool.Dispose();
            projectilePool = null;
            fireDirections = null;
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();

            foreach (var direction in fireDirections)
            {
                var projectile = projectilePool.Rent(skillData.Player.transform.position, Quaternion.LookRotation(direction));
                projectile.Initialize(projectileData, direction, skillData.Player);
            }

            fireDirections.Clear();
        }

        public override bool CheckBarrier(Damage damage, Vector3 direction)
        {
            if (!isSkillPerformed) return false;

            if (damage.Type == DamageType.Projectile)
            {
                fireDirections.Add(-direction);
            }

            return base.CheckBarrier(damage, direction);
        }
    }
}
