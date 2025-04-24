using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Magia.Vfx
{
    public class DecalVfx : MonoBehaviour
    {
        private const string CLIP_KEY = "_Clip";

        [SerializeField] private DecalProjector projector;
        [SerializeField] private AnimationCurve emissionCurve;

        private float lifetime;
        private float time;

        private VfxManager manager;

        public void Initialize(Material material, float _lifetime, VfxManager _manager)
        {
            projector.material = new(material);
            var rand = Random.Range(1.5f, 2f);
            projector.size = new(rand, rand, 5f);
            projector.pivot = new(0, 0, 1f);
            projector.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            time = 0;
            lifetime = _lifetime;
            manager = _manager;
        }

        public void Dispose()
        {
            Destroy(projector.material);
            gameObject.SetActive(false);
            manager.ReturnDecal(this);
        }

        private void Update()
        {
            time += Time.deltaTime;
            projector.material.SetFloat(CLIP_KEY, emissionCurve.Evaluate(time / lifetime));

            if (time < lifetime) return;

            Dispose();
        }
    }
}
