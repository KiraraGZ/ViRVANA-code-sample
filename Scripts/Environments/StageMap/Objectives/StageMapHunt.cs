using Magia.GameLogic;
using UnityEngine;

namespace Magia.Environment
{
    public class StageMapHunt : StageMapObjective
    {
        [SerializeField] private int defeatAmount;

        [SerializeField] private StageMapSpawner[] enemySpawners;

        public override void Initialize()
        {
            foreach (var spawner in enemySpawners)
            {
                spawner.EventEnemyDefeated += OnEnemyDefeated;
            }
        }

        public override void Dispose()
        {
            foreach (var spawner in enemySpawners)
            {
                spawner.EventEnemyDefeated -= OnEnemyDefeated;
            }
        }

        public override ObjectiveData GetObjectiveData()
        {
            return new(ObjectiveData.ObjectiveMode.HUNT, defeatAmount);
        }

        #region subscribe events
        private void OnEnemyDefeated()
        {
            OnObjectiveProgress();
        }
        #endregion
    }
}
