using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Environment
{
    public class StageMap : MonoBehaviour
    {
        public event Action EventObjectiveProgress;

        [SerializeField] protected StageSettingData setting;
        [SerializeField] private StageEnemySpawnerSetup[] enemySpawners;
        [SerializeField] private StageMapObjective[] mapObjectives;
        [SerializeField] private int[] ratingObjectiveIndexes;
        public int[] RatingIndexes => ratingObjectiveIndexes;
        [SerializeField] protected int FirstTimeReward;
        [SerializeField] protected int[] ExperienceRewards;
        [SerializeField] protected DialogueCollection dialogues;

        private List<StageEnemySpawner> activeSpawners;
        protected int objectiveIndex;

        public virtual void Initialize(PlayerController player)
        {
            for (int i = 0; i < enemySpawners.Length; i++)
            {
                enemySpawners[i].Spawner.Initialize(player);
            }

            for (int i = 0; i < mapObjectives.Length; i++)
            {
                mapObjectives[i].EventObjectiveProgress += OnObjectiveProgress;
            }

            activeSpawners = new();
        }

        public virtual void Dispose()
        {
            foreach (var setup in enemySpawners)
            {
                setup.Spawner.Dispose();
            }

            for (int i = 0; i < mapObjectives.Length; i++)
            {
                mapObjectives[i].EventObjectiveProgress -= OnObjectiveProgress;
            }

            activeSpawners = null;
        }

        public virtual void StartNextObjective(int index)
        {
            objectiveIndex = index;
            mapObjectives[index].Initialize();

            if (index > 0)
            {
                mapObjectives[index - 1].Dispose();
            }

            RefreshActiveSpawners();
        }

        protected void RefreshActiveSpawners()
        {
            activeSpawners.Clear();

            foreach (var setup in enemySpawners)
            {
                if (setup.Spawner as StageEnemySpawner == null) continue;

                if (objectiveIndex == setup.StartSequence && objectiveIndex == setup.StopSequence)
                {
                    (setup.Spawner as StageEnemySpawner).SpawnEnemyWave();
                    continue;
                }

                if (objectiveIndex < setup.StartSequence || objectiveIndex >= setup.StopSequence) continue;

                activeSpawners.Add(setup.Spawner as StageEnemySpawner);
            }
        }

        private void Update()
        {
            if (objectiveIndex < 0) return;

            foreach (var spawner in activeSpawners)
            {
                spawner.UpdateLogic();
            }
        }

        #region getter
        public List<BaseEnemy> GetSpawnerEnemies()
        {
            var enemies = new List<BaseEnemy>();

            for (int i = 0; i < enemySpawners.Length; i++)
            {
                var spawnerEnemies = enemySpawners[i].Spawner.Enemies;

                foreach (var enemy in spawnerEnemies)
                {
                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        public EnemySO[] GetSpawnerEnemyDatas()
        {
            var enemies = new EnemySO[enemySpawners.Length];

            for (int i = 0; i < enemySpawners.Length; i++)
            {
                enemies[i] = enemySpawners[i].Spawner.Enemy;
            }

            return enemies;
        }

        public virtual StageMapData GetStageMapData()
        {
            var objectives = new ObjectiveData[mapObjectives.Length];

            for (int i = 0; i < objectives.Length; i++)
            {
                objectives[i] = mapObjectives[i].GetObjectiveData();
            }

            return new()
            {
                Setting = setting,
                Objectives = objectives,
                FirstTimeReward = FirstTimeReward,
                ExperienceRewards = ExperienceRewards,
                Dialogues = dialogues,
            };
        }
        #endregion

        #region subscribe events
        protected virtual void OnObjectiveProgress()
        {
            EventObjectiveProgress?.Invoke();
        }
        #endregion
    }

    [Serializable]
    public class StageEnemySpawnerSetup
    {
        public StageMapSpawner Spawner;
        public int StartSequence;
        public int StopSequence;
    }
}
