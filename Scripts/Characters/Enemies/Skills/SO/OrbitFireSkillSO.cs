using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "OrbitFire", menuName = "ScriptableObject/Skills/OrbitFire")]
    public class OrbitFireSkillSO : ScriptableObject
    {
        public float Duration = 1f;
        public float Downtime = 10f;

        public HomingProjectile CenterProjectilePrefab;
        public HomingProjectileData HomingData;
        public OrbitProjectile SurroundProjectilePrefab;
        public OrbitProjectileData OrbitData;
        public int SurroundProjectileAmount = 3;
        public float InitialSurroundProjectileRadius = 1f;
        public float InitialSurroundProjectileSpeed = 30f;
    }
}
