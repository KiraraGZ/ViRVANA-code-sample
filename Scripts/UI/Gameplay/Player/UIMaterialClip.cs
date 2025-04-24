using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIMaterialClip : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Material material;
        [SerializeField] private float dissolveTime = 0.5f;

        private float currentClip;
        private float targetClip;

        public void Initialize(float clip = 1f)
        {
            image.material = new(material);
            currentClip = clip;
            targetClip = clip;
        }

        public void Dispose()
        {
            Destroy(image.material);
        }

        private void Update()
        {
            currentClip += (targetClip == 0 ? -1 : 1) / dissolveTime * Time.deltaTime;
            currentClip = Mathf.Clamp01(currentClip);
            image.material.SetFloat("_Clip", currentClip);
        }

        public void SetTargetClip(float clip)
        {
            targetClip = clip;
            image.enabled = clip == 1;
        }
    }
}
