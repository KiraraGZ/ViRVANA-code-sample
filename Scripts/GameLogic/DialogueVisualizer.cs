using System.Collections.Generic;
using Magia.Buddy;
using Magia.UI;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Magia.GameLogic
{
    public class DialogueVisualizer
    {
        private enum DialogueState
        {
            OUTRO = 0,
            TUTORIAL = 1,
            INTRO = 2,

            BUDDY_COMBAT = 10,
            ENEMY_WARN = 11,

            IDLE = 100,
        }

        private const string DIALOGUE_TABLE_NAME = "Dialogue";

        private const string TUTORIAL_DIALOGUES = "Tutorial";
        private readonly string[] BUDDY_DIALOGUES = { "Buddy_Mark", "Buddy_Compliment" };

        private int lastRandomIndex;
        private List<string> warnedList;

        private UIDialoguePanel dialoguePanel;
        private DialogueState currentState;
        private LocalizedStringDatabase localizedStringDatabase;

        public void Initialize(UIDialoguePanel _dialoguePanel)
        {
            dialoguePanel = _dialoguePanel;
            dialoguePanel.Initialize();
            dialoguePanel.EventDialogueEnd += () => TryChangeState(DialogueState.IDLE);

            lastRandomIndex = 0;
            warnedList = new();

            currentState = DialogueState.IDLE;
            localizedStringDatabase = LocalizationSettings.StringDatabase;
            localizedStringDatabase.NoTranslationFoundMessage = "";
        }

        public void Dispose()
        {
            dialoguePanel.EventDialogueEnd -= () => TryChangeState(DialogueState.IDLE);
            dialoguePanel.Dispose();
            dialoguePanel = null;

            warnedList = null;

            localizedStringDatabase = null;
        }

        //TODO: remove this method and let buddy script call this method by itself instead.
        public void AddBuddyListener(BuddyController buddy)
        {
            buddy.EventUseMark += () => PlayBuddyDialogue(0);
        }

        public void RemoveBuddyListener(BuddyController buddy)
        {
            buddy.EventUseMark -= () => PlayBuddyDialogue(0);
        }

        #region state dialogue
        public void StartIntroDialogue(DialogueData dialogues)
        {
            if (!TryChangeState(DialogueState.INTRO)) return;

            var dialogueList = GetDialogueList(dialogues.Key, 0, dialogues.Length);
            PlayDialogue(dialogueList);
        }

        public void PlayTutorialDialogue(int index, bool isStall)
        {
            if (!TryChangeState(DialogueState.TUTORIAL)) return;

            var dialogue = GetDialogue(TUTORIAL_DIALOGUES, index);

            if (isStall)
            {
                dialoguePanel.Stall(dialogue);
            }
            else
            {
                dialoguePanel.Play(dialogue);
            }
        }

        public void PlayWarnDialogue(string key, int length)
        {
            if (!TryChangeState(DialogueState.ENEMY_WARN)) return;

            if (warnedList.Contains(key)) return;

            PlayDialogue(GetDialogueList(key, 0, length));
            warnedList.Add(key);
        }

        public void PlayBuddyDialogue(int index)
        {
            if (!TryChangeState(DialogueState.BUDDY_COMBAT)) return;

            var dialogue = GetRandomDialogue(BUDDY_DIALOGUES[index], 3);
            PlayDialogue(dialogue);
        }

        public void StopDialogue()
        {
            dialoguePanel.Stop();
            TryChangeState(DialogueState.IDLE);
        }
        #endregion

        #region reuseable methods
        private void PlayDialogue(string dialogue)
        {
            List<string> dialogueList = new() { dialogue };

            PlayDialogue(dialogueList);
        }

        private void PlayDialogue(List<string> dialogueList)
        {
            dialoguePanel.Play(dialogueList);
        }

        private bool TryChangeState(DialogueState state)
        {
            if (currentState < state && state != DialogueState.IDLE) return false;

            currentState = state;

            return true;
        }
        #endregion

        #region Localization
        private List<string> GetDialogueList(string key, int index, int length)
        {
            List<string> list = new();

            for (int i = index; i < index + length; i++)
            {
                var dialogue = GetDialogue($"{key}_{i}");
                list.Add(dialogue);
            }

            return list;
        }

        private string GetDialogue(string key, int index)
        {
            return GetDialogue($"{key}_{index}");
        }

        private string GetRandomDialogue(string key, int length)
        {
            List<string> list = GetDialogueList(key, 0, length);
            int rand = Random.Range(0, length);

            if (rand == lastRandomIndex)
            {
                rand = (rand + 1) % length;
            }

            lastRandomIndex = rand;
            return list[rand];
        }

        private string GetDialogue(string key)
        {
            return localizedStringDatabase.GetLocalizedString(DIALOGUE_TABLE_NAME, key);
        }
        #endregion
    }
}
