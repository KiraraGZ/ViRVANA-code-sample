using Magia.GameLogic;
using Magia.Projectiles;
using System;

namespace Magia.Buddy.Data
{
    [Serializable]
    public class BuddyCombatData
    {
        public Projectile ProjectilePrefab;
        public ProjectileData ProjectileData;

        public float AttackRange = 75f;
        public float AttackInterval = 2f;
        public float BlockedDuration = 2.5f;

        public float FindNewTargetInterval = 1f;
    }
}