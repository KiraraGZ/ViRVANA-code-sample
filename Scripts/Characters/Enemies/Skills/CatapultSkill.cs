using System;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Skills
{
    //TODO: move this back to hpyer puffer combat script.
    public class CatapultSkill : IEnemySkill
    {
        public event Action EventSkillEnd;

        private CatapultSkillData skillData;

        private float nextAvailableTime;
        private float nextFireTime;
        private int waveAmount;

        private PlayerController player;
        private BaseEnemy owner;
        private ProjectilePoolManager projectilePool;

        private float gravity => skillData.ProjectileData.Gravity;
        private float speed => skillData.ProjectileData.Speed;

        public void Initialize(CatapultSkillData _skillData, PlayerController _player, BaseEnemy _owner)
        {
            skillData = _skillData;
            player = _player;
            owner = _owner;

            nextAvailableTime = Time.time + skillData.Downtime;

            projectilePool = new(skillData.ProjectilePrefab);
        }

        public void Dispose()
        {
            skillData = null;
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

            waveAmount--;

            var direction = GetProjectileDirection();

            for (int i = 0; i < skillData.Amount; i++)
            {
                Fire(direction: GetRandomDirection(direction));
            }

            if (waveAmount <= 0)
            {
                nextAvailableTime = Time.time + skillData.Downtime;
                EventSkillEnd?.Invoke();
                return;
            }

            nextFireTime = Time.time + skillData.Interval;
        }

        private void Fire(Vector3 direction)
        {
            var projectile = projectilePool.Rent(owner.transform.position + direction * 5, Quaternion.LookRotation(direction));
            projectile.Initialize(skillData.ProjectileData, direction, owner);
        }

        private Vector3 GetRandomDirection(Vector3 direction)
        {
            float randomAngleX = Random.Range(-skillData.SpreadAngle / 2, skillData.SpreadAngle / 2);
            float randomAngleY = Random.Range(-skillData.SpreadAngle / 2, skillData.SpreadAngle / 2);
            Quaternion randomRotation = Quaternion.Euler(randomAngleY, randomAngleX, 0);

            return randomRotation * direction;
        }

        public bool IsAvailable()
        {
            if (Time.time < nextAvailableTime) return false;
            if (CalculateAngle() < 0) return false;

            return true;
        }

        public void Cast()
        {
            waveAmount = skillData.Wave;
            nextFireTime = Time.time;
        }

        private float CalculateAngle()
        {
            Vector3 direction = owner.transform.position - player.transform.position;
            float verticalDistance = direction.y;
            direction.y = 0;
            float horizontalDistance = direction.magnitude;

            float speedSquared = speed * speed;
            float gravityDistance = gravity * horizontalDistance;
            float determinant = speedSquared * speedSquared - gravity * (gravity * horizontalDistance * horizontalDistance + 2 * verticalDistance * speedSquared);

            if (determinant < 0) return -1f;

            float sqrtDeterminant = Mathf.Sqrt(determinant);
            float angle = Mathf.Atan((speedSquared - sqrtDeterminant) / gravityDistance);

            return Mathf.Rad2Deg * angle;
        }

        private Vector3 GetProjectileDirection()
        {
            var angle = Mathf.Deg2Rad * CalculateAngle();

            Vector3 horizontalVelocity = owner.transform.forward.normalized * Mathf.Cos(angle);
            Vector3 verticalVelocity = Vector3.up * Mathf.Sin(angle);

            return horizontalVelocity + verticalVelocity;
        }
    }

    [Serializable]
    public class CatapultSkillData
    {
        public Projectile ProjectilePrefab;
        public ProjectileData ProjectileData;
        public int Wave = 2;
        public int Amount = 3;
        public float Interval = 0.5f;
        public float Downtime = 3f;
        public float SpreadAngle = 5f;
    }
}