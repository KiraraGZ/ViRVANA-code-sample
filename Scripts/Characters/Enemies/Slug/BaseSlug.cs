using System;
using Magia.GameLogic;
using Magia.Player;
using Magia.Vfx;
using UnityEngine;

namespace Magia.Enemy.Slug
{
    public enum SlugState
    {
        ENTER,
        IDLE,
        ALERT,
        DEAD,
    }

    public class BaseSlug : BaseEnemy
    {
        private const string BARRIER_WARN_KEY = "Slug_Barrier";

        [Space(20)]
        [SerializeField] private SlugState currentState;
        [SerializeField] private SlugData data;

        [Header("Mechanics")]
        [SerializeField] private SlugMovement movement;
        [SerializeField] private SlugCombat combat;
        [SerializeField] private Barrier barrier;
        [SerializeField] private Collider barrierCollider;
        [SerializeField] private Collider[] bodyColliders;
        [SerializeField] private ParticleSystem particle;

        private int barrierHealth;
        private int damageAmountToBarrier;
        private float enterHeight;
        private float barrierStartTime;
        private float disposeTime;

        protected DialogueVisualizer dialogueVisualizer;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);

            movement.Initialize(this, data.MovementData);
            combat.Initialize(this);

            health = data.MaxHealth;
            maxHealth = data.MaxHealth;
            barrierHealth = 0;
            damageAmountToBarrier = data.DamageAmountToBarrier;
            enterHeight = data.MovementData.GetRandomEnterHeight();

            ToggleBarrierCollider(false);
        }

        public override void Dispose()
        {
            currentState = SlugState.ENTER;
            Rigidbody.velocity = Vector3.zero;

            movement.Dispose();
            combat.Dispose();

            base.Dispose();
        }

        private void FixedUpdate()
        {
            if (Player == null) return;

            switch (currentState)
            {
                case SlugState.ENTER:
                    {
                        movement.Move(Vector3.up);
                        movement.LerpLookRotation();

                        if (transform.position.y < enterHeight) break;

                        ChangeState(SlugState.IDLE);
                        OnReady();
                        break;
                    }
                case SlugState.IDLE:
                case SlugState.ALERT:
                    {
                        Vector3 playerDirection = Player.transform.position - transform.position;
                        playerDirection.y = 0;
                        float distanceToPlayer = playerDirection.magnitude;
                        bool isAlert = distanceToPlayer <= data.AlertRange;

                        if (currentState == SlugState.IDLE && isAlert)
                        {
                            ChangeState(SlugState.ALERT);
                            combat.EnterAlertState();
                            particle.Play();
                        }
                        else if (currentState == SlugState.ALERT && !isAlert)
                        {
                            ChangeState(SlugState.IDLE);
                            combat.EnterIdleState();
                            particle.Stop();
                        }

                        movement.LerpLookRotation();
                        combat.PhysicsUpdate();
                        break;
                    }
                case SlugState.DEAD:
                    {
                        Rigidbody.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);

                        if (Time.time < disposeTime) break;

                        Dispose();
                        break;
                    }
            }

            if (barrierHealth <= 0) return;
            if (Time.time < barrierStartTime + data.BarrierDuration) return;

            DissolveBarrier();
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (barrierHealth > 0)
            {
                BarrierTakeDamage(damage.Amount);
                return new DamageFeedback(damage, 1);
            }

            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (feedback.IsHit == false) return feedback;

            movement.Move(hitDirection);
            CheckBarrierBuildup(damage.Amount);

            if (health <= 0)
            {
                ChangeState(SlugState.DEAD);
                combat.EnterDeadState();
                disposeTime = Time.time + 6f;
                return feedback;
            }

            return feedback;
        }

        private void BarrierTakeDamage(int amount)
        {
            barrierHealth -= amount;

            if (barrierHealth > 0)
            {
                OnHealthUpdate(barrierHealth, data.BarrierHealth);
                return;
            }

            DissolveBarrier();
        }

        private void CheckBarrierBuildup(int amount)
        {
            damageAmountToBarrier -= amount;

            if (damageAmountToBarrier > 0) return;

            BuildBarrier();
        }

        private void BuildBarrier()
        {
            barrierHealth = data.BarrierHealth;
            barrierStartTime = Time.time;

            barrier.BuildUp();
            ToggleBarrierCollider(true);

            base.OnHealthUpdate(barrierHealth, data.BarrierHealth, true);

            dialogueVisualizer.PlayWarnDialogue(BARRIER_WARN_KEY, 1);
        }

        private void DissolveBarrier()
        {
            barrierHealth = 0;
            damageAmountToBarrier = data.DamageAmountToBarrier;

            barrier.Dissolve();
            ToggleBarrierCollider(false);

            base.OnHealthUpdate(health, maxHealth, false);
        }

        private void ToggleBarrierCollider(bool active)
        {
            barrierCollider.enabled = active;

            foreach (var bodyCollider in bodyColliders)
            {
                bodyCollider.enabled = !active;
            }
        }

        protected virtual bool ChangeState(SlugState state)
        {
            if (currentState == state) return false;
            if (currentState == SlugState.DEAD) return false;

            currentState = state;

            return true;
        }

        protected override void OnReady()
        {
            base.OnReady();
            dialogueVisualizer ??= GameplayController.Instance.DialogueVisualizer;
        }

        #region invoke methods
        protected override void OnHealthUpdate(int health, int maxHealth, bool barrier = false)
        {
            if (barrierHealth > 0)
            {
                base.OnHealthUpdate(barrierHealth, data.BarrierHealth, true);
                return;
            }

            base.OnHealthUpdate(health, maxHealth, false);
        }
        #endregion
    }

    [Serializable]
    public class SlugData
    {
        public int MaxHealth = 100;
        public int BarrierHealth = 100;
        public int DamageAmountToBarrier = 60;
        public float BarrierDuration = 10f;

        public SlugMovementData MovementData;
        public float AlertRange = 80f;
    }
}