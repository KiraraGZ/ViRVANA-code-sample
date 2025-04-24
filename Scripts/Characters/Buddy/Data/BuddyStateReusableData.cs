using Magia.Enemy;
using UnityEngine;

namespace Magia.Buddy.Data
{
    public class BuddyStateReusableData
    {
        public bool IsFollowingPlayer;
        public bool IsRepositioning;

        public Vector3 Destination;
        public Vector3 LastDirection;

        public BaseEnemy Target;
        public float NextFindNewTarget;
        public float NextShoot;
        public float BlockedStateTimeOut;
    }
}