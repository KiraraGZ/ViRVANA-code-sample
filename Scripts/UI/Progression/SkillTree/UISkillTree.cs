using System;
using Magia.GameLogic.Progression;
using UnityEngine;

namespace Magia.UI.Progression
{
    public abstract class UISkillTree : MonoBehaviour
    {
        public event Action<UISkillTree, UISkillUpgradeDisplayData> EventNodeSelected;

        [SerializeField] protected SkillTree skillTree;
        [SerializeField] protected UISkillNode[] nodes;

        public virtual void Initialize()
        {
            gameObject.SetActive(true);

            var datas = skillTree.GetUISkillNodeDatas();

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].Initialize(datas[i]);
            }

            AddListener();
        }

        public virtual void Dispose()
        {
            gameObject.SetActive(false);

            foreach (var node in nodes)
            {
                node.Dispose();
            }

            RemoveListener();
        }

        private void AddListener()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].EventNodeSelected += OnNodeSelected;
            }

            skillTree.EventNodeRefreshed += OnNodeRefreshed;
        }

        private void RemoveListener()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].EventNodeSelected -= OnNodeSelected;
            }

            skillTree.EventNodeRefreshed -= OnNodeRefreshed;
        }

        #region public methods
        public void InteractSkillTreeNode()
        {
            int index = skillTree.InteractSelectedNode();
            nodes[index].UpdateStatus(skillTree.GetNodeStatus());
            OnNodeSelected(index);
        }
        #endregion

        #region subscribe events
        private void OnNodeSelected(int index)
        {
            EventNodeSelected?.Invoke(this, skillTree.GetSkillDisplayData(index));
        }

        private void OnNodeRefreshed()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].UpdateStatus(skillTree.Status[i]);
            }
        }
        #endregion
    }
}
