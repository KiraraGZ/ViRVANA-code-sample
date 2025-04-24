using System;
using Magia.Enemy.Angel;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Seraph
{
    public class BaseSeraph : BaseEnemy
    {
        [SerializeField] private AngelData data;

        [Header("Mechanics")]
        [SerializeField] private SeraphMovement movement;
        [SerializeField] private SeraphCombat combat;
        [SerializeField] private AngelAnimatorController animator;
        [HideInInspector] public bool IsOutOfHealth;

        public override void Initialize(PlayerController player)
        {
            movement.Initialize();
            combat.Initialize(this, player);
            combat.EventChantingStarted += OnChantingStarted;
            combat.EventChantingStopped += OnChantingStopped;

            base.Initialize(player);

            health = data.MaxHealth;
            maxHealth = data.MaxHealth;

            IsOutOfHealth = false;
            Rigidbody.drag = 1f;
        }

        public override void Dispose()
        {
            movement.Dispose();
            combat.Dispose();
            combat.EventChantingStarted -= OnChantingStarted;
            combat.EventChantingStopped -= OnChantingStopped;

            base.Dispose();
        }

        private void FixedUpdate()
        {
            if (Player == null) return;

            movement.PhysicsUpdate();

            if (!movement.EnableCombat) return;

            if (!isReady)
            {
                OnReady();
            }

            combat.PhysicsUpdate();
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (IsOutOfHealth) return new(false);

            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (health <= 0)
            {
                IsOutOfHealth = true;
                combat.EnterDeadState();
                OnDefeated();
                return feedback;
            }

            return feedback;
        }

        #region subscribe events
        private void OnChantingStarted()
        {
            animator.StartChant();
        }

        private void OnChantingStopped()
        {
            if (health <= 0) return;

            animator.StopChant();
        }
        #endregion

        [Serializable]
        public class AngelData
        {
            public int MaxHealth = 50;
            public EnemyHitboxData[] HitboxDatas;
        }
    }
}