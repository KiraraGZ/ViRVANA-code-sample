using System;
using System.Collections.Generic;
using Magia.Skills;
using Magia.UI.Progression;
using UnityEngine;

namespace Magia.GameLogic.Progression
{
    public class AttackSkillTree : SkillTree
    {
        [SerializeField] private AttackSkillNodeData[] nodeDatas;

        public override void Initialize(ProgressionManager progressionManager, NodeStatus[] status)
        {
            base.Initialize(progressionManager, status);

            ResizeStatus(nodeDatas.Length);
        }

        #region public methods
        public override int InteractSelectedNode()
        {
            //TODO: if node would be set inactive, also set every node that required this node inactive too.
            return base.InteractSelectedNode();
        }
        #endregion

        #region get data
        public override UISkillNodeData[] GetUISkillNodeDatas()
        {
            var uiDatas = new UISkillNodeData[nodeDatas.Length];

            for (int i = 0; i < nodeDatas.Length; i++)
            {
                uiDatas[i] = new UISkillNodeData(i, nodeDatas[i].DataSO.Icon, statusList[i]);
            }

            return uiDatas;
        }

        public override UISkillUpgradeDisplayData GetSkillDisplayData(int index)
        {
            selectedIndex = index;
            var node = nodeDatas[index];
            var status = statusList[index];
            var requires = new List<(string, int)>();

            //TODO: if required node is inactive, do not allow player to set active this node.
            switch (status)
            {
                case NodeStatus.LOCKED:
                case NodeStatus.UNLOCKABLE:
                    currency = CurrencyData.Currency.EXPERIENCE;
                    cost = node.UnlockCost;
                    break;
                case NodeStatus.INACTIVE:
                case NodeStatus.ACTIVE:
                    currency = CurrencyData.Currency.SOFT_CURRENCY;
                    cost = node.ActiveCost;
                    break;
            }

            for (int i = 0; i < nodeDatas[index].RequiredNodes.Length; i++)
            {
                int requiredIndex = nodeDatas[index].RequiredNodes[i];

                if (statusList[requiredIndex] > NodeStatus.UNLOCKABLE) continue;

                requires.Add((nodeDatas[requiredIndex].DataSO.SkillUpgradeNameKey, nodeDatas[requiredIndex].Level));
            }

            return new(node.DataSO, node.Level, status, cost, requires.ToArray());
        }

        public AttackSkillUpgradeData GetUpgradeData()
        {
            float spreadAngleMultiplier = 1f;
            float forthExplosiveChance = 0f;

            for (int i = 0; i < nodeDatas.Length; i++)
            {
                if (statusList[i] != NodeStatus.ACTIVE) continue;

                switch (nodeDatas[i].Type)
                {
                    case AttackSkillUpgradeType.SPREAD_ANGLE:
                        spreadAngleMultiplier -= 0.2f;
                        break;
                    case AttackSkillUpgradeType.FORTH_EXPLOSIVE:
                        forthExplosiveChance += 0.2f;
                        break;
                }
            }

            return new()
            {
                SpreadAngleMultiplier = spreadAngleMultiplier,
                ForthExplosiveChance = forthExplosiveChance,
            };
        }
        #endregion

        protected override void RefreshUnlockNodes()
        {
            ResizeStatus(nodeDatas.Length);

            for (int i = 0; i < nodeDatas.Length; i++)
            {
                if (statusList[i] >= NodeStatus.UNLOCKABLE) continue;
                if (IsNodeUnlock(i) == false) continue;

                statusList[i] = NodeStatus.UNLOCKABLE;
            }

            base.RefreshUnlockNodes();
        }

        protected bool IsNodeUnlock(int index)
        {
            if (statusList[index] > NodeStatus.UNLOCKABLE) return true;
            if (nodeDatas[index].RequiredNodes.Length == 0) return true;

            foreach (int requiredIndex in nodeDatas[index].RequiredNodes)
            {
                if (index == requiredIndex) continue;
                if (IsNodeUnlock(requiredIndex) == false) return false;
                if (statusList[requiredIndex] <= NodeStatus.UNLOCKABLE) return false;
            }

            return true;
        }
    }

    [Serializable]
    public class AttackSkillNodeData
    {
        public SkillNodeDataSO DataSO;
        public AttackSkillUpgradeType Type;
        public int Level = 0;
        public int UnlockCost = 5;
        public int ActiveCost = 1;
        public int[] RequiredNodes;
    }

    public enum AttackSkillUpgradeType
    {
        SPREAD_ANGLE = 10,
        FORTH_EXPLOSIVE = 100,
    }
}
