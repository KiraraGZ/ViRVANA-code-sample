using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIGameplayObjectiveCapture : MonoBehaviour
    {
        private const string CHARGE_KEY = "IsCharge";
        private const string ACTIVE_KEY = "IsActive";

        [SerializeField] private TMP_Text stateText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Slider secondarySlider;

        [SerializeField] private Animator animator;

        public void Initialize()
        {
            progressSlider.value = 0;
            secondarySlider.value = 0;
            gameObject.SetActive(false);
        }

        public void Dispose()
        {
            gameObject.SetActive(false);
        }

        public void DisplayProgress(int mode, float progress, float target)
        {
            switch (mode)
            {
                case 0:
                    {
                        progressSlider.value = 0;
                        secondarySlider.value = 0;
                        gameObject.SetActive(false);
                        break;
                    }
                case 1:
                    {
                        gameObject.SetActive(true);
                        animator.SetBool(CHARGE_KEY, true);
                        stateText.text = "CHARGE";
                        secondarySlider.maxValue = target;
                        secondarySlider.value = progress;
                        break;
                    }
                case 2:
                    {
                        animator.SetBool(ACTIVE_KEY, true);
                        stateText.text = "ACTIVE";
                        progressSlider.maxValue = target;
                        progressSlider.value = progress;
                        break;
                    }
                case 3:
                    {
                        animator.SetBool(CHARGE_KEY, false);
                        animator.SetBool(ACTIVE_KEY, false);
                        stateText.text = "DISRUPTED";
                        secondarySlider.maxValue = target;
                        secondarySlider.value = progress;
                        break;
                    }
            }
        }
    }
}
