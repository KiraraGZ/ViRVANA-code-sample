using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "SpiritFire", menuName = "ScriptableObject/Skills/SpiritFire")]
    public class SpiritFireSkillSO : ScriptableObject
    {
        public SpiritProjectile ProjectilePrefab;
        public SpiritProjectileData SpiritData;
        public HomingProjectileData HomingData;
        public HomingFireSkillDirection FireDirection;

        public int Amount = 20;
        public float Interval = 0.2f;
        public float Downtime = 10f;
        public float SpreadAngle = 60f;
    }
}
