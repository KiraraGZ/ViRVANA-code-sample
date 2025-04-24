using System;
using System.Linq;
using System.Collections.Generic;
using Magia.GameLogic;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Slug
{
    [Serializable]
    public class MultiBombSkill : IEnemySkill
    {
        //base parameter
        private SlugCombat combat;

        private float maxDuration;
        private float bombChargeDuration;
        private BaseSlug basePiller;
        private PlayerController player;

        //projectile parameter
        [Header("Projectile")]
        public HomingExplodeProjectile Projectile;
        public HomingExplodeProjectileData ProjectileData;
        private ProjectilePoolManager projectilePool;

        public Transform centerPoint;

        //current bomb charge skill
        public Transform initalFirePoint;
        public Transform finalFirePoint;
        public float maxRadiusThreshold;
        public int minDamage;
        public int maxDamage;

        private float currentDamage;
        private Vector3 currentRadius;
        private HomingExplodeProjectile currentProjectile;
        private MultiBombPillerState chargeState;

        //bomb logic parameter
        private Vector3 radiusIncreasingRate;
        private float damageIncreasingRate;

        // Drop logic parameter
        public float dropdownDuration;
        private float startDropdownTime;
        private float dropBombRadius;

        //Surround bomb parameter
        [Header("Moving Surrond")]
        private List<SurroundProjectile> sparedProjectiles = new();
        public int maxBombAmount;
        public float surroundRadius;
        public float angularVelocity;

        private float lastBombCastTime;

        public bool enableCast => sparedProjectiles.Count() >= 1;

        public void Initialize(SlugCombat _combat, BaseSlug _basePiller, PlayerController _player, float _maxDuration)
        {
            combat = _combat;
            basePiller = _basePiller;
            player = _player;
            maxDuration = _maxDuration;

            combat.EventPlayerExit += Clear;
            combat.EventChargeEnough += Launch;
            combat.EventChargeFull += Launch;

            bombChargeDuration = (maxDuration / (maxBombAmount + 1)) - dropdownDuration;

            radiusIncreasingRate = (maxRadiusThreshold - 1) * Projectile.transform.localScale / bombChargeDuration;
            damageIncreasingRate = (maxDamage - minDamage) / bombChargeDuration;

            projectilePool = new(Projectile);
            lastBombCastTime = Time.time;
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
            HomingExplodeProjectile projectile = projectilePool.Rent(initalFirePoint.position, Quaternion.identity) as HomingExplodeProjectile;
            projectile.SetLogicActive(false);
            currentProjectile = projectile;

            currentDamage = minDamage;
            currentRadius = Projectile.transform.localScale;
            currentProjectile.transform.position = initalFirePoint.position;

            lastBombCastTime = Time.time;
        }

        public void UpdateLogic()
        {
            if (chargeState == MultiBombPillerState.CHARGE)
            {
                if (Time.time - lastBombCastTime >= bombChargeDuration)
                {
                    SurroundProjectile surroundProjectile = new SurroundProjectile(currentProjectile, 0);
                    AddProjectile(surroundProjectile);
                    startDropdownTime = Time.time;
                    chargeState = MultiBombPillerState.DROP;
                }
                else
                {
                    currentRadius += radiusIncreasingRate * Time.fixedDeltaTime;
                    currentDamage += damageIncreasingRate * Time.fixedDeltaTime;

                    currentProjectile.transform.localScale = currentRadius;
                    currentProjectile.transform.position = Vector3.Lerp(initalFirePoint.position, finalFirePoint.position, (Time.time - lastBombCastTime) / maxDuration);
                }
            }
            else if (chargeState == MultiBombPillerState.DROP)
            {
                if (Time.time - startDropdownTime >= dropdownDuration)
                {
                    chargeState = MultiBombPillerState.CHARGE;
                    InitialNewCurrentProjectile();
                }
                else
                {
                    float t = Mathf.Clamp01((Time.time - startDropdownTime) / dropdownDuration);
                    dropBombRadius = Mathf.Lerp(0, surroundRadius, t);
                }
            }
            UpdateProjectilePosition();
        }

        public void Clear()
        {
            foreach (SurroundProjectile e in sparedProjectiles)
            {
                e.projectile.Dispose();
            }
            sparedProjectiles.Clear();

            currentProjectile.Dispose();
        }

        public void Launch()
        {
            foreach (var projectile in sparedProjectiles)
            {
                HomingExplodeProjectileData data = ProjectileData;
                data.ProjectileData.ProjectileData.Size = projectile.projectile.transform.localScale.x;
                data.ProjectileData.ProjectileData.Damage = new Damage((int)currentDamage, data.ProjectileData.ProjectileData.Damage.Element, data.ProjectileData.ProjectileData.Damage.Type);
                Vector3 direction = (player.transform.position - basePiller.transform.position).normalized;
                projectile.projectile.Initialize(data, direction, basePiller, player.gameObject);
            }
            sparedProjectiles.Clear();
        }

        public bool IsAvailable()
        {
            return true;
        }

        public void InitialNewCurrentProjectile()
        {
            HomingExplodeProjectile projectile = projectilePool.Rent(initalFirePoint.position, Quaternion.identity) as HomingExplodeProjectile;
            projectile.SetLogicActive(false);
            currentProjectile = projectile;

            currentDamage = minDamage;
            currentRadius = Projectile.transform.localScale;
            currentProjectile.transform.position = initalFirePoint.position;

            lastBombCastTime = Time.time;
        }

        public void AddProjectile(SurroundProjectile surroundProjectile)
        {
            if (sparedProjectiles.Any())
            {
                float newDegreeInterval = 360f / (sparedProjectiles.Count + 1);
                float firstDegree = sparedProjectiles[0].currentDegree;
                for (int i = 1; i != sparedProjectiles.Count; i++)
                {
                    sparedProjectiles[i].currentDegree = firstDegree + (i * newDegreeInterval);
                }
                surroundProjectile.currentDegree = firstDegree + (sparedProjectiles.Count * newDegreeInterval);
            }
            sparedProjectiles.Add(surroundProjectile);
        }

        public void UpdateProjectilePosition()
        {
            if (sparedProjectiles.Any())
            {
                if (sparedProjectiles[0].projectile.transform.position.y != centerPoint.position.y)
                {
                    Vector3 target = Vector3.Lerp(finalFirePoint.transform.position, centerPoint.transform.position, (Time.time - startDropdownTime) / dropdownDuration);
                    sparedProjectiles[0].projectile.transform.position = target;
                }
            }
            for (int i = 0; i != sparedProjectiles.Count; i++)
            {
                AddDegree(sparedProjectiles[i], angularVelocity * Time.fixedDeltaTime);
                if (i == 0 && chargeState == MultiBombPillerState.DROP)
                {
                    SetDropTransformToDegree(sparedProjectiles[0]);
                }
                else
                {
                    SetTransformToDegree(sparedProjectiles[i]);
                }
            }
        }

        public void AddDegree(SurroundProjectile surroundProjectile, float a)
        {
            surroundProjectile.currentDegree += a;
            if (surroundProjectile.currentDegree >= 360f)
            {
                surroundProjectile.currentDegree -= 360f;
            }
        }

        public void SetTransformToDegree(SurroundProjectile surroundProjectile)
        {
            float x = surroundRadius * Mathf.Cos(surroundProjectile.currentDegree * Mathf.Deg2Rad);
            float z = surroundRadius * Mathf.Sin(surroundProjectile.currentDegree * Mathf.Deg2Rad);

            Vector3 localPos = new Vector3(x, 0, z);
            Vector3 worldPos = centerPoint.TransformPoint(localPos);

            surroundProjectile.projectile.transform.position = worldPos;
            surroundProjectile.projectile.transform.rotation = Quaternion.Euler(0, surroundProjectile.currentDegree, 0) * centerPoint.rotation;
        }

        public void SetDropTransformToDegree(SurroundProjectile surroundProjectile)
        {
            float x = dropBombRadius * Mathf.Cos(surroundProjectile.currentDegree * Mathf.Deg2Rad);
            float z = dropBombRadius * Mathf.Sin(surroundProjectile.currentDegree * Mathf.Deg2Rad);

            Vector3 localPos = new Vector3(x, 0, z);
            Vector3 worldPos = centerPoint.TransformPoint(localPos);

            surroundProjectile.projectile.transform.position = worldPos;
            surroundProjectile.projectile.transform.rotation = Quaternion.Euler(0, surroundProjectile.currentDegree, 0) * centerPoint.rotation;
        }

        public class SurroundProjectile
        {
            public HomingExplodeProjectile projectile;
            public float currentDegree;

            public SurroundProjectile(HomingExplodeProjectile _explodeProjectile, float degree)
            {
                projectile = _explodeProjectile;
                currentDegree = degree;
            }
        }

        public enum MultiBombPillerState
        {
            CHARGE,
            DROP,
        }
    }
}