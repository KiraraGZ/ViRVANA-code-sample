using System;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class GatlingSkill : EnergySkill
    {
        //TODO: Move this data to player progression manager once it is implemented.
        [SerializeField] private GatlingSkillData data;

        private ProjectilePoolManager projectilePool;
        private float lastFireTime;

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
        }

        public override void PerformSkill()
        {
            base.PerformSkill();
        }

        public override void RepeatSkill()
        {
            base.RepeatSkill();
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();

            StartCoroutine(Downtime(data.Downtime));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            skillData.Crosshair.UpdateLogic((float)energy / energyData.MaxEnergy);

            if (lastFireTime > Time.time) return;

            SpendEnergy(data.SkillCostPerHit);
            Fire();
            lastFireTime = Time.time + data.Interval;

            if (energy > data.SkillCostPerHit) return;

            ReleaseSkill();
        }

        private void Fire()
        {
            var direction = GetCameraDirection();
            var projectile = projectilePool.Rent(transform.position, Quaternion.LookRotation(direction));
            projectile.Initialize(data.ProjectileData, direction, skillData.Player);
        }
    }

    [Serializable]
    public class GatlingSkillData
    {
        public Projectile ProjectilePrefab;

        public int InitialCost = 10;
        public int SkillCostPerHit = 1;
        public float Downtime = 0.5f;

        [Header("Normal")]
        public ProjectileData ProjectileData;
        public float Interval = 0.1f;
        public float Radius = 0.5f;
    }
}
