using System;
using UnityEngine;

namespace Magia.Buddy.Data
{
    [Serializable]
    public class BuddyMovementData
    {
        public float BaseSpeed = 10f;
        public float AngularRotationSpeed = 270f;

        public float StopDistance = 3.5f;
        public int RaycastSampleCount = 18;
        public float SlowDownRadius = 10f;
        public float ObstacleAvoidanceRange = 3f;

        [Header("Reposition State")]
        public float RepositionSpeedModifier = 2.5f;

        [Header("Idle State")]
        public float HoverHeight = 1f;
        public float HoverHeightOffset = 0.1f;

        [Header("Follow State")]
        public float FollowPlayerLerp = 0.75f;
        public float VerticalLerp = 0.5f;
    }
}