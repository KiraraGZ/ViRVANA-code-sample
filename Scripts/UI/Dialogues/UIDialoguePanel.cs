using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magia.UI
{
    public class UIDialoguePanel : MonoBehaviour
    {
        public event Action EventDialogueEnd;

        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;

        private List<string> currentDialogueList;
        private int currentDialogueIndex;
        private float dialogueTimer;

        public void Initialize()
        {
            gameObject.SetActive(true);
            dialogueBox.SetActive(false);
            currentDialogueList = null;
        }

        public void Dispose()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (currentDialogueList == null) return;

            dialogueTimer -= Time.deltaTime;

            if (currentDialogueIndex >= currentDialogueList.Count && dialogueTimer <= 0)
            {
                currentDialogueList = null;
                currentDialogueIndex = 0;
                dialogueBox.SetActive(false);
                EventDialogueEnd?.Invoke();
                return;
            }

            if (dialogueTimer > 0) return;

            dialogueText.text = currentDialogueList[currentDialogueIndex];
            dialogueTimer = 1.5f + currentDialogueList[currentDialogueIndex].Length * 0.05f;
            currentDialogueIndex++;
        }

        public void Play(List<string> dialogueList)
        {
            Stop();

            dialogueBox.SetActive(true);
            currentDialogueList = dialogueList;
            currentDialogueIndex = 0;
            dialogueTimer = 0;
        }

        public void Play(string dialogue)
        {
            Play(new List<string> { dialogue });
        }

        public void Stall(string dialogue)
        {
            Stop();

            dialogueBox.SetActive(true);
            dialogueText.text = dialogue;
        }

        public void Stop()
        {
            dialogueBox.SetActive(false);
            currentDialogueList = null;
            currentDialogueIndex = 0;
            dialogueTimer = 0;

            EventDialogueEnd?.Invoke();
        }
    }
}
