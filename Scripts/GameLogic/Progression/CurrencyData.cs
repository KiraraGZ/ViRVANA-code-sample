using System;

namespace Magia.GameLogic
{
    [Serializable]
    public class CurrencyData
    {
        public enum Currency { EXPERIENCE, SOFT_CURRENCY, HARD_CURRENCY }

        public int Experience;
        public int SoftCurrency;
        public int HardCurrency;

        public CurrencyData(int experience, int softCurrency, int hardCurrency)
        {
            Experience = experience;
            SoftCurrency = softCurrency;
            HardCurrency = hardCurrency;
        }

        public void Add(CurrencyData data)
        {
            Experience += data.Experience;
            SoftCurrency += data.SoftCurrency;
            HardCurrency += data.HardCurrency;
        }

        public void Add(Currency currency, int amount)
        {
            switch (currency)
            {
                case Currency.EXPERIENCE:

                    Experience += amount;
                    return;

                case Currency.SOFT_CURRENCY:

                    SoftCurrency += amount;
                    return;

                case Currency.HARD_CURRENCY:

                    HardCurrency += amount;
                    return;

                default:
                    return;
            }
        }

        public bool TrySpend(Currency currency, int amount)
        {
            switch (currency)
            {
                case Currency.EXPERIENCE:

                    if (Experience < amount) return false;
                    Experience -= amount;
                    return true;

                case Currency.SOFT_CURRENCY:

                    if (SoftCurrency < amount) return false;
                    SoftCurrency -= amount;
                    return true;

                case Currency.HARD_CURRENCY:

                    if (HardCurrency < amount) return false;
                    HardCurrency -= amount;
                    return true;

                default:
                    return false;
            }
        }
    }
}
