using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorReward : MonoBehaviour
    {
        public event Action<int, int> EventRewardChanged;

        [SerializeField] private Button button;
        [SerializeField] private TMP_InputField input;

        private int index;
        private bool enable; //This variable has no use atm.

        private bool isUpdateInfo;

        public void Initialize(int _index)
        {
            index = _index;

            AddListener();
        }

        public void Dispose()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            button.onClick.AddListener(OnToggleEnable);
            input.onValueChanged.AddListener(OnRewardChanged);
        }

        private void RemoveListener()
        {
            button.onClick.RemoveAllListeners();
            input.onValueChanged.RemoveAllListeners();
        }

        public void UpdateInfo(int value)
        {
            isUpdateInfo = true;

            input.text = value.ToString();

            isUpdateInfo = false;
        }

        public void SetEnable(bool _enable)
        {
            enable = _enable;
        }

        #region subscribe events
        private void OnToggleEnable()
        {
            enable = !enable;
            SetEnable(enable);
        }

        private void OnRewardChanged(string value)
        {
            if (isUpdateInfo) return;

            if (value == "")
            {
                EventRewardChanged?.Invoke(index, 0);
                return;
            }

            var amount = int.Parse(value);

            if (amount < 0) amount = 0;

            EventRewardChanged?.Invoke(index, amount);
        }
        #endregion
    }
}
