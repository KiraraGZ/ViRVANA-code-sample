using System;

namespace Magia.GameLogic
{
    [Serializable]
    public class ScalableVariable<T>
    {
        public T Initial;
        public T Max;
    }
}