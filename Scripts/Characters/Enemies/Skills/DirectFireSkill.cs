using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Skills
{
    public class DirectFireSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private DirectFireSkillData data;

        private float nextAvailableTime;
        private float nextFireTime;
        private int projectileAmount;
        private int directNumber;

        private Transform firingPoint;
        private BaseEnemy owner;
        private PlayerController player => owner.Player;
        private ProjectilePoolManager projectilePool;

        public void Initialize(DirectFireSkillData _data, Transform _firingPoint, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;
            firingPoint = _firingPoint;

            nextAvailableTime = Time.time + data.Downtime;

            projectilePool = new(data.ProjectilePrefab);
        }

        public void Dispose()
        {
            data = null;
            owner = null;

            if (projectilePool != null)
            {
                projectilePool.Dispose();
                projectilePool = null;
            }
        }

        public void UpdateLogic()
        {
            if (Time.time < nextFireTime) return;

            if (projectileAmount > 0)
            {
                projectileAmount--;
                Fire(directNumber == projectileAmount);
                nextFireTime = Time.time + data.Interval;
            }

            if (projectileAmount <= 0)
            {
                nextAvailableTime = Time.time + data.Downtime;
                EventSkillEnd?.Invoke();
            }
        }

        public bool IsAvailable()
        {
            if (Time.time < nextAvailableTime) return false;
            if (Vector3.Distance(player.transform.position, owner.transform.position) > data.FireRange.y) return false;

            var playerDirection = (player.transform.position - firingPoint.position).normalized;
            if (Vector3.Angle(firingPoint.forward, playerDirection) > 10 + data.SpreadAngle) return false;

            if (Physics.Raycast(firingPoint.position, firingPoint.TransformDirection(Vector3.forward), out var hit, data.FireRange.y))
            {
                if (!hit.collider.TryGetComponent<PlayerController>(out var _)) return false;
            }

            return true;
        }

        public void Cast()
        {
            projectileAmount = data.Amount;
            nextFireTime = Time.time + data.Interval;
            directNumber = Random.Range(0, data.Amount);
        }

        public void FireAtOnce(int amount)
        {
            directNumber = Random.Range(0, amount);

            for (int i = 0; i < amount; i++)
            {
                Fire(directNumber == i);
            }
        }

        private void Fire(bool isDirect)
        {
            Vector3 direction = (player.transform.position - firingPoint.position).normalized;

            if (isDirect == false)
            {
                direction = GetRandomDirection(direction);
            }

            var projectile = projectilePool.Rent(firingPoint.position, Quaternion.LookRotation(direction));
            projectile.Initialize(data.ProjectileData, direction, owner);
        }

        private Vector3 GetRandomDirection(Vector3 direction)
        {
            float randomAngleX = Random.Range(-data.SpreadAngle / 2, data.SpreadAngle / 2);
            float randomAngleY = Random.Range(-data.SpreadAngle / 2, data.SpreadAngle / 2);
            Quaternion randomRotation = Quaternion.Euler(randomAngleY, randomAngleX, 0);

            return randomRotation * direction;
        }
    }

    [Serializable]
    public class DirectFireSkillData
    {
        public Projectile ProjectilePrefab;
        public ProjectileData ProjectileData;
        public int Amount = 3;
        public float Interval = 0.5f;
        public float Downtime = 3f;
        public Vector2 FireRange;
        public float SpreadAngle = 5f;
    }
}
