using System;
using Magia.GameLogic.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Progression
{
    public class UIMainNode : MonoBehaviour
    {
        public event Action<UISkillDisplayData> EventNodeSelected;

        [SerializeField] private SkillNodeDataSO data;
        [SerializeField] private Button button;

        public void Initialize()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        public void Dispose()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        public UISkillDisplayData GetSkillDisplayData()
        {
            return new(data);
        }

        #region subscribe events
        private void OnButtonClicked()
        {
            EventNodeSelected?.Invoke(new(data));
        }
        #endregion
    }
}
