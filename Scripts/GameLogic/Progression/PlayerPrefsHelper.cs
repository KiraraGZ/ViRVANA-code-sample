using UnityEngine;

namespace Magia.GameLogic
{
    public static class PlayerPrefsHelper
    {
        public const string EXPERIENCE_KEY = "Experience";
        public const string SOFT_CURRENCY_KEY = "SoftCurrency";
        public const string HARD_CURRENCY_KEY = "HardCurrency";

        public const string STAGE_RATING_KEY = "StageProgress";

        public const string ATTACK_SKILL_TREE_KEY = "AttackSkillTree";
        public const string CLOCK_SKILL_TREE_KEY = "ClockSkillTree";

        public static int Experience
        {
            get => PlayerPrefs.GetInt(EXPERIENCE_KEY);
            set => PlayerPrefs.SetInt(EXPERIENCE_KEY, value);
        }

        public static int SoftCurrency
        {
            get => PlayerPrefs.GetInt(SOFT_CURRENCY_KEY);
            set => PlayerPrefs.SetInt(SOFT_CURRENCY_KEY, value);
        }

        public static int HardCurrency
        {
            get => PlayerPrefs.GetInt(HARD_CURRENCY_KEY);
            set => PlayerPrefs.SetInt(HARD_CURRENCY_KEY, value);
        }

        public static void SaveCurrency(CurrencyData data)
        {
            Experience = data.Experience;
            SoftCurrency = data.SoftCurrency;
            HardCurrency = data.HardCurrency;
        }

        public static int GetStageRating(string stageName)
        {
            return PlayerPrefs.GetInt($"{STAGE_RATING_KEY}_{stageName}", 0);
        }

        public static void SetStageRating(string stageName, int rating)
        {
            PlayerPrefs.SetInt($"{STAGE_RATING_KEY}_{stageName}", rating);
        }

        public static void ResetAllProgression()
        {
            PlayerPrefs.DeleteKey(EXPERIENCE_KEY);
            PlayerPrefs.DeleteKey(SOFT_CURRENCY_KEY);
            PlayerPrefs.DeleteKey(HARD_CURRENCY_KEY);
            PlayerPrefs.DeleteKey(ATTACK_SKILL_TREE_KEY);
            PlayerPrefs.DeleteKey(CLOCK_SKILL_TREE_KEY);

            for (int i = 0; i < 100; i++)
            {
                PlayerPrefs.DeleteKey($"{STAGE_RATING_KEY}_{i}");
            }
        }
    }
}
