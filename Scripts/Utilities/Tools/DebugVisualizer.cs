using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magia.Utilities.Tools
{
    public class DebugVisualizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text prefab;

        private Dictionary<string, int> logs;
        private List<TMP_Text> logTexts;

        private static DebugVisualizer instance;
        public static DebugVisualizer Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            logs = new();
            logTexts = new();
        }

        public void UpdateLog(string key, string value)
        {
            if (!logs.TryGetValue(key, out int index))
            {
                logs.Add(key, logs.Count);
                index = logs[key];

                var text = Instantiate(prefab, transform);
                logTexts.Add(text);
            }

            logTexts[index].text = $"{key}\n{value}";
        }

        public void ConsoleLog(string log)
        {
            Debug.Log(log);
        }
    }
}
