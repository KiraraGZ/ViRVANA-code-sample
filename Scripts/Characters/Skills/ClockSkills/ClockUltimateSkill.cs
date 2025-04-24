using System;
using System.Collections;
using Magia.Enemy;
using Magia.Projectiles;
using Magia.UI.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Skills
{
    public class ClockUltimateSkill : ClockSkill
    {
        [SerializeField] protected UltimateClockSkillData extra;

        private ProjectilePoolManager extraPool;

        private UIUltimateSkillIcon SkillIcon => skillData.Icon as UIUltimateSkillIcon;

        public override void Initialize(SkillData _data)
        {
            base.Initialize(_data);

            extraPool = new(extra.ProjectilePrefab);
        }

        public override void Dispose()
        {
            base.Dispose();

            extraPool.Dispose();
            extraPool = null;
        }

        protected override void FireSequences()
        {
            var extraAmount = energy - energyData.SkillCost;

            SpendEnergy(energy);

            foreach (var spawn in data.ProjectileSpawns)
            {
                StartCoroutine(FireSequence(data, spawn, projectilePool));
            }

            StartCoroutine(FireExtraSequence(extraAmount));
        }

        protected override void UpdateEnergy()
        {
            base.UpdateEnergy();

            if (!isEquipped) return;

            SkillIcon.UpdateEnergy(energy, energyData.SkillCost, energyData.MaxEnergy);

            if (isSkillPerformed) return;

            bool isActive = energy >= energyData.SkillCost;
            SkillIcon.UpdateActive(isActive);
        }

        protected IEnumerator FireExtraSequence(float amount)
        {
            BaseEnemy[] targets = GetCameraClosestEnemies(extra.TargetCount);

            for (int i = 0; i < amount; i++)
            {
                Vector3 spawnPosition = GetSpawnPosition(i);
                Vector3 direction = GetRandomDirection(i);
                GameObject target = targets.Length > 0 ? targets[i % targets.Length].gameObject : null;

                var projectile = extraPool.Rent(spawnPosition, Quaternion.LookRotation(direction)) as HomingProjectile;
                projectile.Initialize(extra.HomingData, direction, skillData.Player, target);

                yield return new WaitForSeconds(extra.Interval);
            }
        }

        private Vector3 GetSpawnPosition(int index)
        {
            return transform.position +
                   extra.Offset.z * transform.forward +
                   extra.Offset.x * (index % 2 == 0 ? 1 : -1) * transform.right +
                   extra.Offset.y * transform.up;
        }

        private Vector3 GetRandomDirection(int index)
        {
            float forwardAngle = Random.Range(-extra.Angle, extra.Angle);
            float upAngle = Random.Range(-extra.Angle, extra.Angle);
            Quaternion randomRotation = Quaternion.Euler(upAngle, forwardAngle, 0);

            return randomRotation * (index % 2 == 0 ? Vector3.right : Vector3.left);
        }
    }

    [Serializable]
    public class UltimateClockSkillData
    {
        public HomingProjectile ProjectilePrefab;
        public HomingProjectileData HomingData;
        public int TargetCount = 6;
        public float Interval = 0.1f;
        public Vector3 Offset;
        public float Angle = 60f;
    }
}
