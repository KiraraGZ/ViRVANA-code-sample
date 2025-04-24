using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Environment.Tutorial
{
    public class TutorialNarrative : TutorialSequence
    {
        [SerializeField] private float duration = 5f;

        private bool isActive;
        private float stateTime;

        public override void Initialize(PlayerController player)
        {
            stateTime = Time.time;
            isActive = true;

            GameplayController.Instance.InitializeTutorialObjective("Tutorial", null);
        }

        public override void Dispose()
        {
            isActive = false;
        }

        private void Update()
        {
            if (!isActive) return;
            if (Time.time < stateTime + duration) return;

            OnObjectiveProgress();
        }

        #region subscribe events
        protected override void OnObjectiveProgress()
        {
            OnObjectiveProgress(true);
            Dispose();
        }
        #endregion
    }
}
