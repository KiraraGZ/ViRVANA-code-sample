using System;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerUncontrollableData
    {
        public float GroundDetectRange = 2f;
        public PlayerJumpData JumpData;

        public float KnockbackDuration = 0.8f;
        public float KnockbackForce = 40f;
        public float StaggeredDuration = 0.3f;

        public int PullupDamage = 30;
        public float PullupForce = 12f;
        public float PullupDuration = 1f;
    }
}
