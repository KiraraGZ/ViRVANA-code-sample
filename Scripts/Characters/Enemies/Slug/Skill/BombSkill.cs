using System;
using Magia.GameLogic;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Slug
{
    [Serializable]
    public class BombSkill : IEnemySkill
    {
        private SlugCombat combat;

        private float maxDuration;
        private BaseSlug basePiller;
        private PlayerController player;

        public ExplodeProjectile Projectile;
        public ExplodeProjectileData ProjectileData;
        private ProjectilePoolManager projectilePool;
        public Transform initalFirePoint;
        public Transform finalFirePoint;

        public float maxRadiusThreshold;

        public int minDamage;
        public int maxDamage;

        private float currentDamage;
        private Vector3 currentRadius;
        private ExplodeProjectile currentProjectile;

        private Vector3 radiusIncreasingRate;
        private float damageIncreasingRate;

        private float castTime;

        public void Initialize(SlugCombat _combat, BaseSlug _basePiller, PlayerController _player, float _maxDuration)
        {
            combat = _combat;
            basePiller = _basePiller;
            player = _player;
            maxDuration = _maxDuration;

            combat.EventPlayerExit += Clear;
            combat.EventChargeEnough += Launch;
            combat.EventChargeFull += Launch;

            radiusIncreasingRate = (maxRadiusThreshold - 1) * Projectile.transform.localScale / maxDuration;
            damageIncreasingRate = (maxDamage - minDamage) / maxDuration;

            projectilePool = new(Projectile);
        }

        public void Dispose()
        {
            if (projectilePool != null)
            {
                projectilePool.Dispose();
                projectilePool = null;
            }
        }

        public void Cast()
        {
            ExplodeProjectile projectile = projectilePool.Rent(initalFirePoint.position, Quaternion.identity) as ExplodeProjectile;
            projectile.SetLogicActive(false);
            currentProjectile = projectile;

            currentDamage = minDamage;
            currentRadius = Projectile.transform.localScale;
            currentProjectile.transform.position = initalFirePoint.position;

            castTime = Time.time;
        }

        public void UpdateLogic()
        {
            currentRadius += radiusIncreasingRate * Time.fixedDeltaTime;
            currentDamage += damageIncreasingRate * Time.fixedDeltaTime;

            currentProjectile.transform.localScale = currentRadius;
            currentProjectile.transform.position = Vector3.Lerp(initalFirePoint.position, finalFirePoint.position, (Time.time - castTime) / maxDuration);
        }

        public void Clear()
        {
            currentProjectile.Dispose();
        }

        public void Launch()
        {
            ExplodeProjectileData data = ProjectileData;
            data.ProjectileData.Size = currentProjectile.transform.localScale.x;
            data.ProjectileData.Damage = new Damage((int)currentDamage, data.ProjectileData.Damage.Element, data.ProjectileData.Damage.Type);
            Vector3 direction = (player.transform.position - basePiller.transform.position).normalized;
            currentProjectile.Initialize(data, direction, basePiller);
        }

        public bool IsAvailable()
        {
            return true;
        }
    }
}