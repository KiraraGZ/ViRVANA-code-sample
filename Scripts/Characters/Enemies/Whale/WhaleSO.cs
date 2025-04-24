using System;
using System.Collections.Generic;
using UnityEngine;

namespace Magia.Enemy.Whale
{
    [CreateAssetMenu(fileName = "Whale", menuName = "ScriptableObject/Enemy/Whale")]
    public class WhaleSO : ScriptableObject
    {
        public WhaleStats Stats;
        public WhaleMovementData MovementData;
        public WhaleApproachPlayerData ApproachPlayerData;
        public WhaleDecelerationData DecelerationData;
        public WhaleRevolveData RevolveData;
        public WhaleRetreatData RetreatData;
    }

    [Serializable]
    public class WhaleStats
    {
        public int MaxHealth = 2000;
        public List<float> PhaseChangeRatios;
    }

    [Serializable]
    public class WhaleMovementData
    {
        public float MinSpeed = 20f;
        public float AngularSpeed = 10f;
        public float MaxTiltAngle = 15f;
    }

    [Serializable]
    public class WhaleApproachPlayerData
    {
        public float ApproachSpeed = 50f;
        public float MaxApproachSpeed = 80f;
        public float Acceleration = 7f;
        public float AngularSpeed = 3f;
    }

    [Serializable]
    public class WhaleDecelerationData
    {
        public float Deceleration = 8f;
    }

    [Serializable]
    public class WhaleRevolveData
    {
        public float Acceleration = 15f;
        public float MaxSpeed = 150f;
        public float RevolveDistance = 500f;
        public float AttackDuration = 8f;
    }

    [Serializable]
    public class WhaleRetreatData
    {
        public float MaxTilt = 30f;
        public float TileAcceleration = 5f;
        public float Speed = 20f;
        public float Duration = 15f;
    }
}
