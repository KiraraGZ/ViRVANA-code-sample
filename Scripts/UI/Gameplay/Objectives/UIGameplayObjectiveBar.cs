using Magia.Utilities.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIGameplayObjectiveBar : MonoBehaviour
    {
        private const float DEFAULT_HEIGHT = 50f;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private Image checkActive;
        [SerializeField] private Image checkInactive;

        [SerializeField] private UIGameplayObjectiveInfo infoPrefab;

        [SerializeField] private RectTransform container;

        private UIGameplayObjectiveInfo[] infos;

        public void Initialize(string objectiveName, UIObjectiveInfoData[] datas)
        {
            var height = Initialize(objectiveName);

            infos = new UIGameplayObjectiveInfo[datas.Length];

            for (int i = 0; i < datas.Length; i++)
            {
                infos[i] = infoPrefab.Rent(container);
                infos[i].UpdateProgress(0, 1);

                infos[i].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                infos[i].transform.localScale = Vector3.one;
                infos[i].transform.SetSiblingIndex(i);
                height += infos[i].Initialize(datas[i]);
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }

        public float Initialize(string name)
        {
            ClearInfos();

            checkActive.enabled = false;

            if (name == "")
            {
                objectiveText.gameObject.SetActive(false);
                return 0;
            }
            else
            {
                objectiveText.gameObject.SetActive(true);
                objectiveText.text = name;
                return DEFAULT_HEIGHT;
            }
        }

        public void Dispose()
        {
            ClearInfos();
        }

        public void UpdateProgress(int progress, int targetNumber)
        {
            checkActive.enabled = progress >= targetNumber;

            if (infos == null) return;

            infos[0].UpdateProgress(progress, targetNumber);
        }

        public void UpdateCheckList((int progress, int targetNumber)[] checklist)
        {
            bool isComplete = true;

            for (int i = 0; i < infos.Length; i++)
            {
                if (i >= checklist.Length) break;
                if (checklist[i].progress < checklist[i].targetNumber)
                {
                    isComplete = false;
                }

                infos[i].UpdateProgress(checklist[i].progress, checklist[i].targetNumber);
            }

            checkActive.enabled = isComplete;
        }

        private void ClearInfos()
        {
            if (infos == null) return;

            for (int i = 0; i < infos.Length; i++)
            {
                infos[i].Dispose();
            }

            infos = null;
        }
    }
}
