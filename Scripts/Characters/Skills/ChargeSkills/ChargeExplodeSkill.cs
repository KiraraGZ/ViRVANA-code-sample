using System;
using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class ChargeExplodeSkill : ChargeSkill
    {
        //TODO: Move this data to player progression manager once it is implemented.
        [SerializeField] private ChargeExplodeSkillData data;

        public override void Initialize(SkillData _data)
        {
            element = data.ExplodeProjectileData.ProjectileData.Damage.Element;

            base.Initialize(_data);

            projectilePool = new(data.ProjectilePrefab);
        }

        public override void Dispose()
        {
            base.Dispose();

            projectilePool.Dispose();
            projectilePool = null;

            StopAllCoroutines();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            skillData.Crosshair.UpdateLogic(chargeTime / data.ChargeLimit);
        }

        public override void PerformSkill()
        {
            base.PerformSkill();

            SpendEnergy(energyData.SkillCost);
        }

        public override void ReleaseSkill()
        {
            //TODO: Find some way to check if the skill is still on downtime and prevent this from being called.

            base.ReleaseSkill();

            ShootProjectile();

            StartCoroutine(Downtime(data.Downtime));
        }

        protected override void ShootProjectile()
        {
            var projectileData = SetupProjectileData(new(data.ExplodeProjectileData.ProjectileData), chargeTime);
            var explodeProjectileData = new ExplodeProjectileData(data.ExplodeProjectileData)
            {
                ProjectileData = projectileData
            };

            var direction = GetCameraDirection();
            var projectile = projectilePool.Rent(transform.position + transform.forward, Quaternion.identity) as ExplodeProjectile;
            projectile.Initialize(explodeProjectileData, direction, skillData.Player);
        }

        protected override ProjectileData SetupProjectileData(ProjectileData projectileData, float chargeTime)
        {
            var progress = chargeTime / data.ChargeLimit;
            projectileData.Speed = data.Speed.Initial + (data.Speed.Max - data.Speed.Initial) * progress;
            projectileData.Damage.Amount = data.DamageAmount.Initial + (int)((data.DamageAmount.Max - data.DamageAmount.Initial) * progress);

            return projectileData;
        }
    }

    [Serializable]
    public class ChargeExplodeSkillData
    {
        public ExplodeProjectile ProjectilePrefab;
        public ExplodeProjectileData ExplodeProjectileData;
        public float Downtime = 1f;

        public float ChargeLimit = 4f;
        public ScalableVariable<int> DamageAmount;
        public ScalableVariable<float> Speed;
    }
}
