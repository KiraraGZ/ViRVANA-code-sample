using System;
using System.Collections;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.Mark;
using Magia.Projectiles;
using Magia.Vfx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Skills
{
    public class ClockPrimarySkill : EnergySkill
    {
        public event Action<int> EventSkillRewardStack;

        [SerializeField] protected PrimaryClockSkillData data;
        private ClockSkillUpgradeData upgradeData;

        [SerializeField] private AudioClip firstTick;
        [SerializeField] private AudioClip secondTick;

        protected ProjectilePoolManager projectilePool;
        protected MarkPoolManager markPool;
        protected Dictionary<ClockStarVfx, Vector3> gates;
        private float nextCooldown;

        public override void Initialize(SkillData _data)
        {
            base.Initialize(_data);

            element = data.ProjectileData.ProjectileData.Damage.Element;

            projectilePool = new(data.ProjectilePrefab);
            projectilePool.EventProjectileHit += OnDamageDealt;
            projectilePool.EventProjectileHitEnemy += OnProjectileHitEnemy;

            markPool = new(data.MarkPrefab);
            markPool.EventResolveEffectOnEnemy += OnMarkResolved;

            gates = new();
        }

        public void ApplyUpgrade(ClockSkillUpgradeData _upgradeData)
        {
            upgradeData = _upgradeData;

            energy += upgradeData.InitialEnergy;
            energyData.MaxEnergy = upgradeData.MaxEnergy;
        }

        public override void Dispose()
        {
            base.Dispose();

            projectilePool.EventProjectileHit -= OnDamageDealt;
            projectilePool.EventProjectileHitEnemy -= OnProjectileHitEnemy;
            projectilePool = null;

            markPool.EventResolveEffectOnEnemy -= OnMarkResolved;
            markPool = null;

            gates = null;
        }

        public override bool IsSkillAvailable()
        {
            if (Time.time < availableTime) return false;

            if (skillData.Player.ReusableData.IsDash) return upgradeData.OnDash;

            return true;
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

            foreach (var pair in gates)
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

        protected void FireSequences()
        {
            if (skillData.Player.ReusableData.IsDash)
            {
                if (skillData.Player.ReusableData.DashDirection == Vector2.down && upgradeData.BackwardDash)
                {
                    StartCoroutine(BackwardDashFireSequence(data, projectilePool));
                }
                else
                {
                    StartCoroutine(DashFireSequence(data, projectilePool));
                }

                nextCooldown = cooldown * upgradeData.DashCooldownMultiplier;
                return;
            }

            StartCoroutine(ChargeSequence(data, data.ProjectileSpawn, projectilePool));
            nextCooldown = cooldown;
        }

        protected IEnumerator ChargeSequence(PrimaryClockSkillData skill, ClockSkillSpawnData spawn, ProjectilePoolManager pool)
        {
            Vector3[] spawnsPos = PhysicsHelper.GetCircularPositions(data.MaxAmount,
                                                                     2,
                                                                     data.ProjectileSpawn.Radius,
                                                                     data.ProjectileSpawn.Angle);

            while (isSkillPerformed || gates.Count < data.MinAmount)
            {
                if (gates.Count >= data.MinAmount)
                {
                    SpendEnergy(energyData.SkillCost);
                }

                int index = gates.Count;
                spawnsPos[index] += transform.forward * spawn.Offset.z +
                                    transform.right * spawn.Offset.x +
                                    transform.up * spawn.Offset.y;
                var star = data.StarPrefab.Rent(spawnsPos[index], Quaternion.identity, transform);
                star.Initialize();
                gates.Add(star, star.transform.localPosition);

                if (gates.Count == 3)
                {
                    PlaySound(firstTick);
                }
                if (gates.Count == 6)
                {
                    PlaySound(secondTick);
                }

                if (gates.Count >= data.MaxAmount) break;
                if (gates.Count >= data.MinAmount && energy < energyData.SkillCost) break;

                yield return new WaitForSeconds(upgradeData.ChargeInterval);
            }

            yield return new WaitForSeconds(spawn.FireDelay);
            yield return new WaitUntil(() => !isSkillPerformed);

            BaseEnemy[] targets = GetCameraClosestEnemies(data.TargetCount);

            foreach (var pair in gates)
            {
                int index = 0;
                var projectile = pool.Rent(pair.Key.transform.position, Quaternion.identity) as HomingProjectile;
                //TODO: make direction spreading apart.
                var direction = targets.Length == 0 ? GetCameraDirection() :
                                                      (targets[index % targets.Length].transform.position - projectile.transform.position).normalized;
                direction = PhysicsHelper.RandomizeDirectionInCone(direction, 30f);
                var target = targets.Length == 0 ? null : targets[index % targets.Length].gameObject;

                projectile.Initialize(skill.ProjectileData, direction, skillData.Player, target);

                pair.Key.Fire();
                index++;
                yield return new WaitForSeconds(spawn.Interval);
            }

            gates.Clear();

            if (gates.Count >= data.MaxAmount)
            {
                RecoverEnergy(upgradeData.MaxChargeRefund);
            }

            StartCoroutine(Downtime(data.Downtime));
        }

        private IEnumerator DashFireSequence(PrimaryClockSkillData skill, ProjectilePoolManager pool)
        {
            //TODO: maybe make it target closet enemy around player.
            int amount = 2;
            Vector3[] spawnsPos = PhysicsHelper.GetCircularPositions(amount,
                                                                     1,
                                                                     data.ProjectileSpawn.Radius / 2,
                                                                     Random.Range(0, 180));
            BaseEnemy[] targets = GetCameraClosestEnemies(amount);

            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < amount; i++)
            {
                SpawnStar(spawnsPos[i]);
                spawnsPos[i] += skillData.Player.transform.position;
                var projectile = pool.Rent(spawnsPos[i], Quaternion.identity) as HomingProjectile;
                var direction = PhysicsHelper.RandomizeDirectionInCone(GetCameraDirection(), 30f);
                var target = targets.Length == 0 ? null : targets[i % targets.Length].gameObject;

                projectile.Initialize(skill.ProjectileData, direction, skillData.Player, target);
            }

            StartCoroutine(Downtime(data.Downtime));
        }

        private IEnumerator BackwardDashFireSequence(PrimaryClockSkillData skill, ProjectilePoolManager pool)
        {
            int amount = data.MinAmount;
            int extra = Math.Min(energy / energyData.SkillCost, data.MaxAmount - data.MinAmount);
            SpendEnergy(energyData.SkillCost * extra);
            amount += extra;

            Vector3[] spawnsPos = PhysicsHelper.GetCircularPositions(amount,
                                                                     1,
                                                                     data.ProjectileSpawn.Radius / 2,
                                                                     Random.Range(0, 180));
            BaseEnemy[] targets = GetCameraClosestEnemies(amount);

            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < amount; i++)
            {
                SpawnStar(spawnsPos[i]);
                spawnsPos[i] += skillData.Player.transform.position;
                var projectile = pool.Rent(spawnsPos[i], Quaternion.identity) as HomingProjectile;
                var direction = PhysicsHelper.RandomizeDirectionInCone(GetCameraDirection(), 30f);
                var target = targets.Length == 0 ? null : targets[i % targets.Length].gameObject;

                projectile.Initialize(skill.ProjectileData, direction, skillData.Player, target);
            }

            StartCoroutine(Downtime(data.Downtime));
        }

        private void SpawnStar(Vector3 spawnPos)
        {
            var star = data.StarPrefab.Rent(spawnPos, Quaternion.identity, transform);
            star.Initialize();
            star.Fire();
        }

        protected override float GetCooldown()
        {
            return nextCooldown;
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

            VfxManager.Instance.RentOrb(enemy.transform.position, skillData.Player.transform);
        }

        private void OnMarkResolved(BaseEnemy enemy, ElementType element)
        {
            EventSkillRewardStack?.Invoke(upgradeData.MarkResolveStack);

            for (int i = 0; i < upgradeData.MarkResolveStack; i++)
            {
                VfxManager.Instance.RentOrb(enemy.transform.position, skillData.Player.transform);
            }
        }
        #endregion

        protected void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            skillData.Player.AnimatorController.PlayOneShotSound(clip);
        }
    }

    [Serializable]
    public class ClockSkillUpgradeData
    {
        public float ChargeInterval;
        public int InitialEnergy;
        public int MaxEnergy;
        public int MaxChargeRefund;

        public bool OnDash;
        public bool BackwardDash;
        public float DashCooldownMultiplier;

        public int MarkResolveStack;
    }

    [Serializable]
    public class PrimaryClockSkillData
    {
        public ClockStarVfx StarPrefab;
        public HomingProjectile ProjectilePrefab;
        public HomingProjectileData ProjectileData;
        public BuddyMark MarkPrefab;
        public BuddyMarkData MarkData;
        public int MinAmount = 3;
        public int MaxAmount = 6;
        public float Downtime = 1f;
        public int TargetCount = 3;

        public ClockSkillSpawnData ProjectileSpawn;
    }
}
