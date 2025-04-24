using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Environment.Tutorial
{
    public class TutorialObjective : TutorialSequence
    {
        [SerializeField] private StageMapObjective objective;

        public override void Initialize(PlayerController player)
        {
            objective.Initialize();
            objective.EventObjectiveProgress += OnObjectiveProgress;
        }

        public override void Dispose()
        {
            objective.Dispose();
            objective.EventObjectiveProgress -= OnObjectiveProgress;
        }

        public ObjectiveData GetObjectiveData()
        {
            return objective.GetObjectiveData();
        }
    }
}
