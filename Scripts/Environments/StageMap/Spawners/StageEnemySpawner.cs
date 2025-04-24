using Magia.Player;
using Magia.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Environment
{
    public class StageEnemySpawner : StageMapSpawner
    {
        [SerializeField] private int maxAmount = 3;
        [SerializeField] private int spawnAmount = 1;
        [SerializeField] private float minSpawnInterval = 1;
        [SerializeField] private float maxSpawnInterval = 1;
        [SerializeField] private float spawnRadius = 10;
        [SerializeField] private bool flattenSpawnHeight = false;

        private float nextSpawnTime;

        public override void Initialize(PlayerController _player)
        {
            Initialize(_player, maxAmount);

            nextSpawnTime = Time.time;

            foreach (var enemy in enemies)
            {
                UIManager.Instance.CreateEnemyIndicator(enemy.transform);
            }
        }

        public void UpdateLogic()
        {
            if (poolQueue.Count <= 0 || Time.time < nextSpawnTime) return;

            SpawnEnemyWave();
            nextSpawnTime = Time.time + GetSpawnInterval();
        }

        public void SpawnEnemyWave()
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                if (poolQueue.Count == 0) return;

                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

                if (flattenSpawnHeight)
                {
                    spawnPosition.y = transform.position.y;
                }

                SpawnEnemy(spawnPosition);
            }
        }

        private float GetSpawnInterval()
        {
            return Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }
}
