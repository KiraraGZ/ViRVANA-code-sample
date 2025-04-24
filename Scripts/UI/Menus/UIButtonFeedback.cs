using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// quick and temporary solution for feedback on button hover
[RequireComponent(typeof(Button))]
public class UIButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleFactor = 1.15f;
    [SerializeField] private float duration = 0.2f;
    // [SerializeField] private Color hoverColor = Color.yellow;

    // cache
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * scaleFactor, duration).SetEase(Ease.OutCubic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetEase(Ease.OutCubic);
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
    }
}
