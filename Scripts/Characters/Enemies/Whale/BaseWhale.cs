using System;
using System.Collections.Generic;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Whale
{
    public class BaseWhale : BaseEnemy, IPhaseChangeable
    {
        public event Action<int> EventChangePhase;

        [Space(20)]
        [SerializeField] protected WhaleState currentState;
        [SerializeField] protected WhaleSO data;

        [Header("Mechanics")]
        [SerializeField] protected WhaleMovement movement;
        [SerializeField] protected WhaleCombat combat;

        [Header("SFX")]
        [SerializeField] protected AudioSource audioSource;
        public AudioSource AudioSource => audioSource;
        [SerializeField] private AudioClip howlSound;

        protected int currentPhase;

        //TODO: Check if this is neccessary or not.
        public List<float> GetPhaseChangeRatios() => data.Stats.PhaseChangeRatios;

        protected float stateEndTime;
        private float readyTime;
        private const float READY_DELAY = 3f;

        public override void Initialize(PlayerController _player)
        {
            movement.Initialize(data, this);
            combat.Initialize(this);
            combat.EventSkillPerformed += OnSkillPerformed;
            combat.EventSkillEnd += OnSkillEnd;

            health = data.Stats.MaxHealth;
            maxHealth = data.Stats.MaxHealth;

            currentPhase = 0;

            base.Initialize(_player);

            movement.EnterApproachState();
            combat.EnterPhase(currentPhase);

            readyTime = Time.time + READY_DELAY;
        }

        public override void Dispose()
        {
            movement.Dispose();
            combat.Dispose();
            combat.EventSkillPerformed -= OnSkillPerformed;
            combat.EventSkillEnd -= OnSkillEnd;

            base.Dispose();
        }

        protected virtual void FixedUpdate()
        {
            if (Player == null) return;

            switch (currentState)
            {
                case WhaleState.Approach:
                    {
                        movement.ApproachPlayer();
                        combat.PhysicsUpdate(currentState);

                        if (movement.GetAngleToPlayer() >= 0f) break;

                        ChangeState(WhaleState.Decelerate);
                        break;
                    }
                case WhaleState.Decelerate:
                    {
                        movement.DecelerateAfterPassing();
                        combat.PhysicsUpdate(currentState);

                        if (movement.GetCurrentSpeed() > data.MovementData.MinSpeed) break;

                        //TODO: create new virtual and move this to override method, if another variation of whale do not need to change the state.
                        // if (currentPhase > 0 && combat.CheckSpecialSkill())
                        // {
                        //     ChangeState(WhaleState.RevolveTakeoff);
                        //     break;
                        // }

                        ChangeState(WhaleState.Reposition);
                        break;
                    }
                case WhaleState.Reposition:
                    {
                        movement.RepositionForNextFling();
                        combat.PhysicsUpdate(currentState);

                        if (movement.GetAngleToPlayer() < 0.95f) break;

                        ChangeState(WhaleState.Approach);
                        movement.EnterApproachState();
                        break;
                    }
                case WhaleState.RevolveTakeoff:
                    {
                        movement.TakeoffRevolve();

                        if (movement.GetDistanceFromPlayer() < data.RevolveData.RevolveDistance) break;

                        ChangeState(WhaleState.RevolveAttack);

                        stateEndTime = Time.time + data.RevolveData.AttackDuration;
                        break;
                    }
                case WhaleState.RevolveAttack:
                    {
                        movement.MaintainRevolve();
                        combat.PhysicsUpdate(currentState);

                        if (Time.time < stateEndTime) break;

                        ChangeState(WhaleState.Reposition);
                        break;
                    }
                case WhaleState.Retreat:
                    {
                        movement.Retreat();

                        if (Time.time < stateEndTime) break;

                        Dispose();
                        break;
                    }
            }

            if (Time.time >= readyTime && !isReady)
            {
                OnReady();
                PlaySound();
            }
        }

        protected virtual void ChangeState(WhaleState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (feedback.IsHit == false) return feedback;

            if (health <= 0)
            {
                ChangeState(WhaleState.Retreat);
                movement.EnterRetreatState();
                stateEndTime = Time.time + data.RetreatData.Duration;
                return feedback;
            }

            //TODO: move this condition to hyper whale if another variation of whale do not need to change the state.
            if (currentPhase < data.Stats.PhaseChangeRatios.Count && health <= data.Stats.PhaseChangeRatios[currentPhase] * maxHealth)
            {
                ChangePhase();
                ChangeState(WhaleState.RevolveTakeoff);
                combat.EnterPhase(currentPhase);

                EventChangePhase?.Invoke(currentPhase);
            }

            return feedback;
        }

        public virtual void ChangePhase()
        {
            currentPhase += 1;
        }

        public float GetAngleToPlayer()
        {
            return movement.GetAngleToPlayer();
        }

        private void PlaySound()
        {
            audioSource.PlayOneShot(howlSound);
        }

        #region subscribe events
        protected virtual void OnSkillPerformed()
        {
            PlaySound();
        }

        protected virtual void OnSkillEnd()
        {

        }
        #endregion
    }
}