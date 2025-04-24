using Magia.Enemy;
using Magia.GameLogic;
using Magia.UI;
using UnityEngine;

namespace Magia.Environment
{
    public class StageMapBoss : StageMapObjective
    {
        [SerializeField] private StageBossSpawner[] bossSpawners;

        public override void Initialize()
        {
            var bosses = new BaseEnemy[bossSpawners.Length];
            var bossDatas = new EnemySO[bossSpawners.Length];

            for (int i = 0; i < bossSpawners.Length; i++)
            {
                bossSpawners[i].EventEnemyReady += OnBossReady;
                bossSpawners[i].EventEnemyDefeated += OnBossDefeated;
                bosses[i] = bossSpawners[i].SpawnBoss();
                bossDatas[i] = bossSpawners[i].Enemy;
            }

            UIManager.Instance.SetBossUI(bossDatas, bosses);
        }

        public override void Dispose()
        {
            foreach (var spawner in bossSpawners)
            {
                spawner.EventEnemyReady += OnBossReady;
                spawner.EventEnemyDefeated -= OnBossDefeated;
            }
        }

        public override ObjectiveData GetObjectiveData()
        {
            var bosses = new EnemySO[bossSpawners.Length];

            for (int i = 0; i < bossSpawners.Length; i++)
            {
                bosses[i] = bossSpawners[i].Enemy;
            }

            var objective = new ObjectiveData(ObjectiveData.ObjectiveMode.BOSS, bossSpawners.Length)
            {
                Boss = new(bosses)
            };

            return objective;
        }

        #region subscribe events
        private void OnBossReady()
        {
            UIManager.Instance.DisplayBossBar();
        }

        private void OnBossDefeated()
        {
            OnObjectiveProgress();
        }
        #endregion
    }
}
