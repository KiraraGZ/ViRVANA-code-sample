using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "HoverAttack", menuName = "ScriptableObject/Skills/HoverAttack")]
    public class ApparitionFireSkillSO : ScriptableObject
    {
        public RotatableApparitionProjectile ProjectilePrefab;
        public RotatableApparitionProjectileData ProjectileData;
        public float MinRotateDegreesThreshold = 40f;
        public float ApparitionDistance = 20f;
        public float Interval = 0.3f;
        public float Downtime = 60f;
    }
}
