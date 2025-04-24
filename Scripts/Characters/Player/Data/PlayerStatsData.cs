using System;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerStatsData
    {
        public int MaxHealth = 50;
        public int MaxLife = 3;

        public float TimeToRecover = 5f;
        public int HealthRecoverPerSec = 10;
        public int DamageBuildAmountToKnockback = 30;

        public float IFrameDuration = 0.75f;
    }
}
