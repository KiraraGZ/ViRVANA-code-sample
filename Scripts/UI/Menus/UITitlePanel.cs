using System;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UITitlePanel : MonoBehaviour
    {
        public event Action EventStartButtonClicked;
        public event Action EventSettingButtonClicked;
        public event Action EventCreditButtonClicked;

        [SerializeField] private Button startButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button creditButton;
        [SerializeField] private Button exitButton;

        public void Initialize()
        {
            gameObject.SetActive(true);

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            RemoveListener();
        }

        private void AddListener()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            settingButton.onClick.AddListener(OnSettingButtonClicked);
            creditButton.onClick.AddListener(OnCreditButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void RemoveListener()
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
            settingButton.onClick.RemoveListener(OnSettingButtonClicked);
            creditButton.onClick.RemoveListener(OnCreditButtonClicked);
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        #region subscribe methods
        private void OnStartButtonClicked()
        {
            EventStartButtonClicked?.Invoke();
            Dispose();
        }

        private void OnSettingButtonClicked()
        {
            EventSettingButtonClicked?.Invoke();
            Dispose();
        }

        private void OnCreditButtonClicked()
        {
            EventCreditButtonClicked?.Invoke();
            Dispose();
        }

        private void OnExitButtonClicked()
        {
            Application.Quit();
        }
        #endregion
    }
}
