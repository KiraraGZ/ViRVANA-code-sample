using System;

namespace Magia.GameLogic
{
    [Serializable]
    public class ElementalWeakness
    {
        public float Lightning = 1f;
        public float Magia = 1f;
        public float Void = 1f;
        public float Fire = 1f;
        public float Physical = 1f;

        public ElementalWeakness()
        {
            Physical = 1f;
            Lightning = 1f;
            Magia = 1f;
            Void = 1f;
        }

        public int CalculateDamage(Damage damage)
        {
            return (int)(damage.Amount * GetWeakness(damage.Element));
        }

        public float GetWeakness(ElementType element)
        {
            switch (element)
            {
                case ElementType.Physical:
                    {
                        return Physical;
                    }
                case ElementType.Lightning:
                    {
                        return Lightning;
                    }
                case ElementType.Magia:
                    {
                        return Magia;
                    }
                case ElementType.Void:
                    {
                        return Void;
                    }
                case ElementType.Fire:
                    {
                        return Fire;
                    }
                default:
                    {
                        return 1;
                    }
            }
        }
    }
}
