using System;
using Magia.Player;
using UnityEngine;

namespace Magia.Environment.Tutorial
{
    public abstract class TutorialSequence : MonoBehaviour
    {
        public event Action<bool> EventObjectiveProgress;

        protected ThirdPersonInputAction.PlayerActions actions;

        public virtual void Initialize(PlayerController player)
        {
            actions = player.Input.PlayerActions;
        }

        public abstract void Dispose();

        #region subscribe events
        protected virtual void OnObjectiveProgress()
        {
            OnObjectiveProgress(false);
            Dispose();
        }

        protected void OnObjectiveProgress(bool skipImmediately)
        {
            EventObjectiveProgress?.Invoke(skipImmediately);
        }
        #endregion
    }
}
