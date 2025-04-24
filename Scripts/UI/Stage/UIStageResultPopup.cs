using Magia.GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UIStageResultPopup : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        [Header("Display")]
        [SerializeField] private TMP_Text stageNameText;
        [SerializeField] private TMP_Text softCurrencyText;
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] private GameObject firstTimeBox;
        [SerializeField] private Image[] ratingImages;

        public void Initialize(UIStageResultPopupData data)
        {
            gameObject.SetActive(true);

            stageNameText.text = data.StageId;
            softCurrencyText.text = $"{data.Reward.SoftCurrency}";
            experienceText.text = $"{data.Reward.Experience}";

            firstTimeBox.SetActive(data.Rating >= 3);

            for (int i = 0; i < 3; i++)
            {
                ratingImages[i].enabled = data.Rating > i;
            }

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            RemoveListener();
        }

        private void AddListener()
        {
            closeButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void RemoveListener()
        {
            closeButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            Dispose();
        }
    }

    public class UIStageResultPopupData
    {
        public string StageId;
        public int Rating;
        public CurrencyData Currency;
        public CurrencyData Reward;

        public UIStageResultPopupData(string stageId, int rating, CurrencyData currency, CurrencyData reward)
        {
            StageId = stageId;
            Rating = rating;
            Currency = currency;
            Reward = reward;
        }
    }
}
