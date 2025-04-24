using Magia.Audio;
using Magia.Environment;
using Magia.GameLogic.Progression;
using Magia.Player;
using Magia.Player.Utilities;
using Magia.Skills;
using Magia.UI;
using Magia.UI.Gameplay;
using Magia.UI.Progression;
using Magia.Vfx;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Magia.Player.Utilities.PlayerInput;

namespace Magia.GameLogic
{
    public class GameplayController : MonoBehaviour
    {
        #region manager
        [SerializeField] private ProgressionManager progressionManager;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SoundManager soundManager;

        [SerializeField] private ObjectiveManager objectiveManager;
        public ObjectiveManager ObjectiveManager => objectiveManager;

        [SerializeField] private CharacterManager characterManager;
        public CharacterManager CharacterManager => characterManager;

        [SerializeField] private CameraHandler cameraHandler;
        public CameraHandler CameraHandler => cameraHandler;

        [SerializeField] private StageSelectManager stageSelectManager;
        public StageSelectManager StageSelectManager => stageSelectManager;

        [SerializeField] private EnvironmentController environmentManager;
        [SerializeField] private MapManager mapManager;
        [SerializeField] private VfxManager vfxManager;

        private DialogueVisualizer dialogueVisualizer;
        public DialogueVisualizer DialogueVisualizer => dialogueVisualizer;
        #endregion

        public SettingManager SettingManager { get; private set; }

        private bool isPause;

        #region singleton
        private static GameplayController instance;
        public static GameplayController Instance => instance;

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

        private void Start()
        {
            SettingManager = new();
            progressionManager.Initialize(uiManager.GetCurrencyHeader());
            stageSelectManager.Initialize();
            playerInput.Initialize();
            soundManager.SetVolume(SettingManager.SoundSetting);

            cameraHandler.Initialize();
            cameraHandler.SetTitleCameraOnTransform(mapManager.TitleSceneCamera);
            cameraHandler.SetStageSelectCameraOnTransform(stageSelectManager.Scene.CameraStartPoint);

            uiManager.Initialize(this);

            AddListener();
        }

        private void OnDestroy()
        {
            SettingManager = null;
            progressionManager.Dispose();
            stageSelectManager.Dispose();
            playerInput.Dispose();

            cameraHandler.Dispose();

            uiManager.Dispose();

            RemoveListener();
        }

        private void AddListener()
        {
            uiManager.EventMenuStartButtonClicked += OnOpenStageMenu;
            uiManager.EventBackToMenuButtonClicked += OnBackToMenu;
            uiManager.EventStageChangeModeButtonClicked += OnStageChangeMode;
            uiManager.EventStageStartButtonClicked += OnStageStarted;
            uiManager.EventPauseRetreatButtonClicked += OnStageRetreat;
            uiManager.EventPauseBackButtonClicked += OnTogglePausePopup;
            uiManager.EventHorizontalSensitivityChanged += OnHorizontalSensitivityChanged;
            uiManager.EventVerticalSensitivityChanged += OnVerticalSensitivityChanged;
            uiManager.EventMasterLevelChanged += OnMasterLevelChanged;
            uiManager.EventMusicLevelChanged += OnMusicLevelChanged;
            uiManager.EventSFXLevelChanged += OnSFXLevelChanged;
            uiManager.EventLanguageChanged += OnLanguageChanged;

            stageSelectManager.EventStageSelect += OnUpdateStagePopup;

            playerInput.PlayerActions.Menu.started += OnTogglePauseInput;
            playerInput.PauseActions.Menu.started += OnTogglePauseInput;
        }

        private void RemoveListener()
        {
            uiManager.EventMenuStartButtonClicked -= OnOpenStageMenu;
            uiManager.EventBackToMenuButtonClicked -= OnBackToMenu;
            uiManager.EventStageChangeModeButtonClicked -= OnStageChangeMode;
            uiManager.EventStageStartButtonClicked -= OnStageStarted;
            uiManager.EventPauseRetreatButtonClicked -= OnStageRetreat;
            uiManager.EventPauseBackButtonClicked -= OnTogglePausePopup;
            uiManager.EventHorizontalSensitivityChanged -= OnHorizontalSensitivityChanged;
            uiManager.EventVerticalSensitivityChanged -= OnVerticalSensitivityChanged;
            uiManager.EventMasterLevelChanged -= OnMasterLevelChanged;
            uiManager.EventMusicLevelChanged -= OnMusicLevelChanged;
            uiManager.EventSFXLevelChanged -= OnSFXLevelChanged;
            uiManager.EventLanguageChanged -= OnLanguageChanged;

            stageSelectManager.EventStageSelect -= OnUpdateStagePopup;

            playerInput.PlayerActions.Menu.started -= OnTogglePauseInput;
            playerInput.PauseActions.Menu.started -= OnTogglePauseInput;
        }

        #region subscribe events
        private void OnOpenStageMenu()
        {
            stageSelectManager.SetStoryMode();
            uiManager.DisplayStageSelectPanel();

            cameraHandler.EnterStageSelectState();
        }

        private void OnBackToMenu()
        {
            stageSelectManager.ClearScene();

            cameraHandler.EnterTitleState();
        }

        private void OnUpdateStagePopup(UIStageSelectPopupData data)
        {
            uiManager.DisplayStageSelectPopup(data);
        }

        private void OnStageChangeMode()
        {
            stageSelectManager.ChangeMode();
        }

        private void OnStageStarted()
        {
            stageSelectManager.ClearScene();
            //TODO: remove this and use stage data from stage map.
            var data = stageSelectManager.GetStageInitialData();

            mapManager.LoadGameplayScene(data.MapName, () =>
            {
                dialogueVisualizer = new();
                dialogueVisualizer.Initialize(uiManager.GetDialoguePanel());
                dialogueVisualizer.AddBuddyListener(characterManager.Buddy);

                mapManager.UnloadTitleScene();
                mapManager.StageMap.Initialize(characterManager.Player);

                characterManager.Initialize(data.MapData.Setting);
                characterManager.SetPreparedEnemies(mapManager.StageMap.GetSpawnerEnemies());

                environmentManager.Initialize(new(data.MapData.Setting.MapTime, Weather.Clear));
                vfxManager.Initialize();

                objectiveManager = new();
                objectiveManager.Initialize(data.MapData.Objectives, characterManager, mapManager.StageMap);
                objectiveManager.EventObjectiveUpdate += OnObjectiveUpdate;
                objectiveManager.EventStageEnded += OnStageEnded;

                SkillHandlerData skillHandlerData = new(
                    characterManager.Player,
                    uiManager.GetSkillIcons(),
                    uiManager.GetCrosshairUI(),
                    progressionManager.GetSkillTreeProgress()
                );
                characterManager.Player.Initialize(playerInput, skillHandlerData, cameraHandler);
                characterManager.Player.EventDamageDealt += OnPlayerDamageDealt;

                cameraHandler.SetGameplayCameraOnTransform(characterManager.Player.transform);
                cameraHandler.EnterGameplayState();

                uiManager.DisplayGameplayPanel(objectiveManager.Objectives, data.EnemyDict);
                // dialogueVisualizer.StartIntroDialogue(stageSelectManager.GetDialogueCollection().StageStart);

                Cursor.lockState = CursorLockMode.Locked;
            });
        }

        public void OnStageEnded(int rating)
        {
            CurrencyData reward = new(0, 0, 0);
            CurrencyData currency = progressionManager.GetCurrency();

            if (rating > 0)
            {
                reward = stageSelectManager.GetReward(rating);
                progressionManager.AddReward(reward);
                progressionManager.UpdateStageStatus(stageSelectManager.GetCurrentStageMapName(), rating);
                stageSelectManager.RefreshUnlockStages();
            }

            StopGameplay();
            uiManager.DisplayStageResultPopup(new(stageSelectManager.GetCurrentStageMapId(), rating, currency, reward));
        }

        public void OnStageRetreat()
        {
            OnTogglePausePopup();
            StopGameplay();
        }

        private void StopGameplay()
        {
            OnOpenStageMenu();

            mapManager.LoadTitleScene();
            mapManager.StageMap.Dispose();
            mapManager.UnloadScene();

            environmentManager.Dispose();
            vfxManager.Dispose();

            characterManager.Dispose();
            characterManager.Player.EventDamageDealt -= OnPlayerDamageDealt;

            objectiveManager.Dispose();
            objectiveManager.EventObjectiveUpdate -= OnObjectiveUpdate;
            objectiveManager.EventStageEnded -= OnStageEnded;
            objectiveManager = null;

            dialogueVisualizer.Dispose();
            dialogueVisualizer.RemoveBuddyListener(characterManager.Buddy);
            dialogueVisualizer = null;

            uiManager.HideGameplayPanel();

            Cursor.lockState = CursorLockMode.None;
        }

        #region gameplay ui
        public void OnObjectiveUpdate(ObjectiveInfoData progress)
        {
            uiManager.UpdateObjective(progress);
        }

        public void InitializeTutorialObjective(string name, UIObjectiveInfoData[] datas)
        {
            uiManager.InitializeTutorialObjective(name, datas);
        }

        public void UpdateTutorialObjective((int, int)[] checklist)
        {
            uiManager.UpdateTutorialObjective(checklist);
        }

        private void OnPlayerDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            uiManager.DisplayDamageFeedback(feedback, hitPos);
        }
        #endregion

        #region menu ui
        private void OnTogglePauseInput(InputAction.CallbackContext callback)
        {
            OnTogglePausePopup();
        }

        private void OnTogglePausePopup()
        {
            isPause = !isPause;

            if (isPause == true)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                playerInput.SwitchToPauseInput();
                uiManager.DisplayPausePopup();
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                playerInput.SwitchToPlayerInput();
                uiManager.HidePausePopup();
            }

            cameraHandler.TogglePause(isPause);
        }

        private void OnHorizontalSensitivityChanged(float value)
        {
            SettingManager.HorizontalSensitivity = value;
            cameraHandler.SetHorizontalSensitivity(value);
        }

        private void OnVerticalSensitivityChanged(float value)
        {
            SettingManager.VerticalSensitivity = value;
            cameraHandler.SetVerticalSensitivity(value);
        }

        private void OnMasterLevelChanged(float value)
        {
            SettingManager.MasterLevel = value;
            soundManager.SetVolume(SettingManager.SoundSetting);
        }

        private void OnMusicLevelChanged(float value)
        {
            SettingManager.MusicLevel = value;
            soundManager.SetVolume(SettingManager.SoundSetting);
        }

        private void OnSFXLevelChanged(float value)
        {
            SettingManager.SFXLevel = value;
            soundManager.SetVolume(SettingManager.SoundSetting);
        }

        private void OnLanguageChanged(int value)
        {
            SettingManager.LanguageIndex = value;
        }
        #endregion
        #endregion

        #region get data methods
        public SkillTreeProgressData GetUISkillTreeData()
        {
            return progressionManager.GetUISkillTreeData();
        }

        public PlayerController GetPlayer()
        {
            return characterManager.Player;
        }
        #endregion
    }
}
