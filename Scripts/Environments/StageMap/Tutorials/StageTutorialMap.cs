using System;
using System.Collections;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Environment.Tutorial
{
    public class StageTutorialMap : StageMap
    {
        [Header("Tutorial")]
        [SerializeField] private TutorialSequence[] tutorials;

        private PlayerController player;
        private GameplayController gameplayController;

        public override void Initialize(PlayerController _player)
        {
            player = _player;
            gameplayController = GameplayController.Instance;

            base.Initialize(player);

            for (int i = 0; i < tutorials.Length; i++)
            {
                tutorials[i].EventObjectiveProgress += OnObjectiveProgress;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < tutorials.Length; i++)
            {
                tutorials[i].EventObjectiveProgress -= OnObjectiveProgress;
            }

            StopAllCoroutines();
        }

        public override void StartNextObjective(int index)
        {
            objectiveIndex = index;
            tutorials[index].Initialize(player);

            RefreshActiveSpawners();
            PlayDialogue(index, true);
        }

        #region getter
        public override StageMapData GetStageMapData()
        {
            var objectives = new ObjectiveData[tutorials.Length];

            for (int i = 0; i < objectives.Length; i++)
            {
                if (tutorials[i] as TutorialObjective == null)
                {
                    objectives[i] = new(ObjectiveData.ObjectiveMode.TUTORIAL, 1);
                }
                else
                {
                    objectives[i] = (tutorials[i] as TutorialObjective).GetObjectiveData();
                }
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
        protected void OnObjectiveProgress(bool skipImmediately)
        {
            if (skipImmediately == true)
            {
                base.OnObjectiveProgress();
                return;
            }

            gameplayController.DialogueVisualizer.PlayBuddyDialogue(1);

            StartCoroutine(Delay(2f, () =>
            {
                base.OnObjectiveProgress();
            }));
        }
        #endregion

        private void PlayDialogue(int index, bool isStall = false)
        {
            gameplayController.DialogueVisualizer.PlayTutorialDialogue(index, isStall);
        }

        private IEnumerator Delay(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);

            callback.Invoke();
        }
    }
}
