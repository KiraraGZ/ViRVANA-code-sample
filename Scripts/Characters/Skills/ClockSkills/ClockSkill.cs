using System;
using System.Collections;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.Mark;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class ClockSkill : EnergySkill
    {
        public event Action<int> EventSkillRewardStack;

        [SerializeField] protected ClockSkillData data;

        protected ProjectilePoolManager projectilePool;
        protected Dictionary<HomingProjectile, Vector3> projectileDict;
        protected MarkPoolManager markPool;

        public override void Initialize(SkillData _data)
        {
            base.Initialize(_data);

            element = data.ProjectileData.ProjectileData.Damage.Element;

            projectilePool = new(data.ProjectilePrefab);
            projectilePool.EventProjectileHit += OnDamageDealt;
            projectilePool.EventProjectileHitEnemy += OnProjectileHitEnemy;
            projectileDict = new();

            markPool = new(data.MarkPrefab);
            markPool.EventResolveEffectOnEnemy += OnMarkResolved;
        }

        public override void Dispose()
        {
            base.Dispose();

            projectilePool.Dispose();
            projectilePool.EventProjectileHit -= OnDamageDealt;
            projectilePool.EventProjectileHitEnemy -= OnProjectileHitEnemy;
            projectilePool = null;
            projectileDict = null;

            markPool.Dispose();
            markPool.EventResolveEffectOnEnemy -= OnMarkResolved;
            markPool = null;
        }

        public override bool IsSkillAvailable()
        {
            if (energy < energyData.SkillCost) return false;

            return Time.time >= availableTime;
        }

        public override void PerformSkill()
        {
            FireSequences();
            base.PerformSkill();
            return;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            foreach (var pair in projectileDict)
            {
                pair.Key.transform.localPosition = pair.Value;
            }
        }

        protected override void UpdateEnergy()
        {
            base.UpdateEnergy();

            if (isSkillPerformed) return;

            skillData.Icon.UpdateActive(energy >= energyData.SkillCost);
        }

        protected virtual void FireSequences()
        {
            SpendEnergy(energyData.SkillCost);

            foreach (var spawn in data.ProjectileSpawns)
            {
                StartCoroutine(FireSequence(data, spawn, projectilePool));
            }
        }

        protected IEnumerator FireSequence(ClockSkillData skill, ClockSkillSpawnData spawn, ProjectilePoolManager pool)
        {
            var projectiles = new HomingProjectile[spawn.Amount];

            for (int i = 0; i < spawn.Amount; i++)
            {
                Vector3 pos = transform.forward * spawn.Offset.z + transform.right * spawn.Offset.x + transform.up * spawn.Offset.y;
                var spawnPosition = GetPositionInCircle(i, spawn, pos);

                projectiles[i] = pool.Rent(spawnPosition, Quaternion.identity, transform) as HomingProjectile;
                projectileDict.Add(projectiles[i], projectiles[i].transform.localPosition);

                yield return new WaitForSeconds(spawn.Interval);
            }

            yield return new WaitForSeconds(spawn.FireDelay);
            yield return new WaitUntil(() => !isSkillPerformed);

            BaseEnemy[] targets = GetCameraClosestEnemies(data.TargetCount);

            for (int i = 0; i < spawn.Amount; i++)
            {
                projectileDict.Remove(projectiles[i]);
                var projectile = projectiles[i];
                var direction = targets.Length == 0 ? GetCameraDirection() :
                                                      (targets[i % targets.Length].transform.position - projectile.transform.position).normalized;
                direction = PhysicsHelper.RandomizeDirectionInCone(direction, 30f);
                var target = targets.Length == 0 ? null : targets[i % targets.Length].gameObject;

                projectile.transform.parent = null;
                projectile.Initialize(skill.ProjectileData, direction, skillData.Player, target);

                yield return new WaitForSeconds(spawn.Interval);
            }

            StartCoroutine(Downtime(data.Downtime));
        }

        protected Vector3 GetPositionInCircle(int index, ClockSkillSpawnData spawnData, Vector3 offset)
        {
            float angle = index * Mathf.PI * 2 / spawnData.Amount + Mathf.Abs(spawnData.Angle);
            return transform.position + offset +
                   spawnData.Radius * Mathf.Cos(angle) * Vector3.up +
                   spawnData.Radius * Mathf.Sin(spawnData.Angle > 0 ? angle : -angle) * transform.right;
        }

        #region subscribe events
        protected override void OnDamageDealt(DamageFeedback feedback, Vector3 hitDir)
        {
            base.OnDamageDealt(feedback, hitDir);
            EventSkillRewardStack?.Invoke(1);
        }

        protected void OnProjectileHitEnemy(BaseEnemy enemy)
        {
            if (enemy.Mark != null) return;

            var mark = markPool.Rent(enemy.transform);
            enemy.ApplyMark(mark);
            mark.Initialize(data.MarkData, enemy);
        }

        private void OnMarkResolved(BaseEnemy enemy, ElementType element)
        {
            EventSkillRewardStack?.Invoke(1);
        }
        #endregion
    }

    [Serializable]
    public class ClockSkillData
    {
        public HomingProjectile ProjectilePrefab;
        public HomingProjectileData ProjectileData;
        public BuddyMark MarkPrefab;
        public BuddyMarkData MarkData;
        public float Downtime = 1f;
        public int TargetCount = 3;

        public ClockSkillSpawnData[] ProjectileSpawns;
    }

    [Serializable]
    public class ClockSkillSpawnData
    {
        public int Amount = 6;
        public float Radius = 2f;
        public float Angle = 0f;
        public Vector3 Offset;
        public float Interval = 0.1f;
        public float FireDelay = 0.2f;
    }
}
