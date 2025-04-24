using System;
using UnityEngine;

namespace Magia.Skills
{
    public class EnergySkill : ActiveSkill
    {
        public event Action<float> EventEnergyChanged;

        [SerializeField] protected EnergyData energyData;
        [SerializeField] protected int energy;

        public override void Initialize(SkillData _data)
        {
            base.Initialize(_data);

            energy = energyData.InitialEnergy;
        }

        public override void Equip()
        {
            base.Equip();

            UpdateEnergy();
        }

        #region availability
        public void RecoverEnergy(int amount)
        {
            energy = Mathf.Clamp(energy + amount, 0, energyData.MaxEnergy);
            UpdateEnergy();
        }

        protected virtual void SpendEnergy(int amount)
        {
            energy = Mathf.Clamp(energy - amount, 0, energyData.MaxEnergy);
            UpdateEnergy();
        }

        protected virtual void UpdateEnergy()
        {
            if (!isEquipped) return;

            float value = (float)energy / energyData.MaxEnergy;
            EventEnergyChanged?.Invoke(value);
        }
        #endregion
    }

    [Serializable]
    public class EnergyData
    {
        public int InitialEnergy = 40;
        public int SkillCost = 40;
        public int MaxEnergy = 100;
    }
}
