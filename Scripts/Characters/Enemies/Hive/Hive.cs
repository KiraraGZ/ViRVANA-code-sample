using System;
using Magia.GameLogic;
using Magia.UI;
using UnityEngine;

namespace Magia.Environment
{
    public class Hive : MonoBehaviour, IDamageable
    {
        public event Action EventDestroyed;

        [SerializeField] private SpawnerSetup[] spawners;
        [SerializeField] private int maxHelth;

        private int health;
        private int step;

        public void Initialize()
        {
            health = maxHelth;
            step = 0;
            UIManager.Instance.CreateDestinationIndicator(transform);
        }

        public void Dispose()
        {
            UIManager.Instance.RemoveIndicator(transform);
            gameObject.SetActive(false);
        }

        public DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (health <= 0) return new DamageFeedback(damage, -1);

            health -= damage.Amount;

            if (step < spawners.Length && (float)health / maxHelth <= spawners[step].Ratio)
            {
                spawners[step].Spawner.SpawnEnemyWave();
                step++;
            }

            if (health <= 0)
            {
                EventDestroyed?.Invoke();
                Dispose();
            }

            return new DamageFeedback(damage, 1);
        }
    }

    [Serializable]
    public class SpawnerSetup
    {
        public StageEnemySpawner Spawner;
        public float Ratio;
    }
}
