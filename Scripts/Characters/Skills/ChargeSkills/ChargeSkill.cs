using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public abstract class ChargeSkill : EnergySkill
    {
        protected ProjectilePoolManager projectilePool;
        protected float chargeTime;

        public override void Dispose()
        {
            base.Dispose();

            StopAllCoroutines();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            chargeTime += Time.deltaTime;
        }

        public override void PerformSkill()
        {
            base.PerformSkill();

            chargeTime = 0;
        }

        protected abstract void ShootProjectile();

        protected abstract ProjectileData SetupProjectileData(ProjectileData projectileData, float chargeTime);
    }
}
