using System;
using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Projectiles
{
    public class AngelPillar : Projectile
    {
        [Header("Gate Projectile")]
        [SerializeField] private LineRenderer laserLine;
        [SerializeField] private Transform beamVFX;

        private AngelPillarData pillarData;
        private Transform target;
        private float launchTime;
        private float nextDamageTime;
        private bool isLaunched;

        private Vector3 specificDirection;

        public void Initialize(AngelPillarData _data, Transform _target, IDamageable _owner)
        {
            pillarData = _data;
            target = _target;
            launchTime = Time.time + _data.LaunchDelay;
            nextDamageTime = Time.time + _data.LaunchDelay;
            isLaunched = false;

            base.Initialize(_data.ProjectileData, Vector3.zero, _owner);

            WarnLine();
        }

        public void Initialize(AngelPillarData _data, Vector3 destination, IDamageable _owner)
        {
            pillarData = _data;
            launchTime = Time.time + _data.LaunchDelay;
            nextDamageTime = Time.time + _data.LaunchDelay;
            isLaunched = false;
            specificDirection = (destination - transform.position).normalized;
            specificDirection.y = 0;
            transform.position = new Vector3(transform.position.x, 80f, transform.position.z);

            base.Initialize(_data.ProjectileData, Vector3.zero, _owner);

            WarnLine();
        }

        public void Initialize(AngelPillarData _data, Transform _target, IDamageable _owner, Vector3 center)
        {
            Initialize(_data, _target, _owner);
            specificDirection = (transform.position - center).normalized;
            specificDirection.y = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void FixedUpdate()
        {
            if (Time.time < launchTime) return;

            if (!isLaunched)
            {
                laserLine.enabled = false;
                LaserBeam();

                isLaunched = true;
            }

            if (Time.time >= nextDamageTime)
            {
                CheckDamage();
                nextDamageTime = Time.time + pillarData.DamageInterval;
            }

            if (pillarData.MovementMode == PillerMovementMode.FOLLOW_PLAYER)
            {
                FollowPlayer();
            }
            else if (pillarData.MovementMode == PillerMovementMode.TOWARD_CENTER)
            {
                FollowSpecificDirection();
            }

            base.FixedUpdate();
        }

        private void WarnLine()
        {
            beamVFX.gameObject.SetActive(false);
            var bottomPosition = transform.position;
            bottomPosition.y = 0;
            laserLine.SetPosition(0, transform.position);
            laserLine.SetPosition(1, bottomPosition);
            laserLine.widthMultiplier = 2f;
        }

        private void LaserBeam()
        {
            beamVFX.gameObject.SetActive(true);
            beamVFX.localScale = new Vector3(5, transform.position.y, 5);
        }

        private void CheckDamage()
        {
            if (Physics.SphereCast(transform.position, 5, Vector3.down, out RaycastHit hit, transform.position.y))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    Damage damage = new(data.Damage);
                    damageable.TakeDamage(damage, hit.point, transform.forward, owner);
                    if (pillarData.MovementMode == PillerMovementMode.STILL) Dispose();
                }
            }
        }

        private void FollowPlayer()
        {
            var direction = target.position - transform.position;
            direction.y = 0;
            rb.velocity = direction.normalized * data.Speed;
            LaserBeam();
        }

        private void FollowSpecificDirection()
        {
            rb.velocity = specificDirection * data.Speed;
            LaserBeam();
        }

        protected override void OnCollisionEnter(Collision collision)
        {

        }
    }

    [Serializable]
    public class AngelPillarData
    {
        public ProjectileData ProjectileData;
        public float LaunchDelay = 3f;
        public float DamageInterval = 0.5f;
        public PillerMovementMode MovementMode = PillerMovementMode.STILL;
    }

    public enum PillerMovementMode
    {
        STILL,
        FOLLOW_PLAYER,
        SPIRAL,
        TOWARD_CENTER,
    }
}
