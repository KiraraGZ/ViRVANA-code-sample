using System;
using UnityEngine;

namespace Magia.Player.Data
{
    [Serializable]
    public class PlayerJumpData
    {
        public Vector3 JumpForce;
        public float JumpTime = 0.2f;
    }
}
