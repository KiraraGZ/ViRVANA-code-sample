using Magia.GameLogic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Projectiles
{
    [Serializable]
    public class GateHomingProjectileData
    {
        public HomingProjectile Prefab;
        public HomingProjectileData HomingProjectileData;

        public float Amount = 6f;
        public float Delay = 2f;
        public float Lifetime = 4f;
    }

    public class GateHomingProjectile : Projectile
    {
        [SerializeField] private AnimationCurve sizeCurve;
        [SerializeField] private ParticleSystem particle;

        private GateHomingProjectileData gateData;

        private float fireTime;
        private GameObject target;
        private ProjectilePoolManager homingProjectilePool;

        //TODO: call base initialize method if base projectile behavior is needed.
        public void Initialize(GateHomingProjectileData data, ProjectilePoolManager pool, IDamageable _owner, GameObject _target)
        {
            gateData = data;
            homingProjectilePool = pool;
            owner = _owner;
            target = _target;

            vfx.transform.localScale = Vector3.one * sizeCurve.Evaluate(0);

            currentLifeTime = 0;
            fireTime = Time.time + data.Delay;
            expireTime = Time.time + data.Lifetime;
        }

        public override void Dispose()
        {
            gateData = null;
            homingProjectilePool = null;
            owner = null;
            target = null;

            base.Dispose();
        }

        protected override void FixedUpdate()
        {
            currentLifeTime += Time.deltaTime;
            vfx.transform.localScale = Vector3.one * sizeCurve.Evaluate(currentLifeTime / gateData.Lifetime);

            if (Time.time >= fireTime)
            {
                Fire();
                SpawnHitVfx();
                fireTime = expireTime + 1;
            }

            if (Time.time >= expireTime)
            {
                OnProjectileExpire();
                Dispose();
                return;
            }
        }

        private void Fire()
        {
            for (int i = 0; i < gateData.Amount; i++)
            {
                Vector3 direction = Random.insideUnitSphere;
                HomingProjectile projectile = homingProjectilePool.Rent(transform.position, Quaternion.LookRotation(direction)) as HomingProjectile;
                projectile.Initialize(gateData.HomingProjectileData, direction, owner, target);
            }

            particle.Play();
            PlaySound(initSound);
        }

        protected override void OnCollisionEnter(Collision collision)
        {

        }
    }
}
