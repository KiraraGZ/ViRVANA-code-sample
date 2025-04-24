using Magia.GameLogic;
using Magia.Spirits;
using Magia.UI;
using UnityEngine;

namespace Magia.Environment
{
    public class StageMapDestination : StageMapObjective
    {
        [SerializeField] private ContactableObject[] checkpoints;

        private int currentSpiritIndex;

        private void Awake()
        {
            foreach (var checkpoint in checkpoints)
            {
                checkpoint.gameObject.SetActive(false);
            }
        }

        public override void Initialize()
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                checkpoints[i].gameObject.SetActive(true);
                checkpoints[i].Initialize();
                checkpoints[i].EventPlayerContacted += OnSpiritContacted;
            }

            UIManager.Instance.CreateDestinationIndicator(checkpoints[^1].transform);
            currentSpiritIndex = 0;
        }

        public override void Dispose()
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                checkpoints[i].EventPlayerContacted -= OnSpiritContacted;
            }
        }

        public override ObjectiveData GetObjectiveData()
        {
            var objective = new ObjectiveData(ObjectiveData.ObjectiveMode.DESTINATION, 1)
            {
                Destination = new(checkpoints[^1].transform)
            };

            return objective;
        }

        #region subscribe events
        private void OnSpiritContacted(ContactableObject spirit)
        {
            var index = 0;

            for (int i = currentSpiritIndex; i < checkpoints.Length; i++)
            {
                if (spirit != checkpoints[i]) continue;

                index = i;
            }

            for (int i = currentSpiritIndex; i < index; i++)
            {
                checkpoints[i].Dispose();
            }

            currentSpiritIndex = index;

            if (currentSpiritIndex < checkpoints.Length - 1) return;

            UIManager.Instance.RemoveIndicator(checkpoints[^1].transform);
            OnObjectiveProgress();
        }
        #endregion
    }
}
