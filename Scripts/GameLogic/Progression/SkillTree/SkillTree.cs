using System;
using Magia.UI.Progression;
using UnityEngine;

namespace Magia.GameLogic.Progression
{
    public abstract class SkillTree : MonoBehaviour
    {
        public event Action EventNodeRefreshed;

        protected NodeStatus[] statusList;
        protected int selectedIndex;
        protected CurrencyData.Currency currency;
        protected int cost;

        public NodeStatus[] Status => statusList;

        private ProgressionManager progressionManager;

        public virtual void Initialize(ProgressionManager _progressionManager, NodeStatus[] _status)
        {
            progressionManager = _progressionManager;
            statusList = _status;

            RefreshUnlockNodes();
        }

        public virtual void Dispose()
        {
            progressionManager = null;
            statusList = null;
        }

        public virtual int InteractSelectedNode()
        {
            switch (statusList[selectedIndex])
            {
                case NodeStatus.LOCKED:
                    return selectedIndex;

                case NodeStatus.UNLOCKABLE:
                    if (progressionManager.TrySpendCurrency(currency, cost) == false) return selectedIndex;
                    statusList[selectedIndex] = NodeStatus.INACTIVE;
                    RefreshUnlockNodes();
                    break;

                case NodeStatus.INACTIVE:
                    if (progressionManager.TrySpendCurrency(currency, cost) == false) return selectedIndex;
                    statusList[selectedIndex] = NodeStatus.ACTIVE;
                    break;

                case NodeStatus.ACTIVE:
                    progressionManager.AddCurrency(currency, cost);
                    statusList[selectedIndex] = NodeStatus.INACTIVE;
                    break;
            }

            progressionManager.SaveSkillTreeProgress(this, statusList);

            return selectedIndex;
        }

        public NodeStatus GetNodeStatus()
        {
            return statusList[selectedIndex];
        }

        public abstract UISkillNodeData[] GetUISkillNodeDatas();
        public abstract UISkillUpgradeDisplayData GetSkillDisplayData(int index);

        protected virtual void RefreshUnlockNodes()
        {
            EventNodeRefreshed?.Invoke();
        }

        protected void ResizeStatus(int size)
        {
            if (statusList == null) return;
            if (statusList.Length == size) return;

            var newStatus = new NodeStatus[size];

            for (int i = 0; i < newStatus.Length; i++)
            {
                newStatus[i] = i < statusList.Length ? statusList[i] : NodeStatus.LOCKED;
            }

            statusList = newStatus;
        }
    }
}
