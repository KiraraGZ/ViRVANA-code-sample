using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "HomingFire", menuName = "ScriptableObject/Skills/HomingFire")]
    public class HomingFireSkillSO : ScriptableObject
    {
        public HomingProjectile ProjectilePrefab;
        public HomingProjectileData ProjectileData;
        public HomingFireSkillDirection FireDirection;

        public int Amount = 20;
        public float Interval = 0.2f;
        public float Downtime = 10f;
        public float SpreadAngle = 60f;
    }

    public enum HomingFireSkillDirection
    {
        FORWARD_DIRECTION,
        BACKWARD_DIRECTION,
        TARGET_DIRECTION,
    }
}
