using Magia.GameLogic;
using System;
using System.Collections;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class ApparitionProjectileData
    {
        public ProjectileData Projectile;
        public float ApparitionDuration = 1f;

        public ApparitionProjectileData(ApparitionProjectileData data)
        {
            Projectile = data.Projectile;
            ApparitionDuration = data.ApparitionDuration;
        }
    }
    public class ApparitionProjectile : Projectile
    {
        private ApparitionProjectileData apparitionData;
        protected bool isApparition;
        protected Vector3 direction;

        public void Initialize(ApparitionProjectileData _data, Vector3 _direction, IDamageable _owner)
        {
            apparitionData = _data;
            owner = _owner;
            isApparition = true;
            direction = _direction;

            data = apparitionData.Projectile;
            expireTime = Time.time + apparitionData.Projectile.Lifetime + apparitionData.ApparitionDuration;
            transform.localScale = Vector3.one * apparitionData.Projectile.Size;

            StartCoroutine(ApparitionState());
        }

        public virtual IEnumerator ApparitionState()
        {
            yield return new WaitForSeconds(apparitionData.ApparitionDuration);
            rb.AddForce(direction.normalized * data.Speed, ForceMode.VelocityChange);
            isApparition = false;
        }
    }
}