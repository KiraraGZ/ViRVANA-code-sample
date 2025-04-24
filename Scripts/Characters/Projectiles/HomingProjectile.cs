using Magia.GameLogic;
using System;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class HomingProjectileData
    {
        public ProjectileData ProjectileData;
        public float MaxAngularSpeed = 30f;
        public float AngularAcceleration = 6f;
        public float MaxDegreeRotate = 330f;
    }

    public class HomingProjectile : Projectile
    {
        private HomingProjectileData homingData;

        protected float currentRotationSpeed;
        protected float totalDegreeRotate;
        protected GameObject target;

        public void Initialize(HomingProjectileData _data, Vector3 _direction, IDamageable _owner, GameObject _target)
        {
            homingData = _data;
            target = _target;

            currentRotationSpeed = 0;
            totalDegreeRotate = 0;
            expireTime = Time.time + homingData.ProjectileData.Lifetime;

            base.Initialize(_data.ProjectileData, _direction, _owner);
        }

        protected override void UpdateSpeed()
        {
            if (target == null)
            {
                base.UpdateSpeed();
                return;
            }

            rb.velocity = GetCurrentSpeed() * transform.forward;
            UpdateRotation();
        }

        protected void UpdateRotation()
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            if (CanHoming(targetRotation))
            {
                currentRotationSpeed = GetRotationSpeed();
                rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
                totalDegreeRotate += Time.deltaTime * currentRotationSpeed;
            }
            else
            {
                currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, Time.deltaTime * homingData.AngularAcceleration);
            }
        }

        protected virtual float GetCurrentSpeed()
        {
            return data.SpeedModifier.Evaluate(currentLifeTime / data.Lifetime) * data.Speed;
        }

        protected virtual bool CanHoming(Quaternion targetRotation)
        {
            return Quaternion.Angle(transform.rotation, targetRotation) > 0.1f && totalDegreeRotate < homingData.MaxDegreeRotate;
        }

        protected virtual float GetRotationSpeed()
        {
            return Mathf.Clamp(currentRotationSpeed + homingData.AngularAcceleration * Time.deltaTime, 0, homingData.MaxAngularSpeed);
        }
    }
}