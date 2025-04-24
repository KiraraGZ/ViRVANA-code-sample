using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Environment
{
    public abstract class StageMapObjective : MonoBehaviour
    {
        public event Action EventObjectiveProgress;

        public abstract void Initialize();
        public abstract void Dispose();
        public abstract ObjectiveData GetObjectiveData();

        protected void OnObjectiveProgress()
        {
            EventObjectiveProgress?.Invoke();
        }
    }
}
