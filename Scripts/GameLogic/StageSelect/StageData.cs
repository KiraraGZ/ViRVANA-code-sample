using System;
using System.Collections.Generic;
using Magia.Enemy;
using UnityEngine;

namespace Magia.GameLogic
{
    public class StageInitialData
    {
        public string MapName;
        public Dictionary<string, EnemySO> EnemyDict;
        public StageMapData MapData;

        public StageInitialData(string mapName, Dictionary<string, EnemySO> enemyDict, StageMapData mapData)
        {
            MapName = mapName;
            EnemyDict = enemyDict;
            MapData = mapData;
        }
    }

    [Serializable]
    public class StageCollection
    {
        public StageData[] StageDatas;

        public int Count => StageDatas.Length;
    }

    [Serializable]
    public class StageData
    {
        public string MapName;
        public string RequiredMap;
    }

    [Serializable]
    public class StageSettingData
    {
        public string StageId;
        public float MapTime;
        public Vector3 PlayerSpawnPosition;
        public Vector3 PlayerSpawnRotation;

        public StageSettingData()
        {
            MapTime = 1;
            PlayerSpawnPosition = new Vector3(0, 10, 0);
            PlayerSpawnPosition = Vector3.zero;
        }
    }

    [Serializable]
    public class StageMapData
    {
        public StageSettingData Setting;
        public ObjectiveData[] Objectives;

        public int FirstTimeReward;
        public int[] ExperienceRewards;

        public DialogueCollection Dialogues;

        public List<ObjectiveData> GetBossObjectives()
        {
            var toReturn = new List<ObjectiveData>();

            for (int i = 0; i < Objectives.Length; i++)
            {
                if (Objectives[i].Mode == ObjectiveData.ObjectiveMode.BOSS)
                {
                    toReturn.Add(Objectives[i]);
                }
            }

            return toReturn;
        }
    }

    #region objective
    //TODO: Implement better way to handle many objective requirement.
    [Serializable]
    public class ObjectiveData
    {
        public enum ObjectiveMode
        {
            BOSS,
            HUNT,
            DESTINATION,
            DEMOLISH,
            CAPTURE,
            TUTORIAL,
        }
        public ObjectiveMode Mode;
        public int Target;

        public BossObjectiveData Boss;
        public DestinationObjectiveData Destination;

        public ObjectiveData(ObjectiveMode mode, int target)
        {
            Mode = mode;
            Target = target;
        }
    }

    [Serializable]
    public class BossObjectiveData
    {
        public EnemySO[] Bosses;

        public BossObjectiveData(EnemySO[] bosses)
        {
            Bosses = bosses;
        }
    }

    [Serializable]
    public class DestinationObjectiveData
    {
        public Transform Transform;

        public DestinationObjectiveData(Transform transform)
        {
            Transform = transform;
        }
    }
    #endregion

    #region enemy spawn
    [Serializable]
    public class BossSpawnData
    {
        public string Boss;
        public Vector3 SpawnPosition;
        public Vector3 SpawnRotation;

        public BossSpawnData()
        {
            SpawnPosition = Vector3.zero;
            SpawnRotation = Vector3.zero;
        }

        public BossSpawnData(BossSpawnData data)
        {
            Boss = data.Boss;
            SpawnPosition = data.SpawnPosition;
            SpawnRotation = data.SpawnRotation;
        }
    }

    [Serializable]
    public class EnemySpawnData
    {
        public string Enemy;
        public int MaxAmount;
        public int SpawnAmount;
        public float SpawnInterval;
        public Vector2 SpawnRange;
        public int StartSequence;
        public int StopSequence;

        public EnemySpawnData()
        {
            MaxAmount = 1;
            SpawnAmount = 1;
            SpawnInterval = 10;
            SpawnRange = new(200f, 200f);
            StartSequence = 0;
            StopSequence = 100;
        }

        public EnemySpawnData(EnemySpawnData data)
        {
            Enemy = data.Enemy;
            MaxAmount = data.MaxAmount;
            SpawnAmount = data.SpawnAmount;
            SpawnInterval = data.SpawnInterval;
            SpawnRange = data.SpawnRange;
            StartSequence = data.StartSequence;
            StopSequence = data.StopSequence;
        }
    }
    #endregion

    #region dialogue
    [Serializable]
    public class DialogueCollection
    {
        public DialogueData StageStart;
        public DialogueData StageEnd;

        public DialogueCollection()
        {
            StageStart = new();
            StageEnd = new();
        }

        public DialogueCollection(DialogueCollection data)
        {
            StageStart = new(data.StageStart);
            StageEnd = new(data.StageEnd);
        }
    }

    [Serializable]
    public class DialogueData
    {
        public string Key;
        public int Length;

        public DialogueData()
        {
            Key = "";
            Length = 0;
        }

        public DialogueData(DialogueData data)
        {
            Key = data.Key;
            Length = data.Length;
        }
    }
    #endregion
}
