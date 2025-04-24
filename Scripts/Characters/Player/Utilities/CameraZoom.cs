using Cinemachine;
using UnityEngine;

namespace Magia.Player.Utilities
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        public Camera Camera => _camera;

        [Header("Zoom")]
        [SerializeField] private float defaultDistance = 6f;
        [SerializeField] private float minimumDistance = 1f;
        [SerializeField] private float maximumDistance = 6f;
        [SerializeField] private float smoothing = 4f;
        [SerializeField] private float zoomSensitivity = 1f;
        [SerializeField] private float skillPerformZoomDistance = 2f;

        [Header("FOV")]
        [SerializeField] private float defaultFOV = 60f;
        [SerializeField] private float multiplyFOV = 20f;
        [SerializeField] private float smoothingFOV = 1.5f;

        private CinemachineVirtualCamera virtualCamera;
        private CinemachineFramingTransposer framingTransposer;
        private CinemachineInputProvider inputProvider;

        private float zoomValue;
        private float targetDistance;
        private float currentDistance;
        private float lerpedZoomValue;
        private float additionalZoom;

        private float targetFOV;
        private float currentFOV;
        private float lerpedFOVValue;

        public void Initialize()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            inputProvider = GetComponent<CinemachineInputProvider>();

            targetDistance = defaultDistance;
            targetFOV = defaultFOV;
        }

        public void Dispose()
        {
            virtualCamera = null;
            framingTransposer = null;
            inputProvider = null;
        }

        private void Update()
        {
            Zoom();
            FOV();
        }

        private void Zoom()
        {
            zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;
            targetDistance = Mathf.Clamp(targetDistance + zoomValue, minimumDistance, maximumDistance) + additionalZoom;
            currentDistance = framingTransposer.m_CameraDistance;

            if (currentDistance == targetDistance) return;

            lerpedZoomValue = Mathf.Lerp(currentDistance, targetDistance, smoothing * Time.deltaTime);
            framingTransposer.m_CameraDistance = lerpedZoomValue;
        }

        private void FOV()
        {
            currentFOV = virtualCamera.m_Lens.FieldOfView;

            if (currentFOV == targetFOV) return;

            lerpedFOVValue = Mathf.Lerp(currentFOV, targetFOV, smoothingFOV * Time.deltaTime);
            virtualCamera.m_Lens.FieldOfView = lerpedFOVValue;
        }

        public void UpdateVelocity(float speedMultiplier)
        {
            var ratio = Mathf.Clamp(speedMultiplier - 1, 0, 2);

            targetFOV = defaultFOV + ratio * multiplyFOV;
        }

        public void PerformSkill()
        {
            additionalZoom = skillPerformZoomDistance;
        }

        public void ReleaseSkill()
        {
            additionalZoom = 0;
        }
    }
}
