using Magia.GameLogic;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Enemy.Skills
{
    [CreateAssetMenu(fileName = "Shockwave", menuName = "ScriptableObject/Skills/Shockwave")]
    public class ShockwaveSkillSO : ScriptableObject
    {
        public Damage Damage;
        public float Delay = 7f;
        public float MaxRange = 500f;
        public float Downtime = 10f;

        [Header("Audioclip")]
        public HitEffect ImpactBuildingPrefab;
        public HitEffect ImpactPlayerPrefab;
        public AudioClip EarthShakeSound;
    }
}
