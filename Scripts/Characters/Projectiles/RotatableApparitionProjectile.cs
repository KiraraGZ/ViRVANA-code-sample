using Magia.GameLogic;
using System;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class RotatableApparitionProjectileData
    {
        public ApparitionProjectileData ApparitionProjectile;

        public float DegreesPerSecond = 120f;

        public RotatableApparitionProjectileData(RotatableApparitionProjectileData data)
        {
            ApparitionProjectile = data.ApparitionProjectile;
            DegreesPerSecond = data.DegreesPerSecond;
        }
    }

    public class RotatableApparitionProjectile : ApparitionProjectile
    {
        protected RotatableApparitionProjectileData rotatableApparitionProjectileData;
        protected GameObject target;

        public void Initialize(RotatableApparitionProjectileData _data, Vector3 _direction, IDamageable _owner, GameObject _target)
        {
            rotatableApparitionProjectileData = _data;
            target = _target;
            base.Initialize(_data.ApparitionProjectile, _direction, _owner);
        }

        protected override void FixedUpdate()
        {
            if (isApparition)
            {
                Vector3 directionToTarget = target.transform.position - transform.position;

                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                rb.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotatableApparitionProjectileData.DegreesPerSecond * Time.deltaTime
                );
                direction = directionToTarget;
            }
            else
            {
                base.FixedUpdate();
            }
        }
    }
}