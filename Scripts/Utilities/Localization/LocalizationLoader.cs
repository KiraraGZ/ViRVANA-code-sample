using System.Collections.Generic;
using UnityEngine.Localization.Tables;

namespace Magia.Utilities
{
    public static class LocalizationHelper
    {
        public static string GetLocalizedString(StringTable table, string key)
        {
            var entry = table.GetEntry(key);

            if (entry != null)
            {
                return entry.GetLocalizedString();
            }
            else
            {
                return $"{table} | {key}";
            }
        }

        public static string GetLocalizedString(StringTable table, string key, params object[] args)
        {
            var entry = table.GetEntry(key);

            if (entry != null)
            {
                string localizedString = entry.GetLocalizedString();

                if (args != null && args.Length > 0)
                {
                    return string.Format(localizedString, args);
                }

                return localizedString;
            }
            else
            {
                return $"{table} | {key}";
            }
        }

        public static List<string> romanNumerals = new() { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        public static List<int> numerals = new() { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        public static string ToRomanNumeral(int number)
        {
            var romanNumeral = string.Empty;

            while (number > 0)
            {
                var index = numerals.FindIndex(x => x <= number);
                number -= numerals[index];
                romanNumeral += romanNumerals[index];
            }

            return romanNumeral;
        }
    }
}
