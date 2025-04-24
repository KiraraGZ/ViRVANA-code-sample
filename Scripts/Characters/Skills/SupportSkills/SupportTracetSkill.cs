using UnityEngine;
using Magia.Projectiles;
using System;

namespace Magia.Skills
{
    public class SupportTracetSkill : SupportSkill
    {
        [SerializeField] protected SupportTracetSkillData data;
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
            float totalSpread = (data.Amount - 1) * data.DegreeInterval;
            float startDegree = -totalSpread / 2f;
            Vector3 forward = transform.forward;

            for (int i = 0; i != data.Amount; i++)
            {
                float degree = startDegree + (i * data.DegreeInterval);
                Vector3 offset = CalculateOffset(degree, forward);
                Vector3 initialPosition = transform.position + offset;
                Vector3 direction = offset.normalized;

                var projectile = projectilePool.Rent(initialPosition, Quaternion.LookRotation(direction)) as AngularProjectile;
                projectile.Initialize(data.ProjectileData, direction, skillData.Player, new Vector3(0, (data.Amount / 2) - i, 0));
            }
        }

        Vector3 CalculateOffset(float degree, Vector3 forward)
        {
            float radian = degree * Mathf.Deg2Rad;

            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 offset = forward * Mathf.Cos(radian) * data.DistanceFromCenter +
                     right * Mathf.Sin(radian) * data.DistanceFromCenter;
            return offset;
        }
    }

    [Serializable]
    public class SupportTracetSkillData
    {
        public AngularProjectile ProjectilePrefab;
        public AngularProjectileData ProjectileData;
        public int Amount = 3;
        public float DegreeInterval = 30f;
        public float DistanceFromCenter = 7f;
    }
}