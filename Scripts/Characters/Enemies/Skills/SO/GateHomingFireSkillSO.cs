using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "GateHomingFire", menuName = "ScriptableObject/Skills/GateHomingFire")]
    public class GateHomingFireSkillSO : ScriptableObject
    {
        public GateHomingProjectile Prefab;
        public GateHomingProjectileData GateHomingData;

        public int Wave = 8;
        public float Interval = 1.5f;
        public float Downtime = 30f;
        public float SpreadAngle = 60f;
    }
}
