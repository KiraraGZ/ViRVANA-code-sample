using Magia.Utilities.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UITargetIndicator : PoolObject<UITargetIndicator>
    {
        [Header("Visual Range")]
        [SerializeField] private float visibleRange = 50f;
        [SerializeField] private float falloffRange = 50f;

        [Header("Components")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;

        [Header("Sprite")]
        [SerializeField] private Sprite inScreen;
        [SerializeField] private Sprite offScreen;

        [Header("Type")]
        [SerializeField] private Color enemyColor;
        [SerializeField] private Color bossColor;
        [SerializeField] private Color destinationColor;

        private IndicatorType type;
        private bool isVisible;

        public void Initialize(IndicatorType _type)
        {
            type = _type;

            image.enabled = false;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = (type > IndicatorType.ENEMY ? 1.5f : 1f) * Vector3.one;
            isVisible = true;
        }

        public void Dispose()
        {
            Return();
        }

        public void ToggleVisible(bool visible)
        {
            isVisible = visible;
            image.enabled = visible;
        }

        public void UpdateVisual(Vector3 screenPosition, bool isOffScreen, float distance)
        {
            if (isVisible == false) return;

            UpdatePosition(screenPosition, isOffScreen);

            image.enabled = true;
            image.sprite = isOffScreen ? offScreen : inScreen;
            image.color = type switch
            {
                IndicatorType.ENEMY => GetEnemyColor(enemyColor, isOffScreen, distance),
                IndicatorType.BOSS => bossColor,
                IndicatorType.DESTINATION => destinationColor,
                _ => enemyColor,
            };
        }

        private void UpdatePosition(Vector3 screenPosition, bool isOffScreen)
        {
            rectTransform.localPosition = screenPosition;
            rectTransform.localRotation = Quaternion.identity;

            if (isOffScreen)
            {
                Vector3 dir = screenPosition.normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private Color GetEnemyColor(Color color, bool isOffScreen, float distance)
        {
            float alpha = isOffScreen ? 1 - Mathf.Clamp01((distance - visibleRange) / falloffRange)
                                      : 0;
            return new(color.r, color.g, color.b, alpha);
        }
    }

    public enum IndicatorType
    {
        ENEMY,
        BOSS,
        DESTINATION,
    }
}