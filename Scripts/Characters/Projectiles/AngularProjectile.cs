using UnityEngine;
using Magia.GameLogic;
using System;

namespace Magia.Projectiles
{
    [Serializable]
    public class AngularProjectileData
    {
        public ProjectileData ProjectileData;
        public float AngularSpeed = 30f;
        public float MaxDegreeRotate = 330f;
    }

    public class AngularProjectile : Projectile
    {
        private AngularProjectileData angularData;

        protected Vector3 angularDirection;
        protected float totalDegreeRotate;

        public void Initialize(AngularProjectileData _data, Vector3 _direction, IDamageable _owner, Vector3 _angularDirection)
        {
            angularData = _data;
            angularDirection = _angularDirection.normalized * angularData.AngularSpeed;

            totalDegreeRotate = 0;

            base.Initialize(_data.ProjectileData, _direction, _owner);
        }

        protected override void UpdateSpeed()
        {
            rb.velocity = rb.velocity.magnitude * transform.forward;
            UpdateRotation();
        }

        protected void UpdateRotation()
        {
            if (totalDegreeRotate < angularData.MaxDegreeRotate)
            {
                Quaternion targetRotation = transform.rotation * Quaternion.Euler(angularDirection * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularData.AngularSpeed * Time.deltaTime);
                totalDegreeRotate += angularData.AngularSpeed * Time.deltaTime;
            }
        }
    }
}