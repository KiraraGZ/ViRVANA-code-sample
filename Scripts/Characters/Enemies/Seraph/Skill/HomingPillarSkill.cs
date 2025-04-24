using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Magia.Enemy.Skills
{
    public class HomingPillarSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private HomingPillarSkillData data;

        private float nextAvailableTime;

        private float projectileBaseSpeedFirstValue;
        private float projectileBaseSpeed;
        private float projectileBaseDistance;
        private float projectileUnconstantDuration;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;

        private ProjectilePoolManager projectilePool;

        public void Initialize(HomingPillarSkillData _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            projectilePool = new(data.Projectile);
            projectilePool.EventProjectileExpire += OnProjectileExpire;
            nextAvailableTime = Time.time + data.MinimumDowntime;

            projectileBaseSpeedFirstValue = data.AngelPillarData.ProjectileData.SpeedModifier[0].value;
            projectileBaseSpeed = data.AngelPillarData.ProjectileData.Speed;
            projectileUnconstantDuration = data.AngelPillarData.ProjectileData.Lifetime * data.AngelPillarData.ProjectileData.SpeedModifier.keys[1].value;
            projectileBaseDistance = data.AngelPillarData.ProjectileData.Speed * data.AngelPillarData.ProjectileData.Lifetime;
            projectileBaseDistance += 0.5f * projectileUnconstantDuration * (data.AngelPillarData.ProjectileData.SpeedModifier[0].value - data.AngelPillarData.ProjectileData.SpeedModifier[1].value) * data.AngelPillarData.ProjectileData.Speed;
        }

        public void Dispose()
        {
            data = null;
            owner = null;

            projectilePool.Dispose();
            projectilePool.EventProjectileExpire -= OnProjectileExpire;
            projectilePool = null;
        }

        public bool IsAvailable()
        {
            if (data.IsLoop) return false;

            return Time.time >= nextAvailableTime;
        }

        public void Cast()
        {
            nextAvailableTime = Time.time + data.AngelPillarData.ProjectileData.Lifetime + Random.Range(data.MinimumDowntime, data.MaximumDowntime);
            SpawnPillar();
        }

        public void UpdateLogic()
        {

        }

        private void SpawnPillar()
        {
            Vector3 distanceToPlayer = (player.transform.position - owner.transform.position).normalized;
            Vector3 spawnPos = owner.transform.position + (distanceToPlayer * data.SpawnDistance);
            SetPillarSpeed();

            var projectile = projectilePool.Rent(spawnPos, Quaternion.Euler(Vector3.down)) as AngelPillar;
            projectile.Initialize(data.AngelPillarData, player.transform, owner);
            projectile.transform.position = new Vector3(projectile.transform.position.x, -80f, projectile.transform.position.z);
        }

        private void SetPillarSpeed()
        {
            float distanceToPlayer = new Vector3(owner.transform.position.x - player.transform.position.x, 0, owner.transform.position.z - player.transform.position.z).magnitude;
            if (distanceToPlayer < projectileBaseDistance)
            {
                data.AngelPillarData.ProjectileData.SpeedModifier.MoveKey(0, new Keyframe(0, projectileBaseSpeedFirstValue));
            }
            else
            {
                float addictionDistance = distanceToPlayer - projectileBaseDistance;
                data.AngelPillarData.ProjectileData.SpeedModifier.MoveKey(0, new Keyframe(0, 2 * addictionDistance / (projectileUnconstantDuration * projectileBaseSpeed) + 1));
            }
        }

        private void OnProjectileExpire()
        {
            EventSkillEnd?.Invoke();
        }
    }

    [Serializable]
    public class HomingPillarSkillData
    {
        public AngelPillar Projectile;
        public AngelPillarData AngelPillarData;

        public float SpawnDistance = 12f;
        public float MinimumDowntime = 6f;
        public float MaximumDowntime = 8f;

        public bool IsLoop = false;
    }
}