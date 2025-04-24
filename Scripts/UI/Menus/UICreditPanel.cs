using System;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UICreditPanel : MonoBehaviour
    {
        public event Action EventBackButtonClicked;

        [SerializeField] private Button backButton;

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
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void RemoveListener()
        {
            backButton.onClick.RemoveAllListeners();
        }

        #region subscribe events
        private void OnBackButtonClicked()
        {
            EventBackButtonClicked?.Invoke();
            Dispose();
        }
        #endregion
    }
}
