using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Environment
{
    public abstract class StageMapSpawner : MonoBehaviour
    {
        public event Action EventEnemyReady;
        public event Action EventEnemyDefeated;

        [SerializeField] private EnemySO data;

        protected BaseEnemy[] enemies;
        protected Queue<BaseEnemy> poolQueue;
        protected PlayerController player;

        public BaseEnemy[] Enemies => enemies;
        public EnemySO Enemy => data;

        public virtual void Initialize(PlayerController _player)
        {
            Initialize(_player, 1);
        }

        public void Initialize(PlayerController _player, int amount = 1)
        {
            enemies = new BaseEnemy[amount];
            poolQueue = new();
            player = _player;

            for (int i = 0; i < amount; i++)
            {
                enemies[i] = Instantiate(data.Prefab, transform);
                enemies[i].gameObject.SetActive(false);
                enemies[i].EventReady += OnEnemyReady;
                enemies[i].EventDefeated += OnEnemyDefeated;
                poolQueue.Enqueue(enemies[i]);
            }
        }

        public virtual void Dispose()
        {
            foreach (var enemy in enemies)
            {
                enemy.EventReady += OnEnemyReady;
                enemy.EventDefeated += OnEnemyDefeated;
                enemy.Dispose();
                Destroy(enemy.gameObject);
            }

            enemies = null;
            poolQueue.Clear();
            poolQueue = null;
        }

        protected BaseEnemy SpawnEnemy(Vector3 position)
        {
            return SpawnEnemy(position, Quaternion.Euler(Random.insideUnitSphere * 360)); ;
        }

        protected BaseEnemy SpawnEnemy(Vector3 position, Quaternion rotation)
        {
            var enemy = poolQueue.Dequeue();
            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.gameObject.SetActive(true);
            enemy.Initialize(player);
            enemy.PoolQueue = poolQueue;

            return enemy;
        }

        #region subscribe events
        protected void OnEnemyReady()
        {
            EventEnemyReady?.Invoke();
        }

        protected virtual void OnEnemyDefeated(BaseEnemy enemy)
        {
            EventEnemyDefeated?.Invoke();
        }
        #endregion
    }
}
