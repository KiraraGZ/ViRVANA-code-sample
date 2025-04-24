using System.Collections;
using Cinemachine;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Player.Utilities
{
    public class CameraHandler : MonoBehaviour
    {
        private const string TITLE_KEY = "Title";
        private const string STAGE_SELECT_KEY = "StageSelect";
        private const string GAMEPLAY_KEY = "Gameplay";

        [SerializeField] private CinemachineVirtualCamera titleVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera stageSelectVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera gameplayVirtualCamera;
        [SerializeField] private Animator animator;

        [SerializeField] private Camera uiCamera;
        public Camera UICamera => uiCamera;

        [SerializeField] private CameraZoom gameplayCamera;
        public Camera GameplayCamera => gameplayCamera.Camera;

        private CinemachinePOV gameplayPOV;
        private float aimVerticalDecel;
        private float aimHorizontalDecel;

        public void Initialize()
        {
            gameplayCamera.Initialize();

            gameplayPOV = gameplayVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
            aimVerticalDecel = gameplayPOV.m_VerticalAxis.m_DecelTime;
            aimHorizontalDecel = gameplayPOV.m_HorizontalAxis.m_DecelTime;
            SetHorizontalSensitivity(GameplayController.Instance.SettingManager.HorizontalSensitivity);
            SetVerticalSensitivity(GameplayController.Instance.SettingManager.VerticalSensitivity);
        }

        public void Dispose()
        {
            gameplayCamera.Dispose();

            gameplayPOV = null;
        }

        public void SetTitleCameraOnTransform(Transform holder)
        {
            titleVirtualCamera.transform.SetPositionAndRotation(holder.position, holder.rotation);
        }

        public void SetStageSelectCameraOnTransform(Transform holder)
        {
            stageSelectVirtualCamera.transform.SetPositionAndRotation(holder.position, holder.rotation);
        }

        public void SetGameplayCameraOnTransform(Transform player)
        {
            gameplayVirtualCamera.Follow = player;
            gameplayVirtualCamera.LookAt = player;
        }

        public void EnterStageSelectState()
        {
            animator.SetTrigger(STAGE_SELECT_KEY);
        }

        public void EnterTitleState()
        {
            animator.SetTrigger(TITLE_KEY);
        }

        #region stage select
        public void MoveStageSelectCamera(float value)
        {
            var scene = GameplayController.Instance.StageSelectManager.Scene;
            stageSelectVirtualCamera.transform.position = Vector3.Lerp(scene.CameraStartPoint.position, scene.CameraEndPoint.position, value);
        }
        #endregion

        #region gameplay
        public void EnterGameplayState()
        {
            animator.SetTrigger(GAMEPLAY_KEY);
        }

        public void TogglePause(bool isPause)
        {
            gameplayPOV.m_HorizontalAxis.m_DecelTime = isPause ? 0 : aimHorizontalDecel;
            gameplayPOV.m_VerticalAxis.m_DecelTime = isPause ? 0 : aimVerticalDecel;
        }

        public void SetHorizontalSensitivity(float value)
        {
            gameplayPOV.m_HorizontalAxis.m_MaxSpeed = value;
        }

        public void SetVerticalSensitivity(float value)
        {
            gameplayPOV.m_VerticalAxis.m_MaxSpeed = value;
        }

        public void UpdateVelocity(float speedMultiplier)
        {
            gameplayCamera.UpdateVelocity(speedMultiplier);
        }

        public void PerformSkill()
        {
            gameplayCamera.PerformSkill();
        }

        public void ReleaseSkill()
        {
            gameplayCamera.ReleaseSkill();
        }

        public void StartCameraShake(float intensity, float duration)
        {
            StartCoroutine(ShakeCamera(intensity, duration));
        }

        private IEnumerator ShakeCamera(float intensity, float duration)
        {
            var perlin = gameplayVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = intensity;

            yield return new WaitForSeconds(duration);

            perlin.m_AmplitudeGain = 0;
        }
        #endregion
    }
}
