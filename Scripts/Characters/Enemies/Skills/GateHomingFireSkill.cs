using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class GateHomingFireSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private GateHomingFireSkillSO data;

        private float nextAvailableTime;
        private float nextFireTime;
        private int waveAmount;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private ProjectilePoolManager gateProjectilePool;
        private ProjectilePoolManager homingProjectilePool;

        public void Initialize(GateHomingFireSkillSO _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;
            gateProjectilePool = new(data.Prefab);
            homingProjectilePool = new(data.GateHomingData.Prefab);
            nextAvailableTime = Time.time + data.Downtime;
        }

        public void Dispose()
        {
            data = null;
            owner = null;

            if (gateProjectilePool != null)
            {
                gateProjectilePool.Dispose();
                gateProjectilePool = null;
            }

            if (homingProjectilePool != null)
            {
                homingProjectilePool.Dispose();
                homingProjectilePool = null;
            }
        }

        public void UpdateLogic()
        {
            if (Time.time < nextFireTime) return;

            if (waveAmount > 0)
            {
                waveAmount--;
                Fire();
                nextFireTime = Time.time + data.Interval;
            }

            if (waveAmount <= 0)
            {
                nextAvailableTime = Time.time + data.Downtime;
                EventSkillEnd?.Invoke();
            }
        }

        public bool IsAvailable()
        {
            return Time.time >= nextAvailableTime;
        }

        public void Cast()
        {
            waveAmount = data.Wave;
            nextFireTime = Time.time + data.Interval;
        }

        private void Fire()
        {
            var gate = gateProjectilePool.Rent(owner.transform.position, Quaternion.identity) as GateHomingProjectile;
            gate.Initialize(data.GateHomingData, homingProjectilePool, owner, player.gameObject);
        }
    }
}
