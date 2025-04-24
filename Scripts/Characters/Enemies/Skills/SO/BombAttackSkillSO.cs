using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "BombAttack", menuName = "ScriptableObject/Skills/BombAttack")]
    public class BombAttackSkillSO : ScriptableObject
    {
        public ExplodeProjectile BombProjectilePrefab;
        public ExplodeProjectileData ExplodeProjectileData;
        public int MaxProjectileAmount = 15;
        public float ReachPlayerDuration = 3f;
        public float DecisionDistance = 120f;
        public float Interval = 1.25f;
        public float Downtime = 30f;
    }
}