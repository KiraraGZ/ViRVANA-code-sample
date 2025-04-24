using System;
using Magia.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI
{
    public class UIStageSelectPanel : MonoBehaviour
    {
        public event Action EventProgressionButtonClicked;
        public event Action EventBackToMenuButtonClicked;
        public event Action EventStageChangeModeButtonClicked;
        public event Action EventStageStartButtonClicked;

        [SerializeField] private UIStageSelectPopup stagePopup;
        [SerializeField] private Button backButton;
        [SerializeField] private Button progressionButton;
        [SerializeField] private Scrollbar cameraScrollbar;

        public void Initialize()
        {
            gameObject.SetActive(true);

            stagePopup.Initialize();

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            RemoveListener();
        }

        private void AddListener()
        {
            stagePopup.EventChangeModeButtonClicked += OnStageChangeModeButtonClicked;
            stagePopup.EventStartButtonClicked += OnStageStartButtonClicked;

            progressionButton.onClick.AddListener(OnProgressionButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            cameraScrollbar.onValueChanged.AddListener(OnCameraSliderMoved);
        }

        private void RemoveListener()
        {
            stagePopup.EventChangeModeButtonClicked -= OnStageChangeModeButtonClicked;
            stagePopup.EventStartButtonClicked -= OnStageStartButtonClicked;

            progressionButton.onClick.RemoveListener(OnProgressionButtonClicked);
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            cameraScrollbar.onValueChanged.RemoveListener(OnCameraSliderMoved);
        }

        public void DisplayStageSelectPopup(UIStageSelectPopupData data)
        {
            stagePopup.Display(data);
        }

        #region subscribe events
        private void OnProgressionButtonClicked()
        {
            EventProgressionButtonClicked?.Invoke();

            Dispose();
        }

        private void OnBackButtonClicked()
        {
            EventBackToMenuButtonClicked?.Invoke();

            Dispose();
        }

        private void OnStageChangeModeButtonClicked()
        {
            EventStageChangeModeButtonClicked?.Invoke();
        }

        private void OnStageStartButtonClicked()
        {
            EventStageStartButtonClicked?.Invoke();

            Dispose();
        }

        private void OnCameraSliderMoved(float value)
        {
            GameplayController.Instance.CameraHandler.MoveStageSelectCamera(value);
        }
        #endregion
    }
}
