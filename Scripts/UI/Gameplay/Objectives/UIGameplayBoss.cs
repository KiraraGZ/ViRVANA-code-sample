using System.Collections.Generic;
using Magia.Enemy;
using Magia.Utilities.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIGameplayBoss : PoolObject<UIGameplayBoss>
    {
        private const string ACTIVE_KEY = "isActive";

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private RectTransform phasePrefab;
        [SerializeField] private Transform phaseContainer;

        [Header("UI")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider decaySlider;
        [SerializeField] private Image[] sliderFills;
        [SerializeField] private TMP_Text bossName;
        [SerializeField] private TMP_Text bossHeader;

        [SerializeField] private Color healthColor;
        [SerializeField] private Color barrierColor;

        [Header("Parameters")]
        [SerializeField] private float transitionSpeed = 0.1f;

        private float targetValue;
        private float hideTime;
        private const float HIDE_DELAY = 1.5f;

        private BaseEnemy boss;
        private List<RectTransform> phaseSplitters;

        public void Initialize(EnemySO _data, BaseEnemy _boss)
        {
            gameObject.SetActive(false);

            rectTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            rectTransform.localScale = Vector3.one;

            boss = _boss;
            boss.EventHealthUpdate += OnBossHealthUpdate;

            bossName.text = _data.Name;
            bossHeader.text = _data.Description;

            healthSlider.maxValue = boss.Health;
            healthSlider.value = boss.Health;
            decaySlider.maxValue = boss.Health;
            decaySlider.value = boss.Health;
            targetValue = boss.Health;

            SetPhaseBar();
        }

        public void Dispose()
        {
            if (boss is IPhaseChangeable phaseEnemy)
            {
                phaseEnemy.EventChangePhase -= OnChangePhase;

                for (int i = 0; i < phaseSplitters.Count; i++)
                {
                    Destroy(phaseSplitters[i].gameObject);
                }
            }

            boss.EventHealthUpdate -= OnBossHealthUpdate;
            boss = null;

            Return();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            animator.SetBool(ACTIVE_KEY, true);
        }

        public void Hide()
        {
            animator.SetBool(ACTIVE_KEY, false);
            hideTime = Time.time + HIDE_DELAY;
        }

        private void Update()
        {
            if (targetValue == 0 && Time.time >= hideTime)
            {
                gameObject.SetActive(false);
                return;
            }

            if (decaySlider.value > targetValue)
            {
                decaySlider.value -= transitionSpeed * decaySlider.maxValue * Time.deltaTime;
                Mathf.Clamp(decaySlider.value, targetValue, decaySlider.maxValue);
            }
        }

        private void SetPhaseBar()
        {
            if (boss is not IPhaseChangeable phaseEnemy) return;

            phaseEnemy.EventChangePhase += OnChangePhase;

            List<float> phases = phaseEnemy.GetPhaseChangeRatios();
            float bossHealthBarLength = healthSlider.GetComponent<RectTransform>().rect.width;
            Vector3 bossHealthBar = new(bossHealthBarLength, 0, 0);
            phaseSplitters = new();

            foreach (float phase in phases)
            {
                var phaseSplitter = Instantiate(phasePrefab, phaseContainer);
                phaseSplitter.transform.localPosition = Vector3.Lerp(-bossHealthBar / 2, bossHealthBar / 2, phase);
                phaseSplitters.Add(phaseSplitter);
            }
        }

        public void OnChangePhase(int phase)
        {
            for (int i = 0; i < phase; i++)
            {
                phaseSplitters[i].gameObject.SetActive(false);
            }
        }

        #region subscribe events
        private void OnBossHealthUpdate(ProgressBarInfo info)
        {
            healthSlider.maxValue = info.MaxValue;
            healthSlider.value = info.Value;
            targetValue = info.Value;

            for (int i = 0; i < sliderFills.Length; i++)
            {
                sliderFills[i].color = info.Barrier ? barrierColor : healthColor;
            }

            if (info.Barrier == true) return;
            if (info.Value > 0) return;

            targetValue = 0;
            Hide();
        }
        #endregion
    }
}