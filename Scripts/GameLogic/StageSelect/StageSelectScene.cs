using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.GameLogic
{
    public class StageSelectScene : MonoBehaviour
    {
        public event Action<int> EventStageNodeSelect;

        [Header("Camera Point")]
        [SerializeField] private Transform cameraStartPoint;
        public Transform CameraStartPoint => cameraStartPoint;
        [SerializeField] private Transform cameraEndPoint;
        public Transform CameraEndPoint => cameraEndPoint;

        [Header("Stage Node")]
        [SerializeField] private StageNode stageNodePrefab;
        [SerializeField] private StageNode[] storyNodes;

        private StageNode[] expeditionNodes;

        public void Initialize()
        {
            AddListener();
        }

        public void Dispose()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            for (int i = 0; i < storyNodes.Length; i++)
            {
                storyNodes[i].EventStageNodeClicked += OnStageNodeClicked;
            }
        }

        private void RemoveListener()
        {
            for (int i = 0; i < storyNodes.Length; i++)
            {
                storyNodes[i].EventStageNodeClicked -= OnStageNodeClicked;
            }
        }

        #region story
        public void DisplayStoryNode(StageStatus[] stagesStatus)
        {
            for (int i = 0; i < stagesStatus.Length; i++)
            {
                storyNodes[i].Display(i, stagesStatus[i]);
            }
        }

        public void HideStoryNode()
        {
            for (int i = 0; i < storyNodes.Length; i++)
            {
                storyNodes[i].Hide();
            }
        }
        #endregion

        #region expedition
        public void SetupExpeditionNode(int amount)
        {
            expeditionNodes = new StageNode[amount];

            for (int i = 0; i < amount; i++)
            {
                Vector3 pos = Random.insideUnitSphere * 200;
                pos.y = 0;

                expeditionNodes[i] = stageNodePrefab.Rent(pos, Quaternion.identity, transform);
                expeditionNodes[i].Display(i, new StageStatus());
                expeditionNodes[i].EventStageNodeClicked += OnStageNodeClicked;
            }
        }

        public void RemoveExpeditionNode()
        {
            for (int i = 0; i < expeditionNodes.Length; i++)
            {
                expeditionNodes[i].Hide();
                expeditionNodes[i].Return();
                expeditionNodes[i].EventStageNodeClicked -= OnStageNodeClicked;
            }
        }
        #endregion

        #region subscribe events
        private void OnStageNodeClicked(int index)
        {
            EventStageNodeSelect.Invoke(index);
        }
        #endregion
    }
}
