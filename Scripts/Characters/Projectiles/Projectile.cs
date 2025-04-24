using System;
using Magia.Audio;
using Magia.Enemy;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class ProjectileData
    {
        public Damage Damage;
        public float Speed = 10f;
        public float Gravity = 0f;
        public AnimationCurve SpeedModifier = new();
        public float Lifetime = 10f;
        public float Size = 1f;

        public ProjectileData(ProjectileData data)
        {
            Damage = new(data.Damage);
            Speed = data.Speed;
            SpeedModifier = data.SpeedModifier;
            Lifetime = data.Lifetime;
            Size = data.Size;
        }
    }

    public class Projectile : MonoBehaviour
    {
        //TODO: Implement method to subscribe these Action.
        public event Action<DamageFeedback, Vector3> EventProjectileHit;
        public event Action<BaseEnemy> EventProjectileHitEnemy;
        public event Action EventProjectileExpire;

        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected GameObject vfx;
        [SerializeField] private GameObject hitVfx;

        [Header("Sound")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] protected AudioClip initSound;

        protected ProjectileData data;
        protected IDamageable owner;
        protected float currentLifeTime;
        protected float expireTime;
        protected Vector3 currentVelocity;

        public ProjectilePoolManager PoolManager;

        public virtual void Initialize(ProjectileData _data, Vector3 _direction, IDamageable _owner)
        {
            Initialize(_data, _direction, _data.Speed, _owner);
        }

        public virtual void Initialize(ProjectileData _data, Vector3 _direction, float speed, IDamageable _owner)
        {
            data = _data;
            owner = _owner;

            transform.localScale = Vector3.one * data.Size;
            transform.rotation = _direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(_direction);

            currentLifeTime = 0;
            expireTime = Time.time + data.Lifetime;

            currentVelocity = _direction.normalized * speed;
            rb.AddForce(currentVelocity, ForceMode.VelocityChange);

            SetActiveVfx(true);
            PlaySound(initSound);
        }

        public virtual void Dispose()
        {
            data = null;
            owner = null;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            SetActiveVfx(false);

            if (PoolManager != null)
            {
                PoolManager.Return(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (data == null) return;

            currentLifeTime += Time.deltaTime;
            UpdateSpeed();

            if (Time.time < expireTime) return;

            OnProjectileExpire();
            Dispose();
            return;
        }

        protected virtual void UpdateSpeed()
        {
            // var newVelocity = data.SpeedModifier.Evaluate(Mathf.Clamp(currentLifeTime / data.Lifetime, 0, 1)) * data.Speed * rb.velocity.normalized;
            // rb.AddForce(newVelocity - currentVelocity, ForceMode.VelocityChange);
            rb.AddForce(Vector3.down * data.Gravity, ForceMode.Acceleration);
            // currentVelocity = newVelocity;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (owner == null) return;

            if (collision.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                Hit(collision, damageable);
                return;
            }

            PlayHit();
        }

        protected virtual void Hit(Collision collision, IDamageable damageable)
        {
            Damage damage = new(data.Damage);
            DamageFeedback feedback = damageable.TakeDamage(damage, collision.GetContact(0).point, GetHitDirection(collision), owner);

            if (feedback.IsHit == false) return;

            PlayHit();

            if (feedback.Weakness <= 0) return;

            OnProjectileHit(feedback, collision.collider.ClosestPoint(transform.position));

            var enemy = collision.collider.GetComponentInParent<BaseEnemy>();

            if (enemy == null) return;

            EventProjectileHitEnemy?.Invoke(enemy);
        }

        protected Vector3 GetHitDirection(Collision collision)
        {
            Vector3 direction = (collision.GetContact(0).point - transform.position).normalized;

            if (direction == Vector3.zero) return rb.velocity;

            return direction;
        }

        protected void PlayHit()
        {
            SpawnHitVfx();
            OnProjectileExpire();
            Dispose();
        }

        protected void SpawnHitVfx()
        {
            if (hitVfx == null) return;

            Instantiate(hitVfx, transform.position, Quaternion.identity);
        }

        protected void PlaySound(AudioClip sound)
        {
            if (audioSource == null || sound == null) return;

            audioSource.pitch = SoundHelper.GetRandomPitch();
            audioSource.PlayOneShot(sound);
        }

        #region public methods
        public void SetActiveVfx(bool isActive)
        {
            if (vfx == null) return;

            vfx.SetActive(isActive);
        }
        #endregion

        #region subscribe events
        protected void OnProjectileHit(DamageFeedback feedback, Vector3 hitPos)
        {
            EventProjectileHit?.Invoke(feedback, hitPos);
        }

        protected void OnProjectileExpire()
        {
            EventProjectileExpire?.Invoke();
        }
        #endregion
    }
}