using System;
using Magia.Utilities.Pooling;
using UnityEngine;

namespace Magia.Vfx
{
    public class OrbVfx : PoolObject<OrbVfx>
    {
        [SerializeField] private float initSpeed = 20;
        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float turnSpeed = 5f;

        private Transform target;
        private float speed;
        private Vector3 velocity;
        private float followTime;

        public void Initialize(Transform _target, Vector3 initDirection)
        {
            transform.rotation = Quaternion.Euler(initDirection);
            target = _target;
            speed = initSpeed;
            followTime = Time.time + 1f;

            if (target == null)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            target = null;
            Return();
        }

        private void Update()
        {
            if (Time.time < followTime)
            {
                UpdatePop();
            }
            else
            {
                UpdateFollow();
            }
        }

        private void UpdatePop()
        {
            speed = Math.Max(speed - initSpeed * Time.deltaTime, 0);
            transform.position += speed * Time.deltaTime * transform.forward;
        }

        private void UpdateFollow()
        {
            Vector3 delta = target.position - transform.position;
            Vector3 direction = delta.normalized;

            speed = Math.Min(speed + maxSpeed * Time.deltaTime, maxSpeed);
            velocity = Vector3.Lerp(velocity, direction * speed, turnSpeed * Time.deltaTime);

            transform.position += velocity * Time.deltaTime;

            if (delta.magnitude > 1f) return;

            Dispose();
        }
    }
}
