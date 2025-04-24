using System;
using UnityEngine;

namespace Magia.Player.Data
{
    [Serializable]
    public class DefaultColliderData
    {
        [Tooltip("Height of player collider")]
        public float Height = 1.3f;
        public float CenterY = -0.12f;
        public float Radius = 0.15f;
    }
}
