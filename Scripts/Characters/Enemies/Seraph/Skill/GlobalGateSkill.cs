using System;
using System.Collections.Generic;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Skills
{
    public class GlobalGateSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private GlobalGateSkillData data;

        private int LaunchIndex;
        private float nextLaunchTime;
        private float nextAvailableTime;

        private BaseEnemy owner;
        private PlayerController player => owner.Player;

        private ProjectilePoolManager projectilePool;
        private List<Projectile> projectiles;
        private List<Vector3> projectilesPosition;

        public void Initialize(GlobalGateSkillData _data, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;

            projectilePool = new(data.Projectile);
            projectilesPosition = new();
            projectiles = new();

            for (int i = 0; i != data.Amount; i++)
            {
                projectilesPosition.Add(new Vector3(Mathf.Cos(i * 360f / data.Amount * Mathf.Deg2Rad) * data.HorizontalDistance, data.VerticalDistance, Mathf.Sin(i * 360f / data.Amount * Mathf.Deg2Rad) * data.HorizontalDistance));
            }

            nextAvailableTime = Time.time + data.MinimumDowntime;
        }

        public void Dispose()
        {
            data = null;
            owner = null;

            projectilesPosition = null;
            projectiles = null;

            projectilePool.Dispose();
            projectilePool = null;
        }

        public bool IsAvailable()
        {
            return Time.time >= nextAvailableTime;
        }

        public void Cast()
        {
            SpawnProjectiles();

            nextAvailableTime = Time.time + data.Delay + Random.Range(data.MinimumDowntime, data.MaximumDowntime);
            nextLaunchTime = Time.time + data.Delay;
        }

        public void UpdateLogic()
        {
            UpdateProjectilesDirection();

            if (Time.time < nextLaunchTime) return;

            projectiles[LaunchIndex].Initialize(data.ProjectileData, projectiles[LaunchIndex].transform.forward, owner);
            LaunchIndex += 1;

            if (LaunchIndex == data.Amount)
            {
                LaunchIndex = 0;
                projectiles.Clear();
                EventSkillEnd?.Invoke();
                return;
            }

            nextLaunchTime = Time.time + data.LaunchInterval;
        }

        private void SpawnProjectiles()
        {
            var amount = data.Amount;
            var spawnPos = projectilesPosition;

            for (int i = 0; i < amount; i++)
            {
                var projectile = projectilePool.Rent(player.transform.position + spawnPos[i], Quaternion.identity);
                projectile.transform.LookAt(player.transform.position);
                projectiles.Add(projectile);
            }
        }

        private void UpdateProjectilesDirection()
        {
            for (int i = LaunchIndex; i < projectiles.Count; i++)
            {
                projectiles[i].transform.LookAt(player.transform.position);
            }
        }
    }

    [Serializable]
    public class GlobalGateSkillData
    {
        public Projectile Projectile;
        public ProjectileData ProjectileData;

        public int Amount = 4;
        public float HorizontalDistance = 10f;
        public float VerticalDistance = 20f;
        public float Delay = 3f;
        public float LaunchInterval = 0.3f;
        public float MinimumDowntime = 6f;
        public float MaximumDowntime = 8f;
    }
}