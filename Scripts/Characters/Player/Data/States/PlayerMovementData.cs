using System;
using UnityEngine;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerMovementData
    {
        public float BaseSpeed = 20f;
        public float GroundDetectRange = 2f;
        public AnimationCurve SlopeSpeedAngles;

        [Space(10)]

        public Vector3 TargetRotationReachTime;
        public float TargetSpeedReachTime;

        [Space(10)]

        public PlayerAerialData AerialData;

        [Space(10)]

        public PlayerGroundedData GroundedData;

        [Space(10)]

        public PlayerWallData WallData;
    }
}
