using System;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class TracetSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private TracetSkillData data;

        private float nextFireTime;

        private BaseEnemy owner;
        private ProjectilePoolManager ProjectilePool;

        public void Initialize(TracetSkillData _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            nextFireTime = Time.time + data.Downtime;

            ProjectilePool = new(data.HomingProjectilePrefab);
        }

        public void Dispose()
        {
            owner = null;

            if (ProjectilePool != null)
            {
                ProjectilePool.Dispose();
                ProjectilePool = null;
            }
        }

        public void UpdateLogic()
        {

        }

        public bool IsAvailable()
        {
            return Time.time > nextFireTime;
        }

        public void Cast()
        {
            float totalSpread = (data.Amount - 1) * data.DegreeInterval;
            float startDegree = -totalSpread / 2f;
            Vector3 forward = owner.transform.forward;

            for (int i = 0; i != data.Amount; i++)
            {
                float degree = startDegree + (i * data.DegreeInterval);
                Vector3 offset = CalculateOffset(degree, forward);
                Vector3 initialPosition = owner.transform.position + offset;
                Vector3 direction = offset.normalized;

                var projectile = ProjectilePool.Rent(initialPosition, Quaternion.LookRotation(direction)) as HomingProjectile;
                projectile.Initialize(data.HomingProjectileData, direction, owner, owner.Player.gameObject);
            }

            nextFireTime = Time.time + data.Downtime;
            EventSkillEnd?.Invoke();
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
    public class TracetSkillData
    {
        public HomingProjectile HomingProjectilePrefab;
        public HomingProjectileData HomingProjectileData;
        public int Amount = 3;
        public float DegreeInterval = 30f;
        public float DistanceFromCenter = 7f;
        public float Downtime = 5f;
    }
}