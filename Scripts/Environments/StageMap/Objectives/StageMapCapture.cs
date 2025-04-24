using Magia.GameLogic;
using UnityEngine;

namespace Magia.Environment
{
    public class StageMapCapture : StageMapObjective
    {
        [SerializeField] private CapturePoint[] capturePoints;

        private void Awake()
        {
            foreach (var capturePoint in capturePoints)
            {
                capturePoint.gameObject.SetActive(false);
            }
        }

        public override void Initialize()
        {
            for (int i = 0; i < capturePoints.Length; i++)
            {
                capturePoints[i].Initialize();
                capturePoints[i].gameObject.SetActive(true);
                capturePoints[i].EventCaptureDone += OnCaptureDone;
            }
        }

        public override void Dispose()
        {
            foreach (var capturePoint in capturePoints)
            {
                capturePoint.Dispose();
                capturePoint.EventCaptureDone -= OnCaptureDone;
            }
        }

        public override ObjectiveData GetObjectiveData()
        {
            return new(ObjectiveData.ObjectiveMode.CAPTURE, capturePoints.Length);
        }

        #region subscribe events
        private void OnCaptureDone()
        {
            OnObjectiveProgress();
        }
        #endregion
    }
}
