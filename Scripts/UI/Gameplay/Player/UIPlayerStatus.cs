using Magia.Player;
using Magia.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIPlayerStatus : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private UIMaterialClip[] lifes;

        private PlayerController player;

        public void Initialize(PlayerController _player)
        {
            player = _player;

            healthSlider.maxValue = player.Data.StatsData.MaxHealth;

            for (int i = 0; i < lifes.Length; i++)
            {
                lifes[i].Initialize();
            }

            AddListener();

            UpdateInfo(player.ReusableData.StatusData);
        }

        public void Dispose()
        {
            for (int i = 0; i < lifes.Length; i++)
            {
                lifes[i].Dispose();
            }

            RemoveListener();

            player = null;
        }

        private void AddListener()
        {
            player.EventStatusChanged += OnStatusChanged;
            player.EventEnterGroundState += OnEnterGroundState;
            player.EventEnterAerialState += OnEnterAerialState;
        }

        private void RemoveListener()
        {
            player.EventStatusChanged -= OnStatusChanged;
            player.EventEnterGroundState -= OnEnterGroundState;
            player.EventEnterAerialState -= OnEnterAerialState;
        }

        private void UpdateInfo(PlayerStateStatusData data)
        {
            healthSlider.value = data.Health;

            for (int i = 0; i < lifes.Length; i++)
            {
                lifes[i].SetTargetClip(i < data.Life ? 1 : 0);
            }
        }

        private void SetRecoverAnimation(bool isRecovering)
        {

        }

        #region subscribe events
        private void OnStatusChanged(PlayerStateStatusData data)
        {
            UpdateInfo(data);
        }

        private void OnEnterGroundState()
        {
            SetRecoverAnimation(true);
        }

        private void OnEnterAerialState()
        {
            SetRecoverAnimation(false);
        }
        #endregion
    }
}
