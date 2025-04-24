using Magia.Utilities.Pooling;
using UnityEngine;

namespace Magia.Vfx
{
    public class ClockStarVfx : PoolObject<ClockStarVfx>
    {
        [SerializeField] private GameObject star;
        [SerializeField] private ParticleSystem particle;

        private const float lifetime = 0.3f;
        private float timer;
        private bool isStop;

        public void Initialize()
        {
            star.SetActive(true);
            particle.Play();
            isStop = false;
        }

        public void Fire()
        {
            star.SetActive(false);
            particle.Stop();
            isStop = true;
            timer = Time.time;
        }

        private void Dispose()
        {
            Return();
        }

        public void Update()
        {
            if (!isStop) return;
            if (Time.time < timer + lifetime) return;

            Dispose();
        }
    }
}
