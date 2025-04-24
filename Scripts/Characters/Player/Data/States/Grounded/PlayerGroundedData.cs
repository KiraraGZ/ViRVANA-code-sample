using System;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerGroundedData
    {
        public PlayerWalkData WalkData;
        public PlayerRunData RunData;
        public PlayerSprintData SprintData;
    }
}
