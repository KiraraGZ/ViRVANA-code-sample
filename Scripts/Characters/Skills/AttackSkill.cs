using System;
using System.Collections;
using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Magia.Skills
{
    public class AttackSkill : ActiveSkill
    {
        public event Action<AttackUpdateData> EventAttackComboUpdate;
        public event Action<int> EventSkillHit;

        private static readonly int[] ANIMATION_SEQUENCES = { 2, 0, 1, 0 };

        [SerializeField] private AttackSkillData attackData;
        private AttackSkillUpgradeData upgradeData;

        private ProjectilePoolManager projectilePool;
        private ProjectilePoolManager explodeProjectilePool;
        private Coroutine attackCoroutine;

        private int comboNumber;
        private float lastAttackTime;
        private bool isHold;

        public void Initialize(SkillData _data, AttackSkillUpgradeData _upgradeData)
        {
            upgradeData = _upgradeData;
            element = attackData.ProjectileData.Damage.Element;

            projectilePool = new(attackData.ProjectilePrefab);
            projectilePool.EventProjectileHit += OnDamageDealt;

            explodeProjectilePool = new(attackData.ExplodeProjectilePrefab);
            explodeProjectilePool.EventProjectileHit += OnDamageDealt;

            base.Initialize(_data);
        }

        public override void Dispose()
        {
            projectilePool.Dispose();
            projectilePool.EventProjectileHit -= OnDamageDealt;
            projectilePool = null;

            explodeProjectilePool.Dispose();
            explodeProjectilePool.EventProjectileHit -= OnDamageDealt;
            explodeProjectilePool = null;

            base.Dispose();
        }

        //TODO: stop using coroutine and fix hold attack timing.
        public override void PhysicsUpdate()
        {
            //this part is use for hold attak logic.
            if (isSkillPerformed && Time.time > lastAttackTime + attackData.BasicAttack.FireInterval * attackData.BasicAttack.FireAmount
                                                               + attackData.AttackCooldown
                                                               + attackData.HoldAttackAdditionalInterval)
            {
                PerformAttackPattern();
            }

            //this part is use for cancel attacking if player release attack key within attack cooldown interval.
            if (!isSkillPerformed && attackCoroutine == null && isHold)
            {
                StartCoroutine(Downtime(attackData.AttackCooldown));
                isHold = false;
                return;
            }

            if (comboNumber > 0 && Time.time > lastAttackTime + attackData.ComboLastingDuration)
            {
                comboNumber = 0;
                EventAttackComboUpdate?.Invoke(new(comboNumber, false));
            }

            base.PhysicsUpdate();
        }

        public override void PerformSkill()
        {
            base.PerformSkill();

            PerformAttackPattern();
            isHold = true;
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();
        }

        private void PerformAttackPattern()
        {
            lastAttackTime = Time.time;
            comboNumber += 1;

            if (comboNumber < 4)
            {
                EventAttackComboUpdate?.Invoke(new(comboNumber, true));
                attackCoroutine = StartCoroutine(ShootProjectiles(attackData.BasicAttack));
                return;
            }

            comboNumber = 0;
            EventAttackComboUpdate?.Invoke(new(comboNumber, true));

            float rand = Random.Range(0f, 1f);

            if (rand > upgradeData.ForthExplosiveChance)
            {
                attackCoroutine = StartCoroutine(ShootProjectiles(attackData.ForthAttack));
                return;
            }
            else
            {
                attackCoroutine = StartCoroutine(ShootExplosiveProjectile());
                return;
            }
        }

        private IEnumerator ShootProjectiles(FirePattern pattern)
        {
            skillData.Player.AnimatorController.StartBasicAttack(ANIMATION_SEQUENCES[comboNumber]);

            for (int i = 0; i < pattern.FireAmount; i++)
            {
                var centerDirection = GetCameraDirection();

                float angle = pattern.SpreadAngle * upgradeData.SpreadAngleMultiplier * (1 - pattern.SpreadAmount) / 2;

                for (int j = 0; j < pattern.SpreadAmount; j++)
                {
                    Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * centerDirection;
                    angle += pattern.SpreadAngle;

                    Projectile projectile = projectilePool.Rent(transform.position, Quaternion.LookRotation(spreadDirection));
                    projectile.Initialize(attackData.ProjectileData, spreadDirection, skillData.Player);
                }

                yield return new WaitForSeconds(pattern.FireInterval);
            }

            skillData.Icon.StartCooldown(attackData.AttackCooldown);

            if (isSkillPerformed == false)
            {
                StartCoroutine(Downtime(attackData.AttackCooldown));
            }

            attackCoroutine = null;
        }

        private IEnumerator ShootExplosiveProjectile()
        {
            skillData.Player.AnimatorController.StartBasicAttack(ANIMATION_SEQUENCES[comboNumber]);

            var centerDirection = GetCameraDirection();
            ExplodeProjectile projectile = explodeProjectilePool.Rent(transform.position, Quaternion.LookRotation(centerDirection)) as ExplodeProjectile;
            projectile.Initialize(attackData.ExplodeProjectileData, centerDirection, skillData.Player);

            yield return new WaitForSeconds(attackData.ForthAttack.FireInterval);

            skillData.Icon.StartCooldown(attackData.AttackCooldown);

            if (isSkillPerformed == false)
            {
                StartCoroutine(Downtime(attackData.AttackCooldown));
            }

            attackCoroutine = null;
        }

        #region subscribe events
        protected override void OnDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            base.OnDamageDealt(feedback, hitPos);
            EventSkillHit?.Invoke(attackData.EnergyRecoverPerHit);
        }

        protected override void OnSkillEnd()
        {
            base.OnSkillEnd();

            //TODO: find more proper way to stop attack animation after hold duration is end.
            skillData.Player.AnimatorController.StopBasicAttack();
            skillData.Icon.ReleaseSkill();
        }
        #endregion
    }

    [Serializable]
    public class AttackSkillData
    {
        public Projectile ProjectilePrefab;
        public ProjectileData ProjectileData;

        public ExplodeProjectile ExplodeProjectilePrefab;
        public ExplodeProjectileData ExplodeProjectileData;

        public float AttackCooldown = 0.2f;
        public float ComboLastingDuration = 1.5f;
        public int EnergyRecoverPerHit = 1;
        public float HoldAttackAdditionalInterval = 0.1f;

        public FirePattern BasicAttack;
        public FirePattern ForthAttack;

        public AttackSkillData(AttackSkillData data)
        {
            ProjectilePrefab = data.ProjectilePrefab;
            ProjectileData = new(data.ProjectileData);

            AttackCooldown = data.AttackCooldown;
            ComboLastingDuration = data.ComboLastingDuration;
            EnergyRecoverPerHit = data.EnergyRecoverPerHit;
            HoldAttackAdditionalInterval = data.HoldAttackAdditionalInterval;

            BasicAttack = new(data.BasicAttack);
            ForthAttack = new(data.ForthAttack);
        }
    }

    [Serializable]
    public class AttackSkillUpgradeData
    {
        public float ProjectileSpeed;
        public float ProjectileCritChance;
        public float SpreadAngleMultiplier;
        public int ForthAttackAmount;
        public float ForthExplosiveChance;
    }

    [Serializable]
    public class FirePattern
    {
        public int FireAmount = 2;
        public float FireInterval = 0.2f;
        public int SpreadAmount = 3;
        public float SpreadAngle = 15f;

        public FirePattern(FirePattern pattern)
        {
            FireAmount = pattern.FireAmount;
            FireInterval = pattern.FireInterval;
            SpreadAmount = pattern.SpreadAmount;
            SpreadAngle = pattern.SpreadAngle;
        }
    }

    public class AttackUpdateData
    {
        public int ComboNumber;
        public bool IsPerformed;

        public AttackUpdateData(int comboNumber, bool isPerformed)
        {
            ComboNumber = comboNumber;
            IsPerformed = isPerformed;
        }
    }
}