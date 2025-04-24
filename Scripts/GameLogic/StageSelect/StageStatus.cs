using System;

namespace Magia.GameLogic
{
    [Serializable]
    public class StageStatus
    {
        public bool IsUnlocked;
        public int Rating;

        public StageStatus(bool isUnlocked, int rating)
        {
            IsUnlocked = isUnlocked;
            Rating = rating;
        }

        public StageStatus(int rating)
        {
            IsUnlocked = true;
            Rating = rating;
        }


        public StageStatus()
        {
            IsUnlocked = true;
            Rating = 0;
        }
    }
}
