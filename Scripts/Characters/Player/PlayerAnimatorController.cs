using System;
using System.Collections;
using Magia.GameLogic;
using MagicaCloth2;
using UnityEngine;

namespace Magia.Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private const string VELOCITY_FORWARD_KEY = "velocityForward";
        private const string VELOCITY_RIGHT_KEY = "velocityRight";
        private const string AERIAL_KEY = "IsAerial";
        private const string DASH_KEY = "Dash";
        private const string DASH_INDEX_KEY = "DashIndex";
        private const string DASH_RIGHT_KEY = "IsDashRight";
        private const string BRAKE_KEY = "Brake";
        private const string JUMP_KEY = "IsJumping";
        private const string WALL_KEY = "IsWall";
        private const string WALL_RIGHT_KEY = "IsWallRight";

        private const string FALLING_KEY = "IsFalling";
        private const string STAGGERED_KEY = "IsStaggered";
        private const string KNOCKBACK_KEY = "IsKnockback";

        private const string PERFORM_SKILL_KEY = "PerformSkill";
        private const string SKILL_NUMBER_KEY = "SkillNumber";
        private const string BASIC_ATTACK_KEY = "BasicAttack";
        private const string BASIC_ATTACK_NUMBER = "BasicAttackNumber";
        private const string STOP_ATTACK_KEY = "StopAttack";

        private const float IDLE_WIND_STRENGTH = 10f;
        private const float CHARGE_WIND_STRENGTH = 30f;

        [SerializeField] private Animator animator;
        [SerializeField] private MagicaWindZone windZone;

        [Header("Manager")]
        [SerializeField] private PlayerVfxManager vfxManager;
        [SerializeField] private PlayerSoundManager soundManager;

        public void Initialize()
        {
            vfxManager.Initialize();
        }

        public void Dispose()
        {
            vfxManager.Dispose();
        }

        public void UpdateVelocity(Vector3 velocity, float baseSpeed)
        {
            animator.SetFloat(VELOCITY_FORWARD_KEY, velocity.z);
            animator.SetFloat(VELOCITY_RIGHT_KEY, velocity.x);
            vfxManager.UpdatePlayerSpeed(velocity.magnitude / baseSpeed);
            soundManager.UpdateVelocity(velocity);
        }

        public void UpdateEnergy(float lerp)
        {
            vfxManager.UpdateEnergy(lerp);
        }

        #region aerial animation
        public void EnterAerialState()
        {
            animator.SetBool(AERIAL_KEY, true);
        }

        public void StartForwardDash()
        {
            animator.SetBool(BRAKE_KEY, false);
            animator.SetInteger(DASH_INDEX_KEY, 0);
            animator.SetBool(DASH_KEY, true);
        }

        public void StartBackwardDash()
        {
            animator.SetBool(BRAKE_KEY, false);
            animator.SetInteger(DASH_INDEX_KEY, 1);
            animator.SetBool(DASH_KEY, true);
        }

        public void StartSideDash(bool isRight)
        {
            animator.SetBool(BRAKE_KEY, false);
            animator.SetInteger(DASH_INDEX_KEY, 2);
            animator.SetBool(DASH_KEY, true);
            animator.SetBool(DASH_RIGHT_KEY, isRight);
        }

        public void StopDash()
        {
            animator.SetBool(DASH_KEY, false);
        }

        public void StartBrake()
        {
            animator.SetBool(BRAKE_KEY, true);

            StartCoroutine(Wait(0.5f, () => animator.SetBool(BRAKE_KEY, false)));
        }
        #endregion

        #region ground animation
        public void EnterGroundedState()
        {
            animator.SetBool(AERIAL_KEY, false);
        }

        public void EnterJumpState()
        {
            animator.SetBool(JUMP_KEY, true);
        }

        public void ExitJumpState()
        {
            animator.SetBool(JUMP_KEY, false);
        }
        #endregion

        #region wall animation
        public void EnterWallState(bool isRight)
        {
            animator.SetBool(WALL_KEY, true);
            animator.SetBool(WALL_RIGHT_KEY, isRight);
        }

        public void ExitWallState()
        {
            animator.SetBool(WALL_KEY, false);
        }
        #endregion

        #region uncontrollable animation
        public void EnterFallingState()
        {
            animator.SetBool(FALLING_KEY, true);
        }

        public void ExitFallingState()
        {
            animator.SetBool(FALLING_KEY, false);
        }

        public void EnterStaggeredState()
        {
            animator.SetBool(STAGGERED_KEY, true);
        }

        public void ExitStaggeredState()
        {
            animator.SetBool(STAGGERED_KEY, false);
        }

        public void EnterKnockbackState()
        {
            animator.SetBool(KNOCKBACK_KEY, true);
        }

        public void ExitKnockbackState()
        {
            animator.SetBool(KNOCKBACK_KEY, false);
        }
        #endregion

        #region combat
        public void StartPerformSkill(int number)
        {
            animator.SetBool(PERFORM_SKILL_KEY, true);
            animator.SetInteger(SKILL_NUMBER_KEY, number);
            windZone.main = CHARGE_WIND_STRENGTH;

            soundManager.PerformSkillSound(number);
            vfxManager.StartPerformSkill();
        }

        public void ReleaseSkill()
        {
            animator.SetBool(PERFORM_SKILL_KEY, false);
            windZone.main = IDLE_WIND_STRENGTH;

            soundManager.ReleaseSkillSound();
            vfxManager.ReleaseSkill();
        }

        public void PlayOneShotSound(AudioClip clip)
        {
            soundManager.PlayOneShotSound(clip);
        }

        public void StartBasicAttack(int number)
        {
            animator.SetInteger(BASIC_ATTACK_NUMBER, number);
            animator.SetTrigger(BASIC_ATTACK_KEY);

            if (number != 2)
            {
                soundManager.PlayAttackSound();
            }
            else
            {
                soundManager.PlaySpreadAttackSound();
            }
        }

        public void StopBasicAttack()
        {
            animator.SetTrigger(STOP_ATTACK_KEY);
            // animator.SetBool(BASIC_ATTACK_KEY, false);
        }
        #endregion

        #region vfx
        public void TriggerBarrier(float duration)
        {
            vfxManager.TriggerBarrier(duration);
        }

        public void ToggleBarrier(bool isOn)
        {
            vfxManager.ToggleBarrier(isOn);
        }

        public void ToggleWing(bool isOn)
        {
            vfxManager.ToggleWing(isOn);
        }

        public void SetElement(ElementType element)
        {
            vfxManager.SetElement(element);
        }

        public void DisableElementTrail()
        {
            vfxManager.DisableElement();
        }
        #endregion

        private IEnumerator Wait(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);

            callback?.Invoke();
        }
    }
}
