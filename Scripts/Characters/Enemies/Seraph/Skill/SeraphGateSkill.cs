using System;
using Magia.Enemy.Angel;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Seraph
{
    [Serializable]
    public class SeraphGateSkill
    {
        [SerializeField] protected SeraphGateSkillData data;

        protected BaseSeraph baseSeraph;
        protected PlayerController player;
        protected ProjectilePoolManager projectilePool;

        protected int projectileAmount;
        protected float currentRadius;
        protected float nextCastTime;

        public void Initialize(BaseSeraph _baseAngel, PlayerController _player)
        {
            baseSeraph = _baseAngel;
            player = _player;

            projectilePool = new(data.ProjectilePrefab);

            projectileAmount = data.Amount;
            currentRadius = data.InitialRadius;
            nextCastTime = Time.time + UnityEngine.Random.Range(data.MinimumSkillCooldown, data.MaximumSkillCooldown);
        }

        public void Dispose()
        {
            baseSeraph = null;
            player = null;

            projectilePool.Dispose();
            projectilePool = null;
        }

        public bool IsAvailable()
        {
            return Time.time > nextCastTime;
        }

        public virtual void PhysicsUpdate()
        {
            Fire();
            nextCastTime = Time.time + UnityEngine.Random.Range(data.MinimumSkillCooldown, data.MaximumSkillCooldown);
        }

        protected void Fire()
        {
            Vector3 spawnPosition = RandomSpawnPosition(currentRadius, new Vector3(0, 20f, 0));
            Vector3 direction = baseSeraph.Player.transform.position - spawnPosition;

            var gateProjectile = projectilePool.Rent(spawnPosition, Quaternion.Euler(direction)) as AngelGateProjectile;
            gateProjectile.Initialize(data.GateProjectileData, direction, baseSeraph, baseSeraph.Player);
        }

        private Vector3 RandomSpawnPosition(float radius, Vector3 offset)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            return baseSeraph.transform.position + offset +
                   radius * Mathf.Cos(angle) * baseSeraph.transform.forward +
                   radius * Mathf.Sin(angle) * baseSeraph.transform.right;
        }
    }

    [Serializable]
    public class SeraphGateSkillData
    {
        public Projectile ProjectilePrefab;
        public AngelGateProjectileData GateProjectileData;
        public int Amount;
        public float InitialRadius;
        public float MinimumSkillCooldown;
        public float MaximumSkillCooldown;
    }
}