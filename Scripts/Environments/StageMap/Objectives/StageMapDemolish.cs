using Magia.GameLogic;
using UnityEngine;

namespace Magia.Environment
{
    public class StageMapDemolish : StageMapObjective
    {
        [SerializeField] private Hive[] hives;

        private void Awake()
        {
            foreach (var hive in hives)
            {
                hive.gameObject.SetActive(false);
            }
        }

        public override void Initialize()
        {
            foreach (var hive in hives)
            {
                hive.gameObject.SetActive(true);
                hive.Initialize();
                hive.EventDestroyed += OnObjectiveDestroyed;
            }
        }

        public override void Dispose()
        {
            foreach (var hive in hives)
            {
                hive.EventDestroyed += OnObjectiveDestroyed;
            }
        }

        public override ObjectiveData GetObjectiveData()
        {
            return new(ObjectiveData.ObjectiveMode.DEMOLISH, hives.Length);
        }

        #region subscribe events
        private void OnObjectiveDestroyed()
        {
            OnObjectiveProgress();
        }
        #endregion
    }
}
