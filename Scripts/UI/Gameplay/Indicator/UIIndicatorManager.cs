using System.Collections.Generic;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.UI.Gameplay
{
    public class UIIndicatorManager : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float borderOffset = 30f;

        [Header("Components")]
        [SerializeField] private Transform container;
        [SerializeField] private UITargetIndicator indicatorPrefab;
        [SerializeField] private UIDamageIndicator damagePrefab;
        [SerializeField] private UIMarkIndicator markPrefab;

        private Camera _camera;
        private Transform playerTransform;
        private Dictionary<Transform, UITargetIndicator> indicators;

        public void Initialize()
        {
            if (indicators != null) return;

            indicators = new();

            _camera = GameplayController.Instance.CameraHandler.GameplayCamera;
            playerTransform = GameplayController.Instance.GetPlayer().transform;
        }

        public void Dispose()
        {
            foreach (var pair in indicators)
            {
                pair.Value.Dispose();
            }

            indicators.Clear();
            indicators = null;
            _camera = null;
            playerTransform = null;
        }

        private void LateUpdate()
        {
            foreach (var pair in indicators)
            {
                Transform target = pair.Key;
                UITargetIndicator indicator = pair.Value;

                if (target.gameObject.activeInHierarchy == false) continue;

                UpdateIndicatorPosition(target, indicator);
            }
        }

        #region target indicator
        public void CreateEnemyIndicator(Transform transform)
        {
            CreateIndicator(transform, IndicatorType.ENEMY);
        }

        public void CreateBossIndicator(Transform transform)
        {
            CreateIndicator(transform, IndicatorType.BOSS);
        }

        public void CreateDestinationIndicator(Transform transform)
        {
            CreateIndicator(transform, IndicatorType.DESTINATION);
        }

        public void ToggleIndicatorVisible(Transform transform, bool visible)
        {
            if (indicators.TryGetValue(transform, out var indicator))
            {
                indicator.ToggleVisible(visible);
            }
        }

        private void CreateIndicator(Transform transform, IndicatorType type)
        {
            if (indicators == null)
            {
                Initialize();
            }

            var indicator = indicatorPrefab.Rent(Vector2.zero, Quaternion.identity, container);
            indicator.Initialize(type);

            indicators.Add(transform, indicator);
        }

        public void RemoveIndicator(Transform transform)
        {
            if (indicators.TryGetValue(transform, out var indicator))
            {
                indicator.Dispose();
                indicators.Remove(transform);
            }
        }
        #endregion

        #region damage indicator
        public UIDamageIndicator RentDamageIndicator(DamageFeedback feedback)
        {
            var indicator = damagePrefab.Rent(container);
            indicator.Initialize(feedback);
            return indicator;
        }
        #endregion

        #region mark indicator
        public UIMarkIndicator RentMarkIndicator(Transform target)
        {
            var mark = markPrefab.Rent(container);
            mark.Initialize(target);

            return mark;
        }
        #endregion

        #region private methods
        private void UpdateIndicatorPosition(Transform target, UITargetIndicator indicator)
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(target.position);
            Vector3 screenPos;
            float distance = (target.position - playerTransform.position).magnitude;

            if (viewportPos.z < 0)
            {
                screenPos = GetOffScreenBorderPosition(target.position);
                indicator.UpdateVisual(screenPos, true, distance);
                return;
            }

            screenPos = _camera.ViewportToScreenPoint(viewportPos) - new Vector3(Screen.width, Screen.height) / 2f;
            screenPos = ClampScreenBorder(screenPos);
            bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

            indicator.UpdateVisual(screenPos, isOffScreen, distance);
        }

        private Vector3 GetOffScreenBorderPosition(Vector3 targetWorldPos)
        {
            Vector3 dir = (targetWorldPos - _camera.transform.position).normalized;
            float screenWidth = Screen.width / 2;
            float screenHeight = Screen.height / 2;

            float x = dir.x >= 0 ? screenWidth - borderOffset : borderOffset - screenWidth;
            float y = dir.y >= 0 ? screenHeight - borderOffset : borderOffset - screenHeight;

            return new Vector3(x, y, 0);
        }

        private Vector3 ClampScreenBorder(Vector3 screenPos)
        {
            float screenWidth = Screen.width / 2;
            float screenHeight = Screen.height / 2;

            screenPos.x = Mathf.Clamp(screenPos.x, borderOffset - screenWidth, screenWidth - borderOffset);
            screenPos.y = Mathf.Clamp(screenPos.y, borderOffset - screenHeight, screenHeight - borderOffset);

            return screenPos;
        }
        #endregion
    }
}