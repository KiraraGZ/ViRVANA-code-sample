using System;
using System.Collections.Generic;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    public class JuggleHomingSkill : IEnemySkill
    {
        private enum JuggleState { IDLE, CHARGE, FIRE };
        private JuggleState state;

        private JuggleHomingSkillData data;
        private Transform centerPoint;

        private Queue<SurroundProjectile> projectiles;
        private float nextActionTime;

        private BaseEnemy owner;
        private PlayerController Player => owner.Player;
        private ProjectilePoolManager projectilePool;

        public void Initialize(JuggleHomingSkillData _data, Transform _centerPoint, BaseEnemy _owner)
        {
            data = _data;
            owner = _owner;
            centerPoint = _centerPoint;

            state = JuggleState.IDLE;

            projectiles = new();
            projectilePool = new(data.Projectile);
        }

        public void Dispose()
        {
            data = null;
            owner = null;
            centerPoint = null;

            if (projectilePool != null)
            {
                projectilePool.Dispose();
                projectilePool = null;
            }
        }

        public void Cast()
        {
            state = JuggleState.CHARGE;
        }

        public void UpdateLogic()
        {
            switch (state)
            {
                case JuggleState.IDLE:
                    {
                        if (nextActionTime > Time.time) break;
                        if (projectiles.Count == 0) break;

                        var homingProjectile = projectiles.Dequeue().Projectile;
                        homingProjectile.Initialize(data.HomingData, homingProjectile.transform.forward, owner, Player.gameObject);
                        break;
                    }
                case JuggleState.CHARGE:
                    {
                        UpdateProjectilePosition();

                        if (nextActionTime > Time.time) break;

                        AddProjectile();

                        if (projectiles.Count >= data.MaxProjectileAmount)
                        {
                            state = JuggleState.FIRE;
                            nextActionTime = Time.time + data.FireInterval;
                            break;
                        }

                        nextActionTime = Time.time + data.ChargeInterval;
                        break;
                    }
                case JuggleState.FIRE:
                    {
                        UpdateProjectilePosition();

                        if (CheckProjectileAngle(projectiles.Peek().Projectile.transform.forward) > 15f) break;

                        var homingProjectile = projectiles.Dequeue().Projectile;
                        homingProjectile.Initialize(data.HomingData, homingProjectile.transform.forward, owner, Player.gameObject);

                        if (projectiles.Count > 0) break;

                        state = JuggleState.CHARGE;
                        nextActionTime = Time.time + data.ChargeInterval;
                        break;
                    }
            }
        }

        public bool IsAvailable()
        {
            return true;
        }

        public void EnterIdle()
        {
            state = JuggleState.IDLE;
        }

        public void EnterActive()
        {
            state = JuggleState.CHARGE;
        }

        private void AddProjectile()
        {
            HomingProjectile projectile = projectilePool.Rent() as HomingProjectile;
            projectile.transform.localScale = Vector3.one * data.HomingData.ProjectileData.Size;
            float degree = 0f;

            if (projectiles.Count > 0)
            {
                degree = projectiles.Peek().CurrentDegree + 360f * projectiles.Count / (projectiles.Count + 1);
            }

            SurroundProjectile surround = new(projectile, degree);
            projectiles.Enqueue(surround);
        }

        private void UpdateProjectilePosition()
        {
            foreach (var surroundProjectile in projectiles)
            {
                AddDegree(surroundProjectile, data.AngularVelocity * Time.deltaTime);
                SetTransformToDegree(surroundProjectile);
            }
        }

        private void AddDegree(SurroundProjectile surroundProjectile, float degree)
        {
            surroundProjectile.CurrentDegree += degree;
            surroundProjectile.CurrentDegree %= 360f;
        }

        private void SetTransformToDegree(SurroundProjectile surroundProjectile)
        {
            float x = data.Radius * Mathf.Cos(surroundProjectile.CurrentDegree * Mathf.Deg2Rad);
            float z = data.Radius * Mathf.Sin(surroundProjectile.CurrentDegree * Mathf.Deg2Rad);
            var pos = centerPoint.position + new Vector3(x, 0, z);

            surroundProjectile.Projectile.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, surroundProjectile.CurrentDegree, 0) * centerPoint.rotation);
        }

        private float CheckProjectileAngle(Vector3 direction)
        {
            Vector3 toPlayer = Player.transform.position - projectiles.Peek().Projectile.transform.position;

            direction.y = 0;
            toPlayer.y = 0;
            direction.Normalize();
            toPlayer.Normalize();

            float dot = Vector3.Dot(direction, toPlayer);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            return angle;

        }

        private class SurroundProjectile
        {
            public HomingProjectile Projectile;
            public float CurrentDegree;

            public SurroundProjectile(HomingProjectile _homingProjectile, float degree)
            {
                Projectile = _homingProjectile;
                CurrentDegree = degree;
            }
        }
    }

    [Serializable]
    public class JuggleHomingSkillData
    {
        public HomingProjectile Projectile;
        public HomingProjectileData HomingData;

        public float MaxProjectileAmount = 4;
        public float Radius = 1.5f;

        [Header("Charge")]
        public float AngularVelocity = 60f;
        public float ChargeInterval = 4f;

        [Header("Fire")]
        public float FireInterval = 1f;
    }
}