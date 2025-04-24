using Magia.GameLogic;
using System;
using UnityEngine;

namespace Magia.Projectiles
{
    [Serializable]
    public class OrbitProjectileData
    {
        public HomingProjectileData HomingProjectileData;
        public float RadialAcceleration = 10f;
        public float OrbitDegreePerSeconds = 60f;
        public float DroppedRadius = 0f;
    }

    [Serializable]
    public class OrbitProjectileDataInput
    {
        public Projectile CenterProjectile;
        public float Speed;
        public float Radius;
        public float Degree;

        public OrbitProjectileDataInput(Projectile _projectile, float _speed, float _radius, float _degree)
        {
            CenterProjectile = _projectile;
            Speed = _speed;
            Radius = _radius;
            Degree = _degree;
        }
    }

    public class OrbitProjectile : HomingProjectile
    {
        private OrbitProjectileData orbitData;

        protected bool isOrbit;
        protected Projectile centerProjectile;
        protected float currentDegree;
        protected float currentOrbitSpeed;
        protected float currentRadius;

        protected Vector3 previousPosition;

        public void Initialize(OrbitProjectileData _data, OrbitProjectileDataInput _input, Vector3 direction, IDamageable owner, GameObject target)
        {
            orbitData = _data;
            base.Initialize(_data.HomingProjectileData, direction, owner, target);
            centerProjectile = _input.CenterProjectile;
            currentOrbitSpeed = _input.Speed;
            currentDegree = _input.Degree;
            currentRadius = _input.Radius;

            transform.SetParent(centerProjectile.transform);
            isOrbit = true;

            float radians = currentDegree * Mathf.Deg2Rad;
            Vector3 initialOffset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * currentRadius;
            transform.localPosition = initialOffset;

            centerProjectile.EventProjectileExpire += DropFromOrbit;
        }

        public override void Dispose()
        {
            centerProjectile.EventProjectileExpire -= DropFromOrbit;

            base.Dispose();
        }

        protected override void FixedUpdate()
        {
            if (isOrbit)
            {
                currentOrbitSpeed -= orbitData.RadialAcceleration * Time.deltaTime;
                currentRadius += currentOrbitSpeed * Time.deltaTime;
                currentDegree += orbitData.OrbitDegreePerSeconds * Time.deltaTime;
                if (currentRadius < orbitData.DroppedRadius)
                {
                    DropFromOrbit();
                    return;
                }
                float radians = currentDegree * Mathf.Deg2Rad;
                Vector3 initialOffset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
                previousPosition = transform.position;
                transform.localPosition = initialOffset * currentRadius;
            }

            base.FixedUpdate();
        }

        public void DropFromOrbit()
        {
            isOrbit = false;
            transform.SetParent(centerProjectile.transform.parent);

            Vector3 forward = (transform.position - previousPosition).normalized;
            rb.velocity = forward * orbitData.HomingProjectileData.ProjectileData.Speed;
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}