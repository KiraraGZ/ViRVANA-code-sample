using System;
using Magia.GameLogic;
using Magia.Skills;
using Magia.Player.Data;
using Magia.Player.StateMachines;
using Magia.Player.Utilities;
using UnityEngine;
using Magia.Enemy;

namespace Magia.Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public event Action EventEnterGroundState;
        public event Action EventEnterAerialState;
        public event Action<DamageFeedback, Vector3> EventDamageDealt;
        public event Action<PlayerStateStatusData> EventStatusChanged;
        public event Action EventPlayerLifeReachedZero;

        public PlayerSO Data;
        public PlayerStateReusableData ReusableData;

        public Rigidbody Rigidbody { get; private set; }
        public Collider Collider { get; private set; }
        public PlayerInput Input { get; private set; }
        public PlayerAnimatorController AnimatorController { get; private set; }
        public SkillHandler SkillHandler { get; private set; }
        public CameraHandler CameraHandler { get; private set; }
        public Transform CameraTransform { get; private set; }

        private PlayerMovementStateMachine movementStateMachine;
        private PlayerCombatStateMachine combatStateMachine;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            AnimatorController = GetComponent<PlayerAnimatorController>();
            SkillHandler = GetComponentInChildren<SkillHandler>();

            //TODO: Floating Collider not working, adjust its parameter to make it working again.
            Data.ColliderUtility.Init(Collider);

            CameraTransform = Camera.main.transform;
        }

        public void Initialize(PlayerInput input, SkillHandlerData skillHandlerData, CameraHandler cameraHandler)
        {
            ReusableData = new PlayerStateReusableData(Data.StatsData);

            movementStateMachine = new PlayerMovementStateMachine(this);
            combatStateMachine = new PlayerCombatStateMachine(this);

            Input = input;
            Input.SwitchToPlayerInput();
            AnimatorController.Initialize();
            SkillHandler.Initialize(skillHandlerData);
            SkillHandler.EventDamageDealt += OnDamageDealt;
            CameraHandler = cameraHandler;

            movementStateMachine.ChangeState(movementStateMachine.HoverState);
            combatStateMachine.ChangeState(combatStateMachine.IdleState);
        }

        public void Dispose()
        {
            movementStateMachine.Dispose();
            combatStateMachine.Dispose();

            ReusableData = null;
            movementStateMachine = null;
            combatStateMachine = null;

            Input.SwitchToMenuInput();
            Input = null;
            AnimatorController.Dispose();
            SkillHandler.Dispose();
            SkillHandler.EventDamageDealt -= OnDamageDealt;
            CameraHandler = null;
        }

        private void Update()
        {
            movementStateMachine.HandleInput();
            combatStateMachine.HandleInput();

            movementStateMachine.Update();
            combatStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
            SkillHandler.PhysicsUpdate();

            CheckRecoverHealth();
        }

        #region movement
        public void EnterGroundState()
        {
            ReusableData.IsGrounded = true;
            AnimatorController.EnterGroundedState();

            EventEnterGroundState?.Invoke();
        }

        public void EnterAerialState()
        {
            ReusableData.IsGrounded = false;
            AnimatorController.EnterAerialState();

            EventEnterAerialState?.Invoke();
        }

        public void StartMoveAlongCamera()
        {
            ReusableData.MoveAlongCamera = true;
        }

        public void StopMoveAlongCamera()
        {
            ReusableData.MoveAlongCamera = false;
        }
        #endregion

        #region attack & skills
        public void UpdateVelocity(Vector3 velocity)
        {
            AnimatorController.UpdateVelocity(velocity, Data.MovementData.BaseSpeed);
            CameraHandler.UpdateVelocity(velocity.magnitude / Data.MovementData.BaseSpeed);
        }

        public void SetEnableAttack(bool isEnable)
        {
            ReusableData.EnableAttack = isEnable;
        }

        public void RecoverSkills()
        {
            SkillHandler.RecoverSkills();
        }

        public void PerformSkill(SkillPerformedData data)
        {
            if (!ReusableData.IsDash)
            {
                AnimatorController.StartPerformSkill((int)data.Type);
            }

            if (data.CameraZoomOut)
            {
                CameraHandler.PerformSkill();
            }
            // movementStateMachine.StopRush();
        }

        public void ReleaseSkill()
        {
            CameraHandler.ReleaseSkill();
        }
        #endregion

        #region health system
        public DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (owner == (IDamageable)this) return new DamageFeedback(false);
            if (SkillHandler.CheckBarrier(damage, hitDirection.normalized)) return new DamageFeedback(damage, 0);
            if (ReusableData.IsInvulnerable) return new DamageFeedback(damage, -1);
            if (Time.time - ReusableData.StartIFrameTime < Data.StatsData.IFrameDuration) return new DamageFeedback(damage, -1);

            var isKnockback = CheckDamageBuildUp(damage.Amount);

            if (ReusableData.StatusData.Life == 0 && ReusableData.IsDefeated == false)
            {
                ReusableData.IsDefeated = true;
                EventPlayerLifeReachedZero?.Invoke();
                return new DamageFeedback(false);
            }

            if (isKnockback)
            {
                Rigidbody.MoveRotation(Quaternion.LookRotation(-hitDirection));
                ReusableData.KnockbackDirection = hitDirection.normalized;
                movementStateMachine.ChangeState(movementStateMachine.KnockbackState);
                CameraHandler.StartCameraShake(5, 1f);
            }
            else
            {
                movementStateMachine.ChangeState(movementStateMachine.StaggeredState);
                CameraHandler.StartCameraShake(3, 0.2f);
            }

            ReusableData.LastTakeDamageTime = Time.time;

            return new DamageFeedback(damage, 1);
        }

        public void SelfTakeDamage(int amount)
        {
            var statusData = ReusableData.StatusData;

            if (statusData.Health <= 0)
            {
                LoseLife();
                return;
            }

            statusData.Health -= amount;
            OnStatusChanged(statusData);

            if (statusData.Health <= 0)
            {
                statusData.Health = 0;
            }
        }

        private bool CheckDamageBuildUp(int amount)
        {
            var statusData = ReusableData.StatusData;

            if (statusData.Health <= 0)
            {
                LoseLife();
                return false;
            }

            statusData.Health -= amount;
            statusData.DamageBuildAmount += amount;
            OnStatusChanged(statusData);

            if (statusData.DamageBuildAmount >= Data.StatsData.DamageBuildAmountToKnockback)
            {
                statusData.DamageBuildAmount = 0;
                return true;
            }

            if (statusData.Health <= 0)
            {
                statusData.Health = 0;
            }

            return false;
        }

        private void LoseLife()
        {
            var statusData = ReusableData.StatusData;

            statusData.Health = Data.StatsData.MaxHealth;
            statusData.Life = Math.Max(statusData.Life - 1, 0);

            ReusableData.StartIFrameTime = Time.time;
            statusData.DamageBuildAmount = 0;
            AnimatorController.TriggerBarrier(Data.StatsData.IFrameDuration);
            OnStatusChanged(statusData);
        }

        private void CheckRecoverHealth()
        {
            if (Time.time - ReusableData.LastTakeDamageTime < Data.StatsData.TimeToRecover && ReusableData.IsGrounded == false) return;
            if (Time.time - ReusableData.LastRecoverTime < 0) return;

            var statusData = ReusableData.StatusData;

            statusData.Health += Data.StatsData.HealthRecoverPerSec;
            statusData.DamageBuildAmount -= Data.StatsData.HealthRecoverPerSec;

            if (statusData.Health > Data.StatsData.MaxHealth)
            {
                statusData.Health = Data.StatsData.MaxHealth;
            }

            if (statusData.DamageBuildAmount < 0)
            {
                statusData.DamageBuildAmount = 0;
            }

            OnStatusChanged(statusData);

            ReusableData.LastRecoverTime = Time.time + 1f;
        }
        #endregion

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent<EnemyHitbox>(out var enemyHitbox))
            {
                TakeDamage(new(50, ElementType.None, DamageType.Contact), transform.position, (transform.position - collision.transform.position).normalized, enemyHitbox);
            }
        }

        #region subscribe events
        private void OnDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            if (feedback.Weakness < 0) return;

            EventDamageDealt?.Invoke(feedback, hitPos);
        }

        private void OnStatusChanged(PlayerStateStatusData statusData)
        {
            EventStatusChanged?.Invoke(statusData);
        }
        #endregion
    }
}
