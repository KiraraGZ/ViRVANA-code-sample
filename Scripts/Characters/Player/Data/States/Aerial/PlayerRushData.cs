using System;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerRushData
    {
        public float SpeedModifier = 2f;
        public float EnterHoverSpeedRatio = 0.1f;
        public float DashToRushTime = 1f;
    }
}
