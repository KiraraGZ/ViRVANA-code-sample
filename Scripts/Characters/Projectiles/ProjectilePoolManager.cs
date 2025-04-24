using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Magia.Projectiles
{
    public class ProjectilePoolManager
    {
        public event Action<DamageFeedback, Vector3> EventProjectileHit;
        public event Action<BaseEnemy> EventProjectileHitEnemy;
        public event Action EventProjectileExpire;

        private Projectile projectilePrefab;
        private Queue<Projectile> poolQueue;
        private List<Projectile> projectilesInScene;

        public ProjectilePoolManager(Projectile projectile)
        {
            Initialize(projectile);
        }

        public void Initialize(Projectile projectile)
        {
            projectilePrefab = projectile;
            poolQueue = new();
            projectilesInScene = new();
        }

        public void Dispose()
        {
            for (int i = 0; i < projectilesInScene.Count; i++)
            {
                projectilesInScene[i].EventProjectileHit -= OnProjectileHit;
                projectilesInScene[i].EventProjectileHitEnemy -= OnProjectileHitEnemy;
                projectilesInScene[i].EventProjectileExpire -= OnProjectileExpire;
                Object.Destroy(projectilesInScene[i].gameObject);
            }

            projectilePrefab = null;
            poolQueue = null;
            projectilesInScene = null;
        }

        public Projectile Rent(Transform transform)
        {
            var projectile = Rent(transform.position, Quaternion.Euler(transform.forward));
            projectile.transform.parent = transform;

            return projectile;
        }

        public Projectile Rent(Vector3 position, Quaternion rotation, Transform parent)
        {
            var projectile = Rent(position, rotation);
            projectile.transform.parent = parent;

            return projectile;
        }

        public Projectile Rent(Vector3 position, Quaternion rotation)
        {
            var projectile = Rent();
            projectile.transform.SetPositionAndRotation(position, rotation);

            return projectile;
        }

        public Projectile Rent()
        {
            if (poolQueue.Count == 0)
            {
                var projectile = Object.Instantiate(projectilePrefab);
                projectile.PoolManager = this;
                projectile.EventProjectileHit += OnProjectileHit;
                projectile.EventProjectileHitEnemy += OnProjectileHitEnemy;
                projectile.EventProjectileExpire += OnProjectileExpire;
                projectilesInScene.Add(projectile);

                return projectile;
            }
            else
            {
                var projectile = poolQueue.Dequeue();
                projectile.gameObject.SetActive(true);

                return projectile;
            }
        }

        public void Return(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);

            if (poolQueue == null) return;

            poolQueue.Enqueue(projectile);
        }

        #region subscribe events
        private void OnProjectileHit(DamageFeedback feedback, Vector3 hitPos)
        {
            EventProjectileHit?.Invoke(feedback, hitPos);
        }

        private void OnProjectileHitEnemy(BaseEnemy enemy)
        {
            EventProjectileHitEnemy?.Invoke(enemy);
        }

        private void OnProjectileExpire()
        {
            EventProjectileExpire?.Invoke();
        }
        #endregion
    }
}
