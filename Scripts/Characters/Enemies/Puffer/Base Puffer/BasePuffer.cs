using System;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Puffer
{
    public enum PufferState
    {
        ENTER,
        FOLLOW,
        STAGGERED,
        HOLD,
        FIRE,
        STRAFE,
        DEAD,
    }

    public class BasePuffer : BaseEnemy
    {
        [Space(20)]
        [SerializeField] protected PufferState currentState;
        [SerializeField] protected PufferData data;

        [Header("Mechanics")]
        [SerializeField] protected PufferMovement movement;
        [SerializeField] protected PufferCombat combat;
        [SerializeField] protected PufferAnimatorController animator;

        //Random variable
        protected float revolveRange;
        private float enterHeight;

        private int damageAmountToStrafe;
        protected float stateEndTime;
        private float disposeTime;
        private Vector3 deadTorqueAxis;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);

            currentState = PufferState.ENTER;
            stateEndTime = Time.time + 10f;

            movement.Initialize(this, data.MovementData, animator.GetMeshWidth());
            combat.Initialize(this);
            combat.EventFiringEnd += OnFiringEnd;
            animator.Initialize();

            health = data.MaxHealth;
            maxHealth = data.MaxHealth;
            revolveRange = Random.Range(data.RevolveMinRange, data.RevolveMaxRange);
            enterHeight = _player.transform.position.y + Random.Range(-20f, 0f);
            damageAmountToStrafe = data.DamageAmountToStrafe;

            Rigidbody.drag = 0.5f;
        }

        public override void Dispose()
        {
            currentState = PufferState.FOLLOW;
            Rigidbody.velocity = Vector3.zero;

            movement.Dispose();
            combat.Dispose();
            combat.EventFiringEnd -= OnFiringEnd;
            animator.Dispose();

            base.Dispose();
        }

        protected virtual void FixedUpdate()
        {
            if (Player == null) return;

            switch (currentState)
            {
                case PufferState.ENTER:
                    {
                        movement.PhysicsUpdate(Vector3.up, 1.5f);

                        if (transform.position.y < enterHeight & Time.time > stateEndTime) break;

                        ChangeState(PufferState.FOLLOW);
                        break;
                    }
                case PufferState.FOLLOW:
                    {
                        UpdateFollowState();
                        break;
                    }
                case PufferState.STAGGERED:
                    {
                        if (Time.time < stateEndTime) break;

                        ChangeState(PufferState.FOLLOW);
                        break;
                    }
                case PufferState.HOLD:
                    {
                        movement.PhysicsUpdate(isMove: false);

                        if (Time.time < stateEndTime) break;

                        ChangeState(PufferState.FIRE);
                        combat.StartFiring();
                        break;
                    }
                case PufferState.FIRE:
                    {
                        movement.PhysicsUpdate(isMove: false);
                        combat.PhysicsUpdate();
                        break;
                    }
                case PufferState.STRAFE:
                    {
                        movement.PhysicsUpdate(isRotate: true, isClose: true);

                        if (Time.time < stateEndTime) break;

                        ChangeState(PufferState.FOLLOW);
                        break;
                    }
                case PufferState.DEAD:
                    {
                        Rigidbody.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);
                        Rigidbody.AddRelativeTorque(deadTorqueAxis, ForceMode.Acceleration);

                        if (Time.time >= disposeTime) Dispose();

                        break;
                    }
            }
        }

        protected virtual void UpdateFollowState()
        {
            var distance = Vector3.Distance(Player.transform.position, transform.position);
            movement.PhysicsUpdate(isClose: distance < revolveRange);

            if (combat.CheckSkillToCast())
            {
                EnterHoldState();
            }
        }

        protected virtual void EnterHoldState()
        {
            ChangeState(PufferState.HOLD);
            animator.OpenMouth();
            stateEndTime = Time.time + data.HoldDuration;
        }

        protected void EnterStrafeState()
        {
            ChangeState(PufferState.STRAFE);
            stateEndTime = Time.time + data.MovementData.StrafeDuration;
            movement.EnterStrafingState();
            damageAmountToStrafe = data.DamageAmountToStrafe;

            animator.PlayStrafeSound();
        }

        protected virtual bool ChangeState(PufferState newState)
        {
            if (currentState == newState) return false;
            if (currentState == PufferState.DEAD) return false;

            if (currentState == PufferState.ENTER)
            {
                OnReady();
            }

            if (currentState == PufferState.STRAFE)
            {
                movement.ExitStrafingState();
            }

            currentState = newState;

            return true;
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (feedback.IsHit == false) return feedback;

            if (health <= 0)
            {
                ChangeState(PufferState.DEAD);
                animator.Dead();
                deadTorqueAxis = new(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
                disposeTime = Time.time + data.DisposeDelay;
                return feedback;
            }

            animator.TakeDamage(0.5f);
            TakeDamageStrafeCheck(feedback.Amount);
            TakeDamageAnimation(hitPoint);

            return feedback;
        }

        private void TakeDamageStrafeCheck(int amount)
        {
            if (currentState == PufferState.STRAFE) return;

            damageAmountToStrafe -= amount;

            if (damageAmountToStrafe <= 0)
            {
                EnterStrafeState();
            }
        }

        private void TakeDamageAnimation(Vector3 direction)
        {
            if (currentState == PufferState.HOLD || currentState == PufferState.FIRE || currentState == PufferState.STRAFE) return;

            if (data.StaggeredDuration > 0 && !ChangeState(PufferState.STAGGERED)) return;

            Quaternion targetRotation = Quaternion.RotateTowards(Rigidbody.rotation, Quaternion.LookRotation(direction), data.StaggeredAngle);
            Rigidbody.MoveRotation(targetRotation);

            stateEndTime = Time.time + data.StaggeredDuration;
        }

        #region subscribe events
        protected virtual void OnFiringEnd()
        {
            ChangeState(PufferState.FOLLOW);
            animator.CloseMouth();
        }
        #endregion
    }

    [Serializable]
    public class PufferData
    {
        public int MaxHealth = 50;
        public float HoldDuration = 1f;
        public float StaggeredAngle = 15f;
        public float StaggeredDuration = 0.5f;
        public int DamageAmountToStrafe = 40;
        public PufferMovementData MovementData;

        public float RevolveMinRange = 30f;
        public float RevolveMaxRange = 60f;

        public float DisposeDelay = 6f;
    }
}