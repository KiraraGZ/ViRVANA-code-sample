using System;
using Magia.GameLogic;
using TMPro;
using UnityEngine;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorHuntObjective : MonoBehaviour
    {
        // public event Action<HuntObjectiveData> EventObjectiveChanged;

        [SerializeField] private TMP_InputField goal;

        private bool isUpdateInfo;

        public void Initialize()
        {
            gameObject.SetActive(true);

            AddListener();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);

            RemoveListener();
        }

        private void AddListener()
        {
            goal.onValueChanged.AddListener(OnInputChanged);
        }

        private void RemoveListener()
        {
            goal.onValueChanged.RemoveAllListeners();
        }

        // public void UpdateInfo(HuntObjectiveData data)
        // {
        //     isUpdateInfo = true;

        //     goal.text = data.DefeatAmount.ToString();

        //     isUpdateInfo = false;
        // }

        private void UpdateData()
        {
            if (isUpdateInfo) return;

            // EventObjectiveChanged?.Invoke(new HuntObjectiveData
            // {
            //     DefeatAmount = int.Parse(goal.text),
            // });
        }

        #region subscribe events
        private void OnInputChanged(string _)
        {
            UpdateData();
        }
        #endregion
    }
}
