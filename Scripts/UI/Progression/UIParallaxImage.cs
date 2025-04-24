using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Visual
{
    public class UIParallaxImage : MonoBehaviour
    {
        private const string PARALLAX_KEY = "_Parallax";

        [SerializeField] private Image image;
        [SerializeField] private Material materialPrefab;
        [SerializeField] private Vector2 parallaxRange;

        private void Start()
        {
            image.material = new(materialPrefab);
        }

        private void OnDestroy()
        {
            Destroy(image.material);
        }


        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 normalized = new(
                Mathf.Lerp(parallaxRange.x, parallaxRange.y, mousePos.x / Screen.width),
                Mathf.Lerp(parallaxRange.x, parallaxRange.y, mousePos.y / Screen.height)
            );

            image.material.SetVector(PARALLAX_KEY, normalized);
        }
    }
}
