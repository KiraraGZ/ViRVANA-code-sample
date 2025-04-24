using System;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Angel
{
    [Serializable]
    public class AngelGateSkill
    {
        [SerializeField] protected AngelGateSkillData data;

        protected BaseAngel baseAngel;
        protected ProjectilePoolManager projectilePool;

        protected int projectileAmount;
        protected float nextCastTime;

        public void Initialize(BaseAngel _baseAngel)
        {
            baseAngel = _baseAngel;

            projectilePool = new(data.ProjectilePrefab);

            projectileAmount = data.Amount;
            nextCastTime = Time.time + data.Cooldown;
        }

        public void Dispose()
        {
            baseAngel = null;

            projectilePool.Dispose();
            projectilePool = null;
        }

        public virtual void PhysicsUpdate()
        {
            if (Time.time < nextCastTime) return;

            Fire();

            nextCastTime = Time.time + (projectileAmount > 1 ? data.Interval : data.Cooldown);
            projectileAmount = projectileAmount > 1 ? projectileAmount - 1 : data.Amount;
        }

        protected void Fire()
        {
            Vector3 spawnPosition = RandomSpawnPosition(15f, new Vector3(0, 20f, 0));
            Vector3 direction = baseAngel.Player.transform.position - spawnPosition;

            var gateProjectile = projectilePool.Rent(spawnPosition, Quaternion.Euler(direction)) as AngelGateProjectile;
            gateProjectile.Initialize(data.GateProjectileData, direction, baseAngel, baseAngel.Player);
        }

        private Vector3 RandomSpawnPosition(float radius, Vector3 offset)
        {
            float angle = Random.Range(0f, 360f);
            return baseAngel.transform.position + offset +
                   radius * Mathf.Cos(angle) * baseAngel.transform.forward +
                   radius * Mathf.Sin(angle) * baseAngel.transform.right;
        }
    }

    [Serializable]
    public class AngelGateSkillData
    {
        public Projectile ProjectilePrefab;
        public AngelGateProjectileData GateProjectileData;
        public int Amount;
        public float Interval;
        public float Cooldown;
    }
}