using System;
using UnityEngine;

namespace Magia.Utilities
{
    [Serializable]
    public struct ModifiableProperty<T> where T : struct
    {
        public T baseValue;
        public bool isOverLifetime;
        public AnimationCurve multiplier;

        public ModifiableProperty(T baseValue, bool isOverLifetime, AnimationCurve multiplier)
        {
            this.baseValue = baseValue;
            this.isOverLifetime = isOverLifetime;
            this.multiplier = multiplier ?? new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        }

        public T GetValue(float time)
        {
            if (isOverLifetime)
            {
                float evaluatedValue = multiplier.Evaluate(time);
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)Mathf.RoundToInt(Convert.ToSingle(baseValue) * evaluatedValue);
                }
                else if (typeof(T) == typeof(float))
                {
                    return (T)(object)(Convert.ToSingle(baseValue) * evaluatedValue);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported type");
                }
            }
            else
            {
                return baseValue;
            }
        }
    }
}