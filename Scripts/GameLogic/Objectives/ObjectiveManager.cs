using System;
using Magia.Environment;
using ObjectiveMode = Magia.GameLogic.ObjectiveData.ObjectiveMode;

namespace Magia.GameLogic
{
    public class ObjectiveManager
    {
        public static string PLAYER_LIFE_WARN_KEY = "Player_Life_Reached_Zero";
        public static string SKIP_OBJECTIVE_KEY = "Skip_Objective";

        public event Action<ObjectiveInfoData> EventObjectiveUpdate;
        public event Action<int> EventStageEnded;

        private int objectiveIndex;
        public ObjectiveData[] Objectives { get; private set; }
        private int targetNumber;
        private int progress;

        private int rating;
        private bool skipObjective;

        private CharacterManager characterManager;
        private StageMap stageMap;

        public void Initialize(ObjectiveData[] objectives, CharacterManager _characterManager, StageMap _stageMap)
        {
            Objectives = objectives;
            objectiveIndex = 0;
            skipObjective = false;

            characterManager = _characterManager;
            characterManager.Player.EventPlayerLifeReachedZero += OnPlayerLifeReachedZero;

            stageMap = _stageMap;
            stageMap.EventObjectiveProgress += OnObjectiveProgress;

            StartNextObjective();
        }

        public void Dispose()
        {
            characterManager.Player.EventPlayerLifeReachedZero -= OnPlayerLifeReachedZero;

            stageMap.EventObjectiveProgress -= OnObjectiveProgress;
        }

        #region objectives
        private void StartNextObjective()
        {
            var objective = Objectives[objectiveIndex];

            stageMap.StartNextObjective(objectiveIndex);

            switch (objective.Mode)
            {
                case ObjectiveMode.BOSS:
                    {
                        targetNumber = objective.Target;
                        progress = 0;
                        break;
                    }
                case ObjectiveMode.HUNT:
                    {
                        targetNumber = objective.Target;
                        progress = 0;
                        break;
                    }
                case ObjectiveMode.DESTINATION:
                    {
                        characterManager.SetBuddyDestination(objective.Destination.Transform.position);
                        targetNumber = 1;
                        progress = 0;
                        break;
                    }
                case ObjectiveMode.DEMOLISH:
                    {
                        targetNumber = objective.Target;
                        progress = 0;
                        break;
                    }
                case ObjectiveMode.CAPTURE:
                    {
                        targetNumber = objective.Target;
                        progress = 0;
                        break;
                    }
                case ObjectiveMode.TUTORIAL:
                    break;
            }
        }

        private void UpdateProgress()
        {
            EventObjectiveUpdate?.Invoke(new ObjectiveInfoData(objectiveIndex, progress, targetNumber, rating));

            if (progress < targetNumber) return;

            UpdateRating();

            if (skipObjective && objectiveIndex < Objectives.Length - 1)
            {
                GameplayController.Instance.DialogueVisualizer.PlayWarnDialogue(SKIP_OBJECTIVE_KEY, 1);
                objectiveIndex = Objectives.Length - 1;
            }
            else
            {
                objectiveIndex++;
            }

            if (objectiveIndex >= Objectives.Length)
            {
                rating = rating == stageMap.RatingIndexes.Length ? 3 : rating;
                EventStageEnded?.Invoke(rating);
                return;
            }

            StartNextObjective();

            if (Objectives[objectiveIndex].Mode == ObjectiveMode.TUTORIAL) return;

            EventObjectiveUpdate?.Invoke(new ObjectiveInfoData(objectiveIndex, progress, targetNumber, rating));
        }

        private void UpdateRating()
        {
            if (skipObjective) return;

            if (rating < stageMap.RatingIndexes.Length && objectiveIndex == stageMap.RatingIndexes[rating])
            {
                rating++;
            }
        }
        #endregion

        #region subscribe events
        private void OnPlayerLifeReachedZero()
        {
            skipObjective = true;

            GameplayController.Instance.DialogueVisualizer.PlayWarnDialogue(PLAYER_LIFE_WARN_KEY, 1);
        }

        private void OnObjectiveProgress()
        {
            if (objectiveIndex >= Objectives.Length) return;

            progress++;
            UpdateProgress();
        }
        #endregion
    }

    public class ObjectiveInfoData
    {
        public int Index;
        public int Progress;
        public int TargetNumber;
        public int Rating;

        public ObjectiveInfoData(int index, int progress, int targetNumber, int rating)
        {
            Index = index;
            Progress = progress;
            TargetNumber = targetNumber;
            Rating = rating;
        }
    }
}
