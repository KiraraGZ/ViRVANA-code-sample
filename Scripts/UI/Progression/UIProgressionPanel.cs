using System;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace Magia.UI.Progression
{
    public class UIProgressionPanel : MonoBehaviour
    {
        public event Action EventStageSelectButtonClicked;

        [SerializeField] private Button stageSelectButton;

        [SerializeField] private UIAttackSkillTree attackSkillTree;
        [SerializeField] private UIClockSkillTree clockSkillTree;
        [SerializeField] private UIMainNode[] mainNodes;

        [SerializeField] private UIProgressionDisplayBox displayBox;

        private UISkillTree selectedSkillTree;

        public void Initialize(StringTable localizedTable)
        {
            gameObject.SetActive(true);

            attackSkillTree.Initialize();
            attackSkillTree.EventNodeSelected += OnSkillNodeSelected;

            clockSkillTree.Initialize();
            clockSkillTree.EventNodeSelected += OnSkillNodeSelected;

            for (int i = 0; i < mainNodes.Length; i++)
            {
                mainNodes[i].Initialize();
                mainNodes[i].EventNodeSelected += OnMainNodeSelected;
            }

            displayBox.Initialize(localizedTable);
            displayBox.EventButtonClicked += OnDisplayBoxButtonClicked;
            displayBox.Display(mainNodes[0].GetSkillDisplayData());

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            attackSkillTree.Dispose();
            attackSkillTree.EventNodeSelected -= OnSkillNodeSelected;

            clockSkillTree.Dispose();
            clockSkillTree.EventNodeSelected -= OnSkillNodeSelected;

            for (int i = 0; i < mainNodes.Length; i++)
            {
                mainNodes[i].Dispose();
                mainNodes[i].EventNodeSelected -= OnMainNodeSelected;
            }

            displayBox.Dispose();
            displayBox.EventButtonClicked -= OnDisplayBoxButtonClicked;

            RemoveListener();
        }

        private void AddListener()
        {
            stageSelectButton.onClick.AddListener(OnStageSelectButtonClicked);
        }

        private void RemoveListener()
        {
            stageSelectButton.onClick.RemoveListener(OnStageSelectButtonClicked);
        }

        #region subscribe events
        private void OnStageSelectButtonClicked()
        {
            EventStageSelectButtonClicked?.Invoke();

            Dispose();
        }

        private void OnSkillNodeSelected(UISkillTree skillTree, UISkillUpgradeDisplayData data)
        {
            selectedSkillTree = skillTree;
            displayBox.Display(data);
        }

        private void OnMainNodeSelected(UISkillDisplayData data)
        {
            displayBox.Display(data);
        }

        private void OnDisplayBoxButtonClicked()
        {
            if (selectedSkillTree == null) return;

            selectedSkillTree.InteractSkillTreeNode();
        }
        #endregion
    }

    public class SkillTreeProgressData
    {
        public NodeStatus[] Attack;
        public NodeStatus[] Clock;

        public SkillTreeProgressData(NodeStatus[] attack, NodeStatus[] clock)
        {
            Attack = attack;
            Clock = clock;
        }
    }
}
