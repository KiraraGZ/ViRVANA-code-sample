using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.UI.Gameplay.Crosshair;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Magia.UI.Gameplay
{
    public class UIGameplayPanel : MonoBehaviour
    {
        public event Action EventPauseRetreatButtonClicked;
        public event Action EventPauseBackButtonClicked;
        public event Action<float> EventHorizontalSensitivityChanged;
        public event Action<float> EventVerticalSensitivityChanged;
        public event Action<float> EventMasterLevelChanged;
        public event Action<float> EventMusicLevelChanged;
        public event Action<float> EventSFXLevelChanged;
        public event Action<bool> EventUIToggleChanged;
        public event Action<int> EventLanguageChanged;

        [Header("Player")]
        [SerializeField] private UIGameplayPlayer player;

        [Header("Enemy")]
        [SerializeField] private UIGameplayObjective objective;

        [Header("Misc")]
        [SerializeField] private UIPausePopup pausePopup;
        [SerializeField] private UICrosshairManager crosshair;
        [SerializeField] private UIIndicatorManager indicator;

        private GameplayController gameplayController;

        public void Initialize(GameplayController _gameplayController, ObjectiveData[] objectives, StringTable localizedTable, Dictionary<string, EnemySO> enemyDict)
        {
            gameplayController = _gameplayController;

            gameObject.SetActive(true);

            player.Initialize(gameplayController.GetPlayer());
            objective.Initialize(objectives, localizedTable, enemyDict);
            crosshair.Initialize();
            indicator.Initialize();

            AddListener();
        }

        public void Dispose()
        {
            gameplayController = null;

            gameObject.SetActive(false);

            player.Dispose();
            objective.Dispose();
            crosshair.Dispose();
            indicator.Dispose();

            RemoveListener();
        }

        private void Update()
        {
            crosshair.UpdateLogic();
            player.UpdateLogic();
        }

        private void AddListener()
        {
            pausePopup.EventHorizontalSensitivityChanged += OnHorizontalSensitivityChanged;
            pausePopup.EventVerticalSensitivityChanged += OnVerticalSensitivityChanged;
            pausePopup.EventMasterLevelChanged += OnMasterLevelChanged;
            pausePopup.EventMusicLevelChanged += OnMusicLevelChanged;
            pausePopup.EventSFXLevelChanged += OnSFXLevelChanged;
            pausePopup.EventUIToggleChanged += OnUIToggleChanged;
            pausePopup.EventLanguageChanged += OnLanguageChanged;
            pausePopup.EventRetreatButtonClicked += OnPauseRetreatButtonClicked;
            pausePopup.EventBackButtonClicked += OnPauseBackButtonClicked;
        }

        private void RemoveListener()
        {
            pausePopup.EventHorizontalSensitivityChanged -= OnHorizontalSensitivityChanged;
            pausePopup.EventVerticalSensitivityChanged -= OnVerticalSensitivityChanged;
            pausePopup.EventMasterLevelChanged -= OnMasterLevelChanged;
            pausePopup.EventMusicLevelChanged -= OnMusicLevelChanged;
            pausePopup.EventSFXLevelChanged -= OnSFXLevelChanged;
            pausePopup.EventUIToggleChanged -= OnUIToggleChanged;
            pausePopup.EventLanguageChanged -= OnLanguageChanged;
            pausePopup.EventRetreatButtonClicked -= OnPauseRetreatButtonClicked;
            pausePopup.EventBackButtonClicked -= OnPauseBackButtonClicked;
        }

        public UISkillIconGroup GetSkillIcons()
        {
            return player.GetSkillIcons();
        }

        public UICrosshairManager GetCrosshairUI()
        {
            return crosshair;
        }

        public void UpdateObjective(ObjectiveInfoData progress)
        {
            objective.UpdateObjective(progress);
        }

        public void InitializeTutorialObjective(string name, UIObjectiveInfoData[] datas)
        {
            objective.InitializeTutorialObjective(name, datas);
        }

        public void UpdateTutorialObjective((int, int)[] checklist)
        {
            objective.UpdateTutorialObjective(checklist);
        }

        public UIDamageIndicator RentDamageIndicator(DamageFeedback feedback)
        {
            return indicator.RentDamageIndicator(feedback);
        }

        public UIMarkIndicator RentMarkIndicator(Transform target)
        {
            return indicator.RentMarkIndicator(target);
        }

        public void SetBossUI(EnemySO[] datas, BaseEnemy[] enemies)
        {
            objective.SetBossUI(datas, enemies);
        }

        public void DisplayBossBar()
        {
            objective.ShowBossBar();
        }

        public void DisplayCaptureProgress(int mode, float progress, float target)
        {
            objective.DisplayCaptureProgress(mode, progress, target);
        }

        public void CreateEnemyIndicator(Transform transform)
        {
            indicator.CreateEnemyIndicator(transform);
        }

        public void CreateBossIndicator(Transform transform)
        {
            indicator.CreateBossIndicator(transform);
        }

        public void CreateDestinationIndicator(Transform transform)
        {
            indicator.CreateDestinationIndicator(transform);
        }

        public void ToggleIndicatorVisible(Transform transform, bool visible)
        {
            indicator.ToggleIndicatorVisible(transform, visible);
        }

        public void RemoveIndicator(Transform transform)
        {
            indicator.RemoveIndicator(transform);
        }

        public void DisplayPausePopup()
        {
            pausePopup.Initialize(gameplayController.SettingManager);
        }

        public void HidePausePopup()
        {
            pausePopup.Dispose();
        }

        //TODO: improve logic to handle visible of these components.
        public void ToggleVisible(bool visible)
        {
            player.gameObject.SetActive(visible);
            objective.gameObject.SetActive(visible);
            crosshair.gameObject.SetActive(visible);
            indicator.gameObject.SetActive(visible);
        }

        #region subscribe events
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
            EventUIToggleChanged?.Invoke(value);
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
    }

    public class ProgressBarInfo
    {
        public int Value;
        public int MaxValue;
        public bool Barrier;

        public ProgressBarInfo(int value, int maxValue, bool barrier)
        {
            Value = value;
            MaxValue = maxValue;
            Barrier = barrier;
        }
    }
}
