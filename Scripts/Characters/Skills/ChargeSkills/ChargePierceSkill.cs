using System;
using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class ChargePierceSkill : ChargeSkill
    {
        //TODO: Move this data to player progression manager once it is implemented.
        [SerializeField] private ChargePierceSkillData data;

        public override void Initialize(SkillData _data)
        {
            element = data.ProjectileData.Damage.Element;

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
            var projectileData = SetupProjectileData(new ProjectileData(data.ProjectileData), chargeTime);

            var direction = GetCameraDirection();
            var projectile = projectilePool.Rent(transform.position + transform.forward, Quaternion.identity);
            projectile.Initialize(projectileData, direction, skillData.Player);
        }

        protected override ProjectileData SetupProjectileData(ProjectileData projectileData, float chargeTime)
        {
            var progress = chargeTime / data.ChargeLimit;
            projectileData.Speed = data.Speed.Initial + (data.Speed.Max - data.Speed.Initial) * progress;
            projectileData.Damage.Amount = data.DamageAmount.Initial + (int)((data.DamageAmount.Max - data.DamageAmount.Initial) * progress);
            // projectileData.Pierce = data.Pierce.Initial + (int)((data.Pierce.Max - data.Pierce.Initial) * progress);

            return projectileData;
        }
    }

    [Serializable]
    public class ChargePierceSkillData
    {
        public Projectile ProjectilePrefab;
        public ProjectileData ProjectileData;
        public float Downtime = 1f;

        public float ChargeLimit = 4f;
        public ScalableVariable<int> DamageAmount;
        public ScalableVariable<float> Speed;
        public ScalableVariable<int> Pierce;
    }

}
