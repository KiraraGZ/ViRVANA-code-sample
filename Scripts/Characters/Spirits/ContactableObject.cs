using System;
using System.Collections;
using Magia.Player;
using UnityEngine;

namespace Magia.Spirits
{
    [RequireComponent(typeof(Collider))]
    public class ContactableObject : MonoBehaviour
    {
        public event Action<ContactableObject> EventPlayerContacted;

        private const string CLIP_KEY = "_Clip";
        private const float FADE_DURATION = 0.5f;

        [SerializeField] private MeshRenderer body;
        [SerializeField] private MeshRenderer[] tentacles;
        [SerializeField] private Material[] materialPrefabs;

        private Material[] materials;

        public void Initialize()
        {
            materials = new Material[materialPrefabs.Length];

            for (int i = 0; i < materialPrefabs.Length; i++)
            {
                materials[i] = new(materialPrefabs[i]);
            }

            body.material = materials[0];

            for (int i = 0; i < tentacles.Length; i++)
            {
                tentacles[i].material = materials[1];
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < materials.Length; i++)
            {
                Destroy(materials[i]);
            }

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var _) == false) return;

            EventPlayerContacted?.Invoke(this);
            StartCoroutine(Fade(FADE_DURATION));
        }

        private IEnumerator Fade(float duration)
        {
            float lerp = 1;

            while (lerp > 0)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat(CLIP_KEY, lerp);
                }

                lerp -= Time.deltaTime / duration;
                yield return null;
            }

            Dispose();
        }
    }
}
