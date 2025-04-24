using System;
using System.Collections.Generic;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class StalkerVoidPuffer : VoidPuffer, IPhaseChangeable
    {
        private const string CHARGE_WARN_KEY = "Stalker_Charge";

        public event Action<int> EventChangePhase;

        [Header("Stalker Mechanics")]
        [SerializeField] private List<float> phaseChangeRatios;
        [SerializeField] private StalkerChargeData[] chargeDatas;
        [SerializeField] private ParticleSystem chargeEffect;
        [SerializeField] private AudioClip chargeSound;

        private StalkerChargeData Charge => phase > 0 ? chargeDatas[phase - 1] : chargeDatas[0];
        private ExplodePufferCombat VoidCombat => combat as ExplodePufferCombat;
        private int phase;
        private float nextChargeTime;
        private float stopChargeTime;
        private bool isCharge;

        public List<float> GetPhaseChangeRatios() => phaseChangeRatios;

        protected override void UpdateFollowState()
        {
            if (phase == 0 || isCharge == false)
            {
                if (phase >= 2 && Time.time > nextChargeTime)
                {
                    EnterStrafeState();
                }

                base.UpdateFollowState();
                return;
            }

            if (isCharge && Time.time > stopChargeTime)
            {
                isCharge = false;
                nextChargeTime = Time.time + Charge.Downtime;
                animator.CloseMouth();
            }

            movement.PhysicsUpdate(Charge.SpeedMultiplier, isDodge: false);
        }

        protected override bool ChangeState(PufferState state)
        {
            if (currentState == PufferState.STRAFE && phase > 0)
            {
                if (Time.time > nextChargeTime)
                {
                    isCharge = true;
                    stopChargeTime = Time.time + Charge.Duration;
                    chargeEffect.Play();
                    animator.PlayAudioOneShot(chargeSound);
                    animator.OpenMouth();
                    dialogueVisualizer.PlayWarnDialogue(CHARGE_WARN_KEY, 1);
                }
                else
                {
                    VoidCombat.FireAtOnce();
                }
            }

            return base.ChangeState(state);
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (phase < phaseChangeRatios.Count && health <= phaseChangeRatios[phase] * maxHealth)
            {
                ChangePhase();
                EventChangePhase?.Invoke(phase);
            }

            return feedback;
        }

        public void ChangePhase()
        {
            phase += 1;
            VoidCombat.EnterPhase(phase);
        }

        [Serializable]
        public class StalkerChargeData
        {
            public float Downtime = 30f;
            public float Duration = 1.5f;
            public float SpeedMultiplier = 5f;
        }
    }
}
