using Magia.GameLogic;
using Magia.Utilities.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIDamageIndicator : PoolObject<UIDamageIndicator>
    {
        private const float LIFETIME = 1.5f;

        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image image;

        [Header("Element")]
        [SerializeField] private Color magia;
        [SerializeField] private Color lightning;
        [SerializeField] private Color other;
        [SerializeField] private Color unaffected;

        private float disposeTime;

        public void Initialize(DamageFeedback feedback)
        {
            disposeTime = Time.time + LIFETIME;

            var color = feedback.Element switch
            {
                ElementType.Magia => magia,
                ElementType.Lightning => lightning,
                _ => other,
            };

            color = Color.Lerp(unaffected, color, feedback.Weakness * 0.7f);
            text.color = color;
            image.color = color;
            image.enabled = feedback.Weakness > 1;

            text.text = $"{(int)(feedback.Amount * feedback.Weakness)}";

            if (feedback.Weakness > 1f)
            {
                text.text += "!";
            }

            if (feedback.Weakness > 2f)
            {
                text.text += "!";
            }

            animator.SetBool("Crit", feedback.Weakness > 2f);
        }

        public void Dispose()
        {
            disposeTime = 0;

            Return();
        }

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);

            if (Time.time < disposeTime) return;

            Dispose();
        }
    }
}
