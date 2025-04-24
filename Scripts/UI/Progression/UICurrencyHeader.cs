using Magia.GameLogic;
using TMPro;
using UnityEngine;

namespace Magia.UI.Progression
{
    public class UICurrencyHeader : MonoBehaviour
    {
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] private TMP_Text softCurrencyText;
        [SerializeField] private TMP_Text hardCurrencyText;

        public void Display()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateCurrency(CurrencyData data)
        {
            experienceText.text = data.Experience.ToString();
            softCurrencyText.text = data.SoftCurrency.ToString();
            hardCurrencyText.text = data.HardCurrency.ToString();
        }
    }
}
