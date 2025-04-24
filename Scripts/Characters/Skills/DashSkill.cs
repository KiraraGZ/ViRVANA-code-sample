using System;
using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class DashSkill : ActiveSkill
    {
        public event Action<int> EventSkillHit;

        //TODO: Move this data to player progression manager once it is implemented.
        [SerializeField] private DashSkillData data;
        public DashSkillData Data => data;

        private ProjectilePoolManager projectilePool;

        private float lastAttackTime;

        public override void Initialize(SkillData _data)
        {
            element = ElementType.None;
            projectilePool = new(data.ProjectilePrefab);
            projectilePool.EventProjectileHit += OnDamageDealt;

            base.Initialize(_data);
        }

        public override void Dispose()
        {
            projectilePool.Dispose();
            projectilePool.EventProjectileHit -= OnDamageDealt;
            projectilePool = null;

            base.Dispose();
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();

            StartCoroutine(Downtime(data.DashDowntime));
            skillData.Icon.StartCooldown(data.DashDowntime);
        }

        public override void PhysicsUpdate()
        {
            if (lastAttackTime + data.FireInterval < Time.time)
            {
                Fire();
                lastAttackTime = Time.time;
            }

            base.PhysicsUpdate();
        }

        private void Fire()
        {
            var direction = GetCameraDirection();

            Projectile projectile = projectilePool.Rent(transform.position, Quaternion.LookRotation(direction));
            projectile.Initialize(data.ProjectileData, direction, skillData.Player);
        }


        #region subscribe events
        protected override void OnDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            base.OnDamageDealt(feedback, hitPos);
            EventSkillHit?.Invoke(data.EnergyRecoverPerHit);
        }
        #endregion
    }

    [Serializable]
    public class DashSkillData
    {
        public float SpeedModifier = 3f;
        public float DashUptime = 0.5f;
        public float DashDowntime = 2f;

        [Space(10)]
        public Projectile ProjectilePrefab;
        public ProjectileData ProjectileData;
        public float FireInterval = 0.2f;
        public int EnergyRecoverPerHit = 2;
    }
}
