using Magia.GameLogic;
using System;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class ExplodeProjectileData
    {
        public ProjectileData ProjectileData;
        public float VerticalAcceleration = 15f;
        public float ExplodeRadius = 30f;

        public ExplodeProjectileData(ExplodeProjectileData data)
        {
            ProjectileData = new(data.ProjectileData);
            ExplodeRadius = data.ExplodeRadius;
            VerticalAcceleration = data.VerticalAcceleration;
        }
    }

    public class ExplodeProjectile : Projectile
    {
        protected ExplodeProjectileData explodeData;

        [SerializeField] private GameObject explodePrefab;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material material;

        private float timeElapsed;
        protected bool isActive = false;

        public void Initialize(ExplodeProjectileData _data, Vector3 _direction, IDamageable _owner)
        {
            explodeData = _data;
            meshRenderer.material = new(material);
            isActive = true;

            base.Initialize(_data.ProjectileData, _direction, _owner);
        }

        public override void Dispose()
        {
            timeElapsed = 0;
            Destroy(meshRenderer.material);
            meshRenderer.material = null;
            isActive = false;

            base.Dispose();
        }

        public void SetLogicActive(bool _isActive)
        {
            isActive = _isActive;
        }

        protected override void FixedUpdate()
        {
            if (!isActive) return;

            if (Time.time >= expireTime)
            {
                Explode();
                OnProjectileExpire();
                Dispose();
            }

            timeElapsed += Time.deltaTime;
            meshRenderer.material.SetFloat("_Clip", timeElapsed / explodeData.ProjectileData.Lifetime);

            rb.AddForce(explodeData.VerticalAcceleration * Time.deltaTime * Vector3.down, ForceMode.VelocityChange);
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!isActive) return;

            //TODO: make upgrade node that can deal extra damage to the hit target.
            if (collision.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                Damage damage = new(data.Damage);
                DamageFeedback feedback = damageable.TakeDamage(damage, collision.GetContact(0).point, collision.collider.transform.position - transform.position, owner);

                if (feedback.IsHit == false) return;

                OnProjectileHit(feedback, collision.collider.ClosestPoint(transform.position));
            }

            Explode();

            OnProjectileExpire();
            Dispose();
        }

        protected virtual void Explode()
        {
            var explode = Instantiate(explodePrefab, transform.position, Quaternion.identity);
            explode.transform.localScale = Vector3.one * explodeData.ExplodeRadius;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explodeData.ExplodeRadius);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (!hitColliders[i].TryGetComponent<IDamageable>(out var damageable)) continue;

                Damage damage = new(explodeData.ProjectileData.Damage);
                var feedback = damageable.TakeDamage(damage, hitColliders[i].transform.position, hitColliders[i].transform.position - transform.position, owner);

                if (feedback.IsHit == false) continue;
                if (feedback.Amount != 0)
                {
                    OnProjectileHit(feedback, hitColliders[i].ClosestPoint(transform.position));
                }
            }
        }
    }
}