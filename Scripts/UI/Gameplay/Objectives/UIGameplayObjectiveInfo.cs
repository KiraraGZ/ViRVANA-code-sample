using System;
using Magia.Utilities.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIGameplayObjectiveInfo : PoolObject<UIGameplayObjectiveInfo>
    {
        private const float DEFAULT_HEIGHT = 30f;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Image checkActive;
        [SerializeField] private Image checkInactive;

        private UIObjectiveType type;

        public float Initialize(UIObjectiveInfoData data)
        {
            descriptionText.text = data.Description;
            type = data.Type;

            float height = Math.Max(DEFAULT_HEIGHT, descriptionText.preferredHeight);
            height += type == UIObjectiveType.PROGRESS ? 30 : 0;

            SetProgress(type == UIObjectiveType.PROGRESS);
            SetCheck(type == UIObjectiveType.CHECK);

            rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, height);

            return height;
        }

        public void Dispose()
        {
            Return();
        }

        public void UpdateProgress(int progress, int targetNumber)
        {
            if (type == UIObjectiveType.PROGRESS)
            {
                progressSlider.value = (float)progress / targetNumber;
            }
            else
            {
                checkActive.enabled = targetNumber == progress;
            }
        }

        private void SetProgress(bool isProgress)
        {
            progressSlider.gameObject.SetActive(isProgress);
            progressSlider.value = 0;
        }

        private void SetCheck(bool isCheck)
        {
            checkInactive.enabled = isCheck;
            checkActive.enabled = false;
        }
    }

    public class UIObjectiveInfoData
    {
        public string Description;
        public UIObjectiveType Type;

        public UIObjectiveInfoData(string description, UIObjectiveType type)
        {
            Description = description;
            Type = type;
        }
    }

    public enum UIObjectiveType
    {
        PROGRESS,
        CHECK,
    }
}
