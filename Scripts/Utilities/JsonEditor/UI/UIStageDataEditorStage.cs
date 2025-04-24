using System;
using Magia.Utilities.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.Utilities.Json.Editor
{
    public class UIStageDataEditorStage : PoolObject<UIStageDataEditorStage>
    {
        public event Action<int> EventSelect;

        [SerializeField] private Button button;
        [SerializeField] private TMP_Text number;

        private int index;

        public void Initialize(int _index)
        {
            EventSelect = null;
            index = _index;
            number.text = (index + 1).ToString();

            AddListener();
        }

        public void Dispose()
        {
            EventSelect = null;

            RemoveListener();

            Return();
        }

        public void SelectColor(Color color)
        {
            button.image.color = color;
        }

        private void AddListener()
        {
            button.onClick.AddListener(OnSelect);
        }

        private void RemoveListener()
        {
            button.onClick.RemoveAllListeners();
        }

        #region subscribe events
        private void OnSelect()
        {
            EventSelect?.Invoke(index);
        }
        #endregion
    }
}
