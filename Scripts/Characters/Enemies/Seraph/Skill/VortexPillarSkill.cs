using System;
using System.Collections.Generic;
using Magia.GameLogic;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Seraph
{
    public class VortexPillarSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        [SerializeField] private VortexPillarSkillData data;

        private float currentRadius;
        private Vector3 centerPoint;
        private List<SurroundProjectile> surroundProjectiles;

        private float RadiusVelocity;
        private float RadiusAcceleration;

        private float OuterRadius;
        private float InnerRadius;

        private float nextAvailableTime;
        private float skillEndTime;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;

        private ProjectilePoolManager projectilePool;

        public void Initialize(VortexPillarSkillData _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            centerPoint = new Vector3(0, owner.transform.position.y, 0);
            surroundProjectiles = new();
            nextAvailableTime = Time.time + data.MaximumDowntime;

            OuterRadius = 350f;
            InnerRadius = 200f;

            projectilePool = new(data.Projectile);
        }

        public void Dispose()
        {
            owner = null;

            projectilePool.Dispose();
            projectilePool = null;
        }

        public bool IsAvailable()
        {
            if (data.IsLoop) return false;

            return Time.time > nextAvailableTime;
        }

        public void Cast()
        {
            SpawnPillars();
            SetupRadiusValue();
            nextAvailableTime = Time.time + data.ProjectileData.ProjectileData.Lifetime + Random.Range(data.MinimumDowntime, data.MaximumDowntime);
            skillEndTime = Time.time + data.VortexTime;
        }

        public void UpdateLogic()
        {
            currentRadius += UpdateVelocity() * Time.deltaTime;
            UpdatePillarPosition();

            if (data.IsLoop) return;
            if (Time.time < skillEndTime) return;

            surroundProjectiles.Clear();
            EventSkillEnd?.Invoke();
        }

        private void SetupRadiusValue()
        {
            currentRadius = data.InitialRadius;

            if (!data.IsLoop)
            {
                float distance = (player.transform.position - centerPoint).magnitude * data.PlayerDistanceDecisionMultiplier;
                RadiusAcceleration = -8f * distance / (data.VortexTime * data.VortexTime);
                RadiusVelocity = 4 * distance / data.VortexTime;
                return;
            }

            RadiusVelocity = data.Velocity;
            RadiusAcceleration = 0;
        }

        private float UpdateVelocity()
        {
            RadiusVelocity += RadiusAcceleration * Time.deltaTime;

            if (!data.IsLoop) return RadiusVelocity;

            if (RadiusVelocity > 0 && currentRadius >= OuterRadius)
            {
                RadiusVelocity = data.Velocity;
            }
            else if (RadiusVelocity < 0 && currentRadius <= InnerRadius)
            {
                RadiusVelocity = -data.Velocity;
            }

            return RadiusVelocity;
        }

        private void UpdatePillarPosition()
        {
            foreach (var surroundProjectile in surroundProjectiles)
            {
                AddDegree(surroundProjectile, data.AngularVelocity * Time.fixedDeltaTime);
                SetTransformToDegree(surroundProjectile);
            }
        }

        private void AddDegree(SurroundProjectile surroundProjectile, float amount)
        {
            surroundProjectile.Angle = MathHelper.ClampAngle(surroundProjectile.Angle + amount);
        }

        private void SetTransformToDegree(SurroundProjectile surroundProjectile)
        {
            float x = currentRadius * Mathf.Cos(surroundProjectile.Angle * Mathf.Deg2Rad);
            float z = currentRadius * Mathf.Sin(surroundProjectile.Angle * Mathf.Deg2Rad);

            Vector3 localPos = new(x, 0, z);
            Vector3 worldPos = centerPoint + localPos;

            surroundProjectile.Projectile.transform.SetPositionAndRotation(worldPos, Quaternion.Euler(0, surroundProjectile.Angle, 0));
        }

        private void SpawnPillars()
        {
            for (int i = 0; i != data.Amount; i++)
            {
                float deg = i * 360f / data.Amount;
                var projectile = projectilePool.Rent(centerPoint + new Vector3((float)Math.Cos(deg), 0, (float)Math.Sin(deg)) * data.InitialRadius, Quaternion.Euler(Vector3.down)) as AngelPillar;

                projectile.Initialize(data.ProjectileData, player.transform, owner);
                surroundProjectiles.Add(new(projectile, deg));
            }
        }

        public class SurroundProjectile
        {
            public AngelPillar Projectile;
            public float Angle;

            public SurroundProjectile(AngelPillar projectile, float angle)
            {
                Projectile = projectile;
                Angle = angle;
            }
        }
    }

    [Serializable]
    public class VortexPillarSkillData
    {
        public AngelPillar Projectile;
        public AngelPillarData ProjectileData;

        public int Amount = 8;
        public float PlayerDistanceDecisionMultiplier = 1.2f;
        public float VortexTime = 12f;
        public float InitialRadius = 30f;
        public float Velocity = 15f;
        public float AngularVelocity = 45f;
        public float MinimumDowntime = 6f;
        public float MaximumDowntime = 8f;

        public bool IsLoop = false;
    }
}