using UnityEngine;

namespace Magia.Player.Data
{
    public class PlayerStateReusableData
    {
        public PlayerStateStatusData StatusData;
        public float LastTakeDamageTime;
        public float LastRecoverTime;
        public float StartIFrameTime;

        public Vector2 MovementInput;
        public float MovementSpeedModifier = 1f;
        public float MovementSpeedOnSlopeModifier = 1f;
        public bool MoveAlongCamera;
        public bool IsGrounded;
        public bool IsRush;
        public Vector2 DashDirection;
        public bool IsDash => DashDirection != Vector2.zero;
        public bool IsInvulnerable;
        public bool IsDefeated;

        public Vector3 KnockbackDirection;
        public Vector3 DetectedWallNormal;

        public bool EnableAttack;
        public bool IsPerformingSkill;
        public int CurrentSkillIndex;

        private Vector3 currentTargetRotation;
        private Vector3 timeToReachTargetRotation;
        private Vector3 dampedTargetRotationCurrentVelocity;
        private Vector3 dampedTargetRotationPassedTime;

        public ref Vector3 CurrentTargetRotation
        {
            get { return ref currentTargetRotation; }
        }

        public ref Vector3 TimeToReachTargetRotation
        {
            get { return ref timeToReachTargetRotation; }
        }

        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get { return ref dampedTargetRotationCurrentVelocity; }
        }

        public ref Vector3 DampedTargetRotationPassedTime
        {
            get { return ref dampedTargetRotationPassedTime; }
        }

        public PlayerStateReusableData(PlayerStatsData stats)
        {
            StatusData = new(stats);
        }
    }

    public class PlayerStateStatusData
    {
        public int Health;
        public int Life;
        public int DamageBuildAmount;

        public PlayerStateStatusData(PlayerStatsData stats)
        {
            Health = stats.MaxHealth;
            Life = stats.MaxLife;
        }
    }
}
