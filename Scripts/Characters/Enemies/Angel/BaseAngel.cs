using System;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Angel
{
    public class BaseAngel : BaseEnemy
    {
        [SerializeField] private AngelData data;

        [Header("Mechanics")]
        [SerializeField] private AngelMovement movement;
        [SerializeField] private AngelGateSkill gateSkill;
        [SerializeField] private AngelPillarSkill pillarSkill;

        private float readyTime;
        private const float READY_DELAY = 2f;

        public override void Initialize(PlayerController player)
        {
            movement.Initialize(this);
            gateSkill.Initialize(this);
            pillarSkill.Initialize(this);

            health = data.MaxHealth;
            maxHealth = data.MaxHealth;

            base.Initialize(player);

            Rigidbody.drag = 1f;

            readyTime = Time.time + READY_DELAY;
        }

        public override void Dispose()
        {
            movement.Dispose();
            gateSkill.Dispose();
            pillarSkill.Dispose();

            base.Dispose();
        }

        private void FixedUpdate()
        {
            if (Player == null) return;

            if (Time.time < readyTime) return;

            movement.PhysicsUpdate();
            gateSkill.PhysicsUpdate();
            pillarSkill.PhysicsUpdate();

            if (Time.time >= readyTime && !isReady)
            {
                OnReady();
            }
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (health <= 0)
            {
                Dispose();

                return feedback;
            }

            return feedback;
        }
    }

    [Serializable]
    public class AngelData
    {
        public int MaxHealth = 50;
        public EnemyHitboxData[] HitboxDatas;
    }
}
