using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.UI.Gameplay;
using Magia.UI.Gameplay.Crosshair;
using Magia.UI.Menu;
using Magia.UI.Progression;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Magia.UI
{
    public class UIManager : MonoBehaviour
    {
        private const string UI_TABLE_KEY = "UI";
        private const string PROGRESSION_TABLE_KEY = "Progression";

        public event Action EventMenuStartButtonClicked;
        public event Action EventBackToMenuButtonClicked;
        public event Action EventStageChangeModeButtonClicked;
        public event Action EventStageStartButtonClicked;
        public event Action EventPauseRetreatButtonClicked;
        public event Action EventPauseBackButtonClicked;
        public event Action<float> EventHorizontalSensitivityChanged;
        public event Action<float> EventVerticalSensitivityChanged;
        public event Action<float> EventMasterLevelChanged;
        public event Action<float> EventMusicLevelChanged;
        public event Action<float> EventSFXLevelChanged;
        public event Action<int> EventLanguageChanged;

        [SerializeField] private UIMenuManager menuManager;
        [SerializeField] private UIStageSelectPanel stageSelectPanel;
        [SerializeField] private UIStageResultPopup stageResultPopup;
        [SerializeField] private UIProgressionPanel progressionPanel;
        [SerializeField] private UICurrencyHeader currencyHeader;
        [SerializeField] private UIGameplayPanel gameplayPanel;
        [SerializeField] private UIDialoguePanel dialoguePanel;

        [SerializeField] private UIWorldCanvas worldCanvas;

        private GameplayController gameplayController;

        private StringTable uiLocalizedTable;
        private StringTable progressionLocalizedTable;

        #region singleton
        private static UIManager instance;
        public static UIManager Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }
        #endregion

        public void Initialize(GameplayController _gameplayController)
        {
            gameplayController = _gameplayController;

            menuManager.Initialize(gameplayController);

            AddListener();

            LoadLocalizationTable();
        }

        public void Dispose()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            menuManager.EventStartButtonClicked += OnMenuStartButtonClicked;
            menuManager.EventHorizontalSensitivityChanged += OnHorizontalSensitivityChanged;
            menuManager.EventVerticalSensitivityChanged += OnVerticalSensitivityChanged;
            menuManager.EventMasterLevelChanged += OnMasterLevelChanged;
            menuManager.EventMusicLevelChanged += OnMusicLevelChanged;
            menuManager.EventSFXLevelChanged += OnSFXLevelChanged;
            menuManager.EventUIToggleChanged += OnUIToggleChanged;
            menuManager.EventLanguageChanged += OnLanguageChanged;

            stageSelectPanel.EventProgressionButtonClicked += DisplayProgressionPanel;
            stageSelectPanel.EventBackToMenuButtonClicked += OnBackToMenuButtonClicked;
            stageSelectPanel.EventStageChangeModeButtonClicked += OnStageChangeModeButtonClicked;
            stageSelectPanel.EventStageStartButtonClicked += OnStageStartButtonClicked;

            progressionPanel.EventStageSelectButtonClicked += DisplayStageSelectPanel;

            gameplayPanel.EventHorizontalSensitivityChanged += OnHorizontalSensitivityChanged;
            gameplayPanel.EventVerticalSensitivityChanged += OnVerticalSensitivityChanged;
            gameplayPanel.EventPauseRetreatButtonClicked += OnPauseRetreatButtonClicked;
            gameplayPanel.EventPauseBackButtonClicked += OnPauseBackButtonClicked;
            gameplayPanel.EventMasterLevelChanged += OnMasterLevelChanged;
            gameplayPanel.EventMusicLevelChanged += OnMusicLevelChanged;
            gameplayPanel.EventSFXLevelChanged += OnSFXLevelChanged;
            gameplayPanel.EventUIToggleChanged += OnUIToggleChanged;
            gameplayPanel.EventLanguageChanged += OnLanguageChanged;

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void RemoveListener()
        {
            menuManager.EventStartButtonClicked -= OnMenuStartButtonClicked;
            menuManager.EventHorizontalSensitivityChanged -= OnHorizontalSensitivityChanged;
            menuManager.EventVerticalSensitivityChanged -= OnVerticalSensitivityChanged;
            menuManager.EventMasterLevelChanged -= OnMasterLevelChanged;
            menuManager.EventMusicLevelChanged -= OnMusicLevelChanged;
            menuManager.EventSFXLevelChanged -= OnSFXLevelChanged;
            menuManager.EventUIToggleChanged -= OnUIToggleChanged;
            menuManager.EventLanguageChanged -= OnLanguageChanged;

            stageSelectPanel.EventProgressionButtonClicked -= DisplayProgressionPanel;
            stageSelectPanel.EventBackToMenuButtonClicked -= OnBackToMenuButtonClicked;
            stageSelectPanel.EventStageChangeModeButtonClicked -= OnStageChangeModeButtonClicked;
            stageSelectPanel.EventStageStartButtonClicked -= OnStageStartButtonClicked;

            progressionPanel.EventStageSelectButtonClicked -= DisplayStageSelectPanel;

            gameplayPanel.EventHorizontalSensitivityChanged -= OnHorizontalSensitivityChanged;
            gameplayPanel.EventVerticalSensitivityChanged -= OnVerticalSensitivityChanged;
            gameplayPanel.EventPauseRetreatButtonClicked -= OnPauseRetreatButtonClicked;
            gameplayPanel.EventPauseBackButtonClicked -= OnPauseBackButtonClicked;
            gameplayPanel.EventMasterLevelChanged -= OnMasterLevelChanged;
            gameplayPanel.EventMusicLevelChanged -= OnMusicLevelChanged;
            gameplayPanel.EventSFXLevelChanged -= OnSFXLevelChanged;
            gameplayPanel.EventUIToggleChanged -= OnUIToggleChanged;
            gameplayPanel.EventLanguageChanged -= OnLanguageChanged;

            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        #region stage select
        public void DisplayStageSelectPanel()
        {
            stageSelectPanel.Initialize();
            currencyHeader.Hide();
        }

        public void DisplayStageSelectPopup(UIStageSelectPopupData data)
        {
            stageSelectPanel.DisplayStageSelectPopup(data);
        }

        public void DisplayStageResultPopup(UIStageResultPopupData data)
        {
            stageResultPopup.Initialize(data);
        }

        private void DisplayProgressionPanel()
        {
            progressionPanel.Initialize(progressionLocalizedTable);
            currencyHeader.Display();
        }
        #endregion

        #region gameplay
        public void DisplayGameplayPanel(ObjectiveData[] objectives, Dictionary<string, EnemySO> enemyDict)
        {
            gameplayPanel.Initialize(gameplayController, objectives, uiLocalizedTable, enemyDict);
        }

        public void HideGameplayPanel()
        {
            gameplayPanel.Dispose();
        }

        public void DisplayPausePopup()
        {
            gameplayPanel.DisplayPausePopup();
        }

        public void HidePausePopup()
        {
            gameplayPanel.HidePausePopup();
        }

        public UISkillIconGroup GetSkillIcons()
        {
            return gameplayPanel.GetSkillIcons();
        }

        public UICrosshairManager GetCrosshairUI()
        {
            return gameplayPanel.GetCrosshairUI();
        }

        public UICurrencyHeader GetCurrencyHeader()
        {
            return currencyHeader;
        }

        public UIDialoguePanel GetDialoguePanel()
        {
            return dialoguePanel;
        }

        public void UpdateObjective(ObjectiveInfoData progress)
        {
            gameplayPanel.UpdateObjective(progress);
        }

        public void InitializeTutorialObjective(string name, UIObjectiveInfoData[] datas)
        {
            gameplayPanel.InitializeTutorialObjective(name, datas);
        }

        public void UpdateTutorialObjective((int, int)[] checklist)
        {
            gameplayPanel.UpdateTutorialObjective(checklist);
        }

        public void DisplayDamageFeedback(DamageFeedback feedback, Vector3 hitPos)
        {
            UIDamageIndicator indicator = gameplayPanel.RentDamageIndicator(feedback);
            worldCanvas.DisplayDamageFeedback(indicator, hitPos);
        }

        public UIMarkIndicator DisplayMarkIndicator(Transform target)
        {
            UIMarkIndicator mark = gameplayPanel.RentMarkIndicator(target);
            worldCanvas.DisplayMarkIndicator(mark, target);
            return mark;
        }

        public void SetBossUI(EnemySO[] datas, BaseEnemy[] enemies)
        {
            gameplayPanel.SetBossUI(datas, enemies);
        }

        /// <summary>
        /// This method does not care about the number and order of enemies. It will show enemy health bar in order.
        /// </summary>
        public void DisplayBossBar()
        {
            gameplayPanel.DisplayBossBar();
        }

        public void DisplayCaptureProgress(int mode, float progress, float target)
        {
            gameplayPanel.DisplayCaptureProgress(mode, progress, target);
        }

        public void CreateEnemyIndicator(Transform transform)
        {
            gameplayPanel.CreateEnemyIndicator(transform);
        }

        public void CreateBossIndicator(Transform transform)
        {
            gameplayPanel.CreateBossIndicator(transform);
        }

        public void CreateDestinationIndicator(Transform transform)
        {
            gameplayPanel.CreateDestinationIndicator(transform);
        }

        public void ToggleIndicatorVisible(Transform transform, bool visible)
        {
            gameplayPanel.ToggleIndicatorVisible(transform, visible);
        }

        public void RemoveIndicator(Transform transform)
        {
            gameplayPanel.RemoveIndicator(transform);
        }
        #endregion

        #region subscribe methods
        private void OnMenuStartButtonClicked()
        {
            menuManager.Dispose();
            EventMenuStartButtonClicked?.Invoke();
        }

        private void OnBackToMenuButtonClicked()
        {
            menuManager.Initialize(gameplayController);
            EventBackToMenuButtonClicked?.Invoke();
        }

        private void OnStageChangeModeButtonClicked()
        {
            EventStageChangeModeButtonClicked?.Invoke();
        }

        private void OnStageStartButtonClicked()
        {
            EventStageStartButtonClicked?.Invoke();
        }

        private void OnHorizontalSensitivityChanged(float value)
        {
            EventHorizontalSensitivityChanged?.Invoke(value);
        }

        private void OnVerticalSensitivityChanged(float value)
        {
            EventVerticalSensitivityChanged?.Invoke(value);
        }

        private void OnMasterLevelChanged(float value)
        {
            EventMasterLevelChanged?.Invoke(value);
        }

        private void OnMusicLevelChanged(float value)
        {
            EventMusicLevelChanged?.Invoke(value);
        }

        private void OnSFXLevelChanged(float value)
        {
            EventSFXLevelChanged?.Invoke(value);
        }

        private void OnUIToggleChanged(bool value)
        {
            //TODO: make playerpref to keep the state of the toggle.
            gameplayPanel.ToggleVisible(value);
        }

        public void OnLanguageChanged(int index)
        {
            EventLanguageChanged?.Invoke(index);
        }

        private void OnPauseRetreatButtonClicked()
        {
            EventPauseRetreatButtonClicked?.Invoke();
        }

        private void OnPauseBackButtonClicked()
        {
            EventPauseBackButtonClicked?.Invoke();
        }
        #endregion

        #region localization
        private void OnLocaleChanged(Locale newLocale)
        {
            LoadLocalizationTable();
        }

        private void LoadLocalizationTable()
        {
            LocalizationSettings.StringDatabase.GetTableAsync(UI_TABLE_KEY).Completed += OnUITableLoaded;
            LocalizationSettings.StringDatabase.GetTableAsync(PROGRESSION_TABLE_KEY).Completed += OnProgressionTableLoaded;
        }

        private void OnUITableLoaded(AsyncOperationHandle<StringTable> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;

            uiLocalizedTable = handle.Result;
        }

        private void OnProgressionTableLoaded(AsyncOperationHandle<StringTable> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;

            progressionLocalizedTable = handle.Result;
        }
        #endregion
    }
}
