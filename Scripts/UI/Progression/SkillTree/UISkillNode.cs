using System;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Progression
{
    public class UISkillNode : MonoBehaviour
    {
        public event Action<int> EventNodeSelected;

        [Header("Visual")]
        [SerializeField] private Button activeButton;
        [SerializeField] private Image iconImage;

        [Space(20)]
        [SerializeField] private Image lockedImage;
        [SerializeField] private Image unlockableImage;
        [SerializeField] private Image inactiveImage;
        [SerializeField] private Image activeImage;
        [SerializeField] private Image glowImage;

        private int index;

        public void Initialize(UISkillNodeData data)
        {
            index = data.Index;

            iconImage.sprite = data.Sprite;
            iconImage.enabled = data.Sprite != null;

            UpdateStatus(data.Status);

            AddListener();
        }

        public void Dispose()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            activeButton.onClick.AddListener(OnActiveButtonClicked);
        }

        private void RemoveListener()
        {
            activeButton.onClick.RemoveListener(OnActiveButtonClicked);
        }

        public void UpdateStatus(NodeStatus status)
        {
            lockedImage.enabled = false;
            unlockableImage.enabled = false;
            activeImage.enabled = false;
            inactiveImage.enabled = false;

            switch (status)
            {
                case NodeStatus.LOCKED:
                    lockedImage.enabled = true;
                    break;
                case NodeStatus.UNLOCKABLE:
                    unlockableImage.enabled = true;
                    break;
                case NodeStatus.ACTIVE:
                    activeImage.enabled = true;
                    break;
                case NodeStatus.INACTIVE:
                    inactiveImage.enabled = true;
                    break;
            }

            glowImage.enabled = status == NodeStatus.ACTIVE;
        }

        #region subscribe events
        private void OnActiveButtonClicked()
        {
            EventNodeSelected?.Invoke(index);
        }
        #endregion
    }

    public enum NodeStatus
    {
        LOCKED = 0,
        UNLOCKABLE = 1,
        INACTIVE = 2,
        ACTIVE = 3,
    }

    public struct UISkillNodeData
    {
        public int Index;
        public Sprite Sprite;
        public NodeStatus Status;

        public UISkillNodeData(int index, Sprite sprite, NodeStatus status)
        {
            Index = index;
            Sprite = sprite;
            Status = status;
        }
    }

    [Serializable]
    public class UpgradeData
    {
        public int Type;
        public int Amount;

        public UpgradeData(int type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
