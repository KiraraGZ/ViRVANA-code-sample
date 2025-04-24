using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Enemy
{
    [RequireComponent(typeof(Collider))]
    public class EnemyHitbox : MonoBehaviour, IDamageable
    {
        private EnemyHitboxData data;

        [Header("Read Only")]
        [SerializeField] private int health;

        private BaseEnemy baseEnemy;

        public void Initialize(EnemyHitboxData hitboxData, BaseEnemy _baseEnemy)
        {
            data = hitboxData;
            baseEnemy = _baseEnemy;

            health = data.MaxHealth;
        }

        public DamageFeedback TakeDamage(Damage rawDamage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (data == null) return new DamageFeedback(false);
            if (health <= 0 && data.MaxHealth > 0) return new DamageFeedback(false);

            var damage = new Damage(rawDamage);
            var amount = data.Weakness.CalculateDamage(rawDamage);
            damage.Amount = amount;
            var feedback = baseEnemy.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (feedback.IsHit == false) return new DamageFeedback(false);

            health -= amount;

            return new DamageFeedback(rawDamage, data.Weakness.GetWeakness(rawDamage.Element));
        }

        public BaseEnemy GetOwner()
        {
            return baseEnemy;
        }
    }

    [Serializable]
    public class EnemyHitboxData
    {
        public ElementalWeakness Weakness;
        public int MaxHealth = 100;

        public EnemyHitboxData()
        {
            Weakness = new();
            MaxHealth = 0;
        }
    }
}
