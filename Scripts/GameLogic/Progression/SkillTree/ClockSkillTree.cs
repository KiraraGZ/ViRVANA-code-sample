using System;
using System.Collections.Generic;
using Magia.Skills;
using Magia.UI.Progression;
using UnityEngine;

namespace Magia.GameLogic.Progression
{
    public class ClockSkillTree : SkillTree
    {
        [SerializeField] private ClockSkillNodeData[] nodeDatas;

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

        public ClockSkillUpgradeData GetUpgradeData()
        {
            float chargeInterval = 0.25f;
            int initialEnergy = 0;
            int maxEnergy = 18;
            int maxChargeRefund = 0;
            bool onDash = false;
            bool backwardDash = false;
            float dashCooldownMultiplier = 1f;
            int markResolveStack = 1;

            for (int i = 0; i < nodeDatas.Length; i++)
            {
                if (statusList[i] != NodeStatus.ACTIVE) continue;

                switch (nodeDatas[i].Type)
                {
                    case ClockSkillUpgradeType.CHARGE_INTERVAL:
                        chargeInterval -= 0.05f;
                        break;
                    case ClockSkillUpgradeType.INITIAL_ENERGY:
                        initialEnergy += 6;
                        break;
                    case ClockSkillUpgradeType.MAX_ENERGY:
                        maxEnergy += 6;
                        break;
                    case ClockSkillUpgradeType.MAX_CHARGE_REFUND:
                        maxChargeRefund += 6;
                        break;
                    case ClockSkillUpgradeType.DASH:
                        onDash = true;
                        break;
                    case ClockSkillUpgradeType.DASH_BACKWARD:
                        backwardDash = true;
                        break;
                    case ClockSkillUpgradeType.DASH_COOLDOWN:
                        dashCooldownMultiplier -= 0.25f;
                        break;
                    case ClockSkillUpgradeType.MARK_RESOLVE_STACK:
                        markResolveStack += 1;
                        break;
                }
            }

            return new()
            {
                ChargeInterval = chargeInterval,
                InitialEnergy = initialEnergy,
                MaxEnergy = maxEnergy,
                MaxChargeRefund = maxChargeRefund,
                OnDash = onDash,
                BackwardDash = backwardDash,
                DashCooldownMultiplier = dashCooldownMultiplier,
                MarkResolveStack = markResolveStack,
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
    public class ClockSkillNodeData
    {
        public SkillNodeDataSO DataSO;
        public ClockSkillUpgradeType Type;
        public int Level = 0;
        public int UnlockCost = 5;
        public int ActiveCost = 1;
        public int[] RequiredNodes;
    }

    public enum ClockSkillUpgradeType
    {
        CHARGE_INTERVAL = 1,
        INITIAL_ENERGY = 10,
        MAX_ENERGY = 11,
        MAX_CHARGE_REFUND = 15,
        DASH = 100,
        DASH_BACKWARD = 101,
        DASH_COOLDOWN = 102,
        MARK_RESOLVE_STACK = 200,
    }
}
