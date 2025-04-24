using System;
using System.Collections.Generic;

public interface IPhaseChangeable
{
    public event Action<int> EventChangePhase;
    public void ChangePhase();
    public List<float> GetPhaseChangeRatios();
}