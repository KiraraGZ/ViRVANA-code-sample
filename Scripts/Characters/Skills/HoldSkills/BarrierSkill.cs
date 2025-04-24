using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Skills
{
    public class BarrierSkill : EnergySkill
    {
        public event Action EventBarrierBlocked;

        private float nextSpendTime;

        [SerializeField] private BarrierSkillData data;

        public override void Initialize(SkillData _data)
        {
            base.Initialize(_data);

            element = ElementType.None;
        }

        public override bool IsSkillAvailable()
        {
            if (energy <= 0) return false;

            return base.IsSkillAvailable();
        }

        public override void PerformSkill()
        {
            base.PerformSkill();

            SpendEnergy(1);
            nextSpendTime = Time.time + data.CostSpendInterval;
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();

            StartCoroutine(Downtime(data.Downtime));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            skillData.Crosshair.UpdateLogic((float)energy / energyData.MaxEnergy);

            if (Time.time >= nextSpendTime)
            {
                SpendEnergy(1);
                nextSpendTime = Time.time + data.CostSpendInterval;
            }
        }

        protected override void UpdateEnergy()
        {
            base.UpdateEnergy();

            if (energy > 0) return;

            skillData.Icon.UpdateActive(false);
        }

        public virtual bool CheckBarrier(Damage damage, Vector3 direction)
        {
            if (!isSkillPerformed) return false;

            //TODO: make proper calculations like elemental weakness.
            float multiplier = damage.Type == DamageType.Explosion ? data.ExplosionDamageMultiplier : 1f;
            float cost = data.Weakness.CalculateDamage(damage) * multiplier / data.HealthPerEnergy;
            // SpendEnergy((int)cost);
            EventBarrierBlocked?.Invoke();

            if (energy > 0) return true;

            ReleaseSkill();
            return true;
        }
    }

    [Serializable]
    public class BarrierSkillData
    {
        public int HealthPerEnergy = 5;
        public ElementalWeakness Weakness;
        public float ExplosionDamageMultiplier = 2f;
        public float Downtime = 0.5f;
        public float CostSpendInterval = 0.3f;
    }
}
