using System;
using Magia.GameLogic;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Angel
{
    public class AngelGateProjectile : Projectile
    {
        [Header("Gate Projectile")]
        [SerializeField] private Transform gateVfx;
        [SerializeField] private GameObject projectileObject;

        private float launchTime;
        private float gateClosedTime;
        private Vector3 direction;
        private bool isLaunched;
        private bool isGateClosed;

        private PlayerController player;

        public void Initialize(AngelGateProjectileData _data, Vector3 _direction, IDamageable _owner, PlayerController _player)
        {
            launchTime = Time.time + _data.LaunchDelay;
            gateClosedTime = Time.time + _data.LaunchDelay + _data.GateCloseDelay;
            direction = _direction;
            isLaunched = false;

            base.Initialize(_data.ProjectileData, Vector3.zero, _owner);

            gateVfx.gameObject.SetActive(true);
            projectileObject.SetActive(false);

            player = _player;
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
                Vector3 direction = PredictPlayerPositionRandomly();
                rb.AddForce(direction.normalized * data.Speed, ForceMode.VelocityChange);

                projectileObject.SetActive(true);

                isLaunched = true;
            }
            if (!isGateClosed && Time.time < gateClosedTime)
            {
                gateVfx.gameObject.SetActive(false);
                isGateClosed = false;
            }

            base.FixedUpdate();
        }

        Vector3 PredictPlayerPositionRandomly()
        {
            float timeReachPlayer = (player.transform.position - transform.position).magnitude / data.Speed;
            Vector3 playerUnitVectorPredict = player.Rigidbody.velocity * timeReachPlayer;
            Vector3 target = player.transform.position + (Random.Range(0, 2) * playerUnitVectorPredict);
            Vector3 direction = target - transform.position;

            return direction;
        }
    }

    [Serializable]
    public class AngelGateProjectileData
    {
        public ProjectileData ProjectileData;
        public float LaunchDelay;
        public float GateCloseDelay;
    }
}