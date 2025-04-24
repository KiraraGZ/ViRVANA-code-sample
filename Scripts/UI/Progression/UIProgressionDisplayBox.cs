using System;
using DG.Tweening;
using Magia.GameLogic.Progression;
using Magia.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace Magia.UI.Progression
{
    public class UIProgressionDisplayBox : MonoBehaviour
    {
        public event Action EventButtonClicked;

        [SerializeField] private Image skillIcon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text requiredText;
        [SerializeField] private TMP_Text statusText;

        [SerializeField] private TMP_Text currencyCost;
        [SerializeField] private Image currencyIcon;
        [SerializeField] private UIIconSO currencyIconSO;

        [Header("Button")]
        [SerializeField] private Button button;
        [SerializeField] private Color lockedColor;
        [SerializeField] private Color unlockedColor;

        private StringTable localizedTable;

        public void Initialize(StringTable _localizedTable)
        {
            localizedTable = _localizedTable;

            button.onClick.AddListener(OnButtonClicked);
        }

        public void Dispose()
        {
            localizedTable = null;

            button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Display(UISkillUpgradeDisplayData data)
        {
            string name = LocalizationHelper.GetLocalizedString(localizedTable, data.Data.SkillUpgradeNameKey);
            string level = LocalizationHelper.ToRomanNumeral(data.Level);

            skillIcon.sprite = data.Data.Icon;
            nameText.text = $"{name} {level}";
            descriptionText.text = LocalizationHelper.GetLocalizedString(localizedTable, data.Data.SkillUpgradeDescriptionKey);
            requiredText.text = "Required : ";

            button.gameObject.SetActive(true);

            switch (data.Status)
            {
                case NodeStatus.LOCKED:
                    statusText.text = "LOCKED";
                    button.image.DOColor(lockedColor, 0.5f);
                    button.image.DOFade(1f, 0.5f);
                    break;
                case NodeStatus.UNLOCKABLE:
                    statusText.text = "UNLOCK";
                    button.image.DOColor(lockedColor, 0.5f);
                    button.image.DOFade(0.5f, 0.5f);
                    break;
                case NodeStatus.INACTIVE:
                    statusText.text = "INACTIVE";
                    button.image.DOColor(unlockedColor, 0.5f);
                    button.image.DOFade(0.5f, 0.5f);
                    break;
                case NodeStatus.ACTIVE:
                    statusText.text = "ACTIVE";
                    button.image.DOColor(unlockedColor, 0.5f);
                    button.image.DOFade(1f, 0.5f);
                    break;
            }

            currencyCost.text = data.Cost.ToString();
            currencyIcon.sprite = currencyIconSO.Sprites[data.Status < NodeStatus.INACTIVE ? 0 : 1];
            currencyIcon.enabled = true;

            if (data.Requires.Length == 0 || data.Status != NodeStatus.LOCKED)
            {
                requiredText.text = "";
                return;
            }

            for (int i = 0; ; i++)
            {
                (string nameKey, int levelNumber) = data.Requires[i];
                name = LocalizationHelper.GetLocalizedString(localizedTable, nameKey);
                level = LocalizationHelper.ToRomanNumeral(levelNumber);
                requiredText.text += $"{name} {level}";

                if (i == data.Requires.Length - 1) return;

                requiredText.text += ", ";
            }
        }

        public void Display(UISkillDisplayData data)
        {
            skillIcon.sprite = data.Data.Icon;
            nameText.text = LocalizationHelper.GetLocalizedString(localizedTable, data.Data.SkillUpgradeNameKey);
            descriptionText.text = LocalizationHelper.GetLocalizedString(localizedTable, data.Data.SkillUpgradeDescriptionKey);
            requiredText.text = "";
            statusText.text = "";

            currencyCost.text = "";
            currencyIcon.enabled = false;
            button.gameObject.SetActive(false);
        }

        #region subscribe events
        private void OnButtonClicked()
        {
            EventButtonClicked?.Invoke();
        }
        #endregion
    }

    public struct UISkillUpgradeDisplayData
    {
        public SkillNodeDataSO Data;
        public int Level;
        public NodeStatus Status;
        public int Cost;
        public (string, int)[] Requires;

        public UISkillUpgradeDisplayData(SkillNodeDataSO data, int level, NodeStatus status, int cost, (string, int)[] requires)
        {
            Data = data;
            Level = level;
            Status = status;
            Cost = cost;
            Requires = requires;
        }
    }

    public struct UISkillDisplayData
    {
        public SkillNodeDataSO Data;

        public UISkillDisplayData(SkillNodeDataSO data)
        {
            Data = data;
        }
    }
}
