using System;
using Magia.Enemy;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class SpiritProjectileData
    {
        public int Health = 30;
        public float RaturnAccelerate = 25f;
        public float ReturnAngularSpeed = 120f;
        public Damage ReturnDamage;
    }

    public class SpiritProjectile : HomingProjectile, IDamageable
    {
        private const string CLIP_KEY = "_Clip";
        private const string RETURN_KEY = "_IsReturn";

        private SpiritProjectileData spiritData;

        [Header("VFX")]
        [SerializeField] private Material spiritMaterial;
        [SerializeField] private Renderer _renderer;

        private int health;
        private GameObject ownerObject;

        public void Initialize(SpiritProjectileData _spiritData, HomingProjectileData _homingData, Vector3 _direction, BaseEnemy _owner, GameObject _target)
        {
            spiritData = _spiritData;
            health = _spiritData.Health;
            ownerObject = _owner.gameObject;

            Initialize(_homingData, _direction, _owner, _target);

            _renderer.material = new(spiritMaterial);
            UpdateMaterial();
        }

        public override void Dispose()
        {
            base.Dispose();

            Destroy(_renderer.material);
        }

        private void ReturnToEnemy()
        {
            target = ownerObject;
            currentLifeTime = 0;
            expireTime = Time.time + data.Lifetime;
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                if (health <= 0)
                {
                    damageable.TakeDamage(spiritData.ReturnDamage, collision.GetContact(0).point, GetHitDirection(collision), null);
                    PlayHit();
                    return;
                }

                Damage damage = new(data.Damage);
                DamageFeedback feedback = damageable.TakeDamage(damage, collision.GetContact(0).point, GetHitDirection(collision), owner);

                if (feedback.IsHit == false) return;
                if (feedback.Amount <= 0) return;

                OnProjectileHit(feedback, collision.collider.ClosestPoint(transform.position));
                PlayHit();
                return;
            }
        }

        protected override float GetCurrentSpeed()
        {
            if (health <= 0) return data.Speed + currentLifeTime * spiritData.RaturnAccelerate;
            return data.SpeedModifier.Evaluate(currentLifeTime / data.Lifetime) * data.Speed;
        }

        protected override bool CanHoming(Quaternion targetRotation)
        {
            if (health <= 0) return true;
            return base.CanHoming(targetRotation);
        }

        protected override float GetRotationSpeed()
        {
            if (health <= 0) return spiritData.ReturnAngularSpeed;
            return base.GetRotationSpeed();
        }

        public DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (health <= 0) return new(false);

            health -= damage.Amount;
            UpdateMaterial();

            if (health <= 0)
            {
                ReturnToEnemy();
            }

            return new(damage, 1);
        }

        private void UpdateMaterial()
        {
            var clip = health > 0 ? (float)health / spiritData.Health * 0.4f + 0.6f : 0;
            var isReturn = health > 0 ? 0 : 1;

            _renderer.material.SetFloat(CLIP_KEY, clip);
            _renderer.material.SetFloat(RETURN_KEY, isReturn);
        }
    }
}
