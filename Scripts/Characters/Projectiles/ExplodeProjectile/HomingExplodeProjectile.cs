using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class HomingExplodeProjectileData
    {
        public ExplodeProjectileData ProjectileData;
        public float AngularVelocity;

        public float MaxAngularSpeed;
        public float AngularAcceleration;

        public float MaxDegreeRotate;

        public HomingExplodeProjectileData(HomingExplodeProjectileData data)
        {
            ProjectileData = new(data.ProjectileData);
            AngularVelocity = data.AngularVelocity;
            MaxDegreeRotate = data.MaxDegreeRotate;
        }
    }

    public class HomingExplodeProjectile : ExplodeProjectile
    {
        protected HomingExplodeProjectileData homingExplodeData;
        protected float currentDegreeRotate;
        protected float currentRotationSpeed;

        protected GameObject target;

        public void Initialize(HomingExplodeProjectileData _data, Vector3 _direction, IDamageable _owner, GameObject _target)
        {
            homingExplodeData = _data;
            target = _target;

            Initialize(_data.ProjectileData, _direction, _owner);
        }

        public override void Dispose()
        {
            currentDegreeRotate = 0;
            base.Dispose();
        }

        protected override void FixedUpdate()
        {
            if (!isActive) return;
            base.FixedUpdate();

            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f && currentDegreeRotate < homingExplodeData.MaxDegreeRotate)
            {
                currentRotationSpeed = Mathf.Min(currentRotationSpeed + homingExplodeData.AngularAcceleration * Time.fixedDeltaTime, homingExplodeData.MaxAngularSpeed);
                rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, currentRotationSpeed * Time.fixedDeltaTime);
                currentDegreeRotate += Time.fixedDeltaTime * currentRotationSpeed;
            }
            else
            {
                currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, Time.fixedDeltaTime * homingExplodeData.AngularAcceleration);
            }
            rb.velocity = rb.transform.forward * homingExplodeData.ProjectileData.ProjectileData.Speed;
        }
    }
}