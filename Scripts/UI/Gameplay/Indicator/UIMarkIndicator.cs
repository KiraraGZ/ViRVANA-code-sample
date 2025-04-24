using System.Collections;
using DG.Tweening;
using Magia.Utilities.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIMarkIndicator : PoolObject<UIMarkIndicator>
    {
        [Header("Components")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image leftImage;
        [SerializeField] private Image rightImage;

        [SerializeField] private Color firstColor;
        [SerializeField] private Color secondColor;

        private Transform target;
        private Coroutine coroutine;

        public void Initialize(Transform _target)
        {
            target = _target;
            rectTransform.localScale = Vector3.one;
            leftImage.color = Color.white;
            rightImage.color = Color.white;
        }

        public void Dispose()
        {
            coroutine = null;

            DOTween.Kill(rectTransform);
            DOTween.Kill(leftImage);
            DOTween.Kill(rightImage);

            Return();
        }

        private void Update()
        {
            transform.position = target.position;
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }

        public void UpdateStack(int progress, int target)
        {
            float targetScale = 1f;

            switch (progress)
            {
                case 1:
                    leftImage.color = firstColor;
                    rightImage.color = Color.white;
                    break;
                case 2:
                    leftImage.color = firstColor;
                    rightImage.color = firstColor;
                    break;
                case 3:
                    leftImage.color = secondColor;
                    rightImage.color = secondColor;
                    targetScale = 1.5f;
                    break;
                case 4:
                    Pop();
                    return;
            }

            DOTween.Kill(rectTransform);
            DOTween.Sequence()
                .Append(rectTransform.DOScale(targetScale + 1f, 0.3f)).SetEase(Ease.OutBack)
                .Append(rectTransform.DOScale(targetScale, 0.3f));
        }

        public void Pop()
        {
            if (coroutine != null) return;

            coroutine = StartCoroutine(PopAnimation());
        }

        private IEnumerator PopAnimation()
        {
            DOTween.Kill(rectTransform);
            rectTransform.DOScale(3f, 0.5f).SetEase(Ease.OutBack);
            leftImage.DOFade(0f, 0.5f);
            rightImage.DOFade(0f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            Dispose();
        }
    }
}
