using System;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerAerialData
    {
        public PlayerFlyData FlyData;
        public PlayerRushData RushData;

        public float VelocitySpeed = 15f;
    }
}
