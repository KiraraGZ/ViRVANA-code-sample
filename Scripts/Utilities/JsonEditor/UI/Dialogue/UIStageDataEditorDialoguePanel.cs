using System;
using Magia.GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorDialoguePanel : MonoBehaviour
    {
        public event Action<DialogueCollection> EventDialogueChanged;

        private static string DIALOGUE_TABLE_NAME = "Dialogue";

        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_InputField keyInput;
        [SerializeField] private TMP_InputField lengthInput;
        [SerializeField] private TMP_Text dialogueText;

        private DialogueCollection dialogues;
        private LocalizedStringDatabase localizedStringDatabase;

        public void Initialize()
        {
            gameObject.SetActive(true);

            localizedStringDatabase = LocalizationSettings.StringDatabase;
            localizedStringDatabase.NoTranslationFoundMessage = "";

            UpdateInfo();

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            localizedStringDatabase = null;

            RemoveListener();
        }

        private void AddListener()
        {
            closeButton.onClick.AddListener(Dispose);
            keyInput.onValueChanged.AddListener(OnValueUpdate);
            lengthInput.onValueChanged.AddListener(OnValueUpdate);
        }

        private void RemoveListener()
        {
            closeButton.onClick.RemoveAllListeners();
            keyInput.onValueChanged.RemoveAllListeners();
            lengthInput.onValueChanged.RemoveAllListeners();
        }

        public void UpdateInfo(DialogueCollection _dialogues)
        {
            if (localizedStringDatabase == null) return;

            dialogues = new(_dialogues);
            keyInput.text = dialogues.StageStart.Key;
            lengthInput.text = dialogues.StageStart.Length.ToString();

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            dialogueText.text = "";

            if (keyInput.text == "") return;

            var length = int.Parse(lengthInput.text);

            for (int i = 0; i < length; i++)
            {
                dialogueText.text += $"{i.ToString().PadLeft(2, '0')} - {GetLocalizedString($"{keyInput.text}_{i}")} \n\n";
            }
        }

        private string GetLocalizedString(string key)
        {
            return localizedStringDatabase.GetLocalizedString(DIALOGUE_TABLE_NAME, key);
        }

        #region subscribe events
        private void OnValueUpdate(string value)
        {
            if (value == "") return;

            dialogues.StageStart.Key = keyInput.text;
            dialogues.StageStart.Length = int.Parse(lengthInput.text);

            UpdateInfo();

            EventDialogueChanged?.Invoke(dialogues);
        }
        #endregion
    }
}
