using System.Linq;
using Magia.Skills;
using Magia.UI.Progression;
using UnityEngine;

namespace Magia.GameLogic.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        [SerializeField] private CurrencyData currencyData;
        [SerializeField] private AttackSkillTree attackSkillTree;
        [SerializeField] private ClockSkillTree clockSkillTree;

        private UICurrencyHeader currencyHeader;

        public void Initialize(UICurrencyHeader _currencyHeader)
        {
            currencyData = new(PlayerPrefsHelper.Experience,
                               PlayerPrefsHelper.SoftCurrency,
                               PlayerPrefsHelper.HardCurrency);
            currencyHeader = _currencyHeader;

            UpdateCurrencyUI();

            attackSkillTree.Initialize(this, LoadSkillTreeProgress(PlayerPrefsHelper.ATTACK_SKILL_TREE_KEY));
            clockSkillTree.Initialize(this, LoadSkillTreeProgress(PlayerPrefsHelper.CLOCK_SKILL_TREE_KEY));
        }

        public void Dispose()
        {
            currencyData = null;
            currencyHeader = null;

            attackSkillTree.Dispose();
            clockSkillTree.Dispose();
        }

        #region currency & reward
        public CurrencyData GetCurrency()
        {
            return currencyData;
        }

        public void AddReward(CurrencyData reward)
        {
            currencyData.Add(reward);
            PlayerPrefsHelper.SaveCurrency(currencyData);
            UpdateCurrencyUI();
        }

        public void AddCurrency(CurrencyData.Currency currency, int amount)
        {
            currencyData.Add(currency, amount);
            PlayerPrefsHelper.SaveCurrency(currencyData);
            UpdateCurrencyUI();
        }

        public bool TrySpendCurrency(CurrencyData.Currency currency, int amount)
        {
            var result = currencyData.TrySpend(currency, amount);
            PlayerPrefsHelper.SaveCurrency(currencyData);
            UpdateCurrencyUI();

            return result;
        }

        private void UpdateCurrencyUI()
        {
            currencyHeader.UpdateCurrency(currencyData);
        }

        [ContextMenu("Add Currency")]
        private void DevAddCurrency()
        {
            currencyData.Add(new(100, 10, 10));
            PlayerPrefsHelper.SaveCurrency(currencyData);
            UpdateCurrencyUI();
        }
        #endregion

        #region stage progression
        public void UpdateStageStatus(string stageName, int rating)
        {
            int currentRating = PlayerPrefsHelper.GetStageRating(stageName);

            if (rating > currentRating)
            {
                PlayerPrefsHelper.SetStageRating(stageName, rating);
            }
        }

        [ContextMenu("Reset Progression")]
        private void ResetAllProgression()
        {
            PlayerPrefsHelper.ResetAllProgression();
        }
        #endregion

        #region skill tree
        public void SaveSkillTreeProgress(SkillTree skillTree, NodeStatus[] status)
        {
            string key = "";

            if (skillTree as AttackSkillTree != null)
            {
                key = PlayerPrefsHelper.ATTACK_SKILL_TREE_KEY;
            }
            else if (skillTree as ClockSkillTree != null)
            {
                key = PlayerPrefsHelper.CLOCK_SKILL_TREE_KEY;
            }

            PlayerPrefs.SetString(key, EncodeStatusToString(status));
        }

        public NodeStatus[] LoadSkillTreeProgress(string key)
        {
            return DecodeStatusFromString(PlayerPrefs.GetString(key));
        }

        public string EncodeStatusToString(NodeStatus[] status)
        {
            return string.Concat(status.Select(s => ((int)s).ToString()));
        }

        public NodeStatus[] DecodeStatusFromString(string data)
        {
            var status = new NodeStatus[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i] - '0';

                if (value < 0 || value > 3) continue;

                status[i] = (NodeStatus)value;
            }

            return status;
        }
        #endregion

        #region player skill
        public SkillUpgradeData GetSkillTreeProgress()
        {
            return new()
            {
                Attack = attackSkillTree.GetUpgradeData(),
                Clock = clockSkillTree.GetUpgradeData()
            };
        }

        public SkillTreeProgressData GetUISkillTreeData()
        {
            return new(attackSkillTree.Status,
                       clockSkillTree.Status);
        }
        #endregion

        #region subscribe events
        #endregion
    }
}
