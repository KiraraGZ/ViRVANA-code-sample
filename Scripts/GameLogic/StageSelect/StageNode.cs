using System;
using DG.Tweening;
using Magia.Utilities.Pooling;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Magia.GameLogic
{
    [RequireComponent(typeof(SphereCollider))]
    public class StageNode : PoolObject<StageNode>, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<int> EventStageNodeClicked;

        private const float DURATION = 0.1f;

        [Header("Visual")]
        [SerializeField] private float minSize = 100f;
        [SerializeField] private float maxSize = 120f;

        private int index;
        private float scaleFactor;
        private StageStatus stageStatus;

        private void Start()
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }

        public void Display(int _index, StageStatus _stageStatus)
        {
            index = _index;
            stageStatus = _stageStatus;

            SetVfx();
        }

        public void Hide()
        {
            transform.DOScale(0, DURATION).SetEase(Ease.OutCubic);
        }

        private void SetVfx()
        {
            //TODO: Improve scale adjustment logic.
            if (stageStatus.IsUnlocked)
            {
                // anim.SetBool("isActive", true);
                gameObject.SetActive(true);
                scaleFactor = Random.Range(minSize, maxSize);
                transform.DOScale(scaleFactor, DURATION).SetEase(Ease.OutCubic);
            }
            else
            {
                // anim.SetBool("isActive", false);
                gameObject.SetActive(false);
            }
        }

        #region event methods
        public void OnPointerClick(PointerEventData eventData)
        {
            if (stageStatus.IsUnlocked == false) return;

            EventStageNodeClicked?.Invoke(index);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(scaleFactor * 1.05f, DURATION).SetEase(Ease.OutCubic);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(scaleFactor, DURATION).SetEase(Ease.OutCubic);
        }
        #endregion
    }
}