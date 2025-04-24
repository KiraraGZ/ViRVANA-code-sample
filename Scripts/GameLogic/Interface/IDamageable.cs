using System;
using UnityEngine;

namespace Magia.GameLogic
{
    public interface IDamageable
    {
        public DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner);
    }

    [Serializable]
    public class Damage
    {
        public int Amount = 10;
        public ElementType Element;
        public DamageType Type;

        public Damage(int amount, ElementType element, DamageType type)
        {
            Amount = amount;
            Element = element;
            Type = type;
        }

        public Damage(Damage damage)
        {
            Amount = damage.Amount;
            Element = damage.Element;
            Type = damage.Type;
        }
    }

    public class DamageFeedback
    {
        public bool IsHit;
        public Damage Damage;
        public int Amount => Damage.Amount;
        public ElementType Element => Damage.Element;
        public float Weakness;
        //-1 : blocked
        //0 : uneffect
        //0.5 : weak
        //1 : normal
        //1+ : effective
        //2+ : very effective

        public DamageFeedback(bool isHit)
        {
            IsHit = isHit;
        }

        public DamageFeedback(Damage damage, float weakness)
        {
            IsHit = true;
            Damage = damage;
            Weakness = weakness;
        }
    }
}