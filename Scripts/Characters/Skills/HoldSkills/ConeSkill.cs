using System;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Skills
{
    public class ConeSkill : EnergySkill
    {
        //TODO: Move this data to player progression manager once it is implemented.
        [SerializeField] private ConeSkillData data;

        private ParticleSystem vfx;
        private Damage damage;

        private float lastDamageTime;

        public override void Initialize(SkillData _data)
        {
            element = data.Element;
            damage = new(data.DamagePerHit, data.Element, DamageType.Effect);

            vfx = Instantiate(data.VfxPrefab, transform);

            base.Initialize(_data);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (vfx != null)
            {
                Destroy(vfx.gameObject);
            }
        }

        public override void PerformSkill()
        {
            base.PerformSkill();

            vfx.Play();
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();

            vfx.Stop();

            StartCoroutine(Downtime(data.Downtime));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            skillData.Crosshair.UpdateLogic((float)energy / energyData.MaxEnergy);

            if (lastDamageTime > Time.time) return;

            CheckConeRadius(GetCameraDirection(), data.ConeAngle, data.Radius);
            SpendEnergy(data.SkillCostPerHit);
            lastDamageTime = Time.time + data.Interval;

            if (energy > data.SkillCostPerHit) return;

            ReleaseSkill();
        }

        private void CheckConeRadius(Vector3 direction, float coneAngle, float radius)
        {
            bool isHit = false;

            foreach (var collider in Physics.OverlapSphere(transform.position, radius))
            {
                Vector3 targetDirection = collider.transform.position - transform.position;
                float angle = Vector3.Angle(direction, targetDirection);

                if (angle > coneAngle / 2) continue;

                if (!collider.TryGetComponent<IDamageable>(out var damageable)) continue;

                damageable.TakeDamage(damage, collider.transform.position, collider.transform.position - skillData.Player.transform.position, skillData.Player);
                isHit = true;
            }

            if (!isHit) return;
        }
    }

    [Serializable]
    public class ConeSkillData
    {
        public int InitialCost = 10;
        public int SkillCostPerHit = 2;
        public float Downtime = 0.5f;

        public ParticleSystem VfxPrefab;

        public ElementType Element;
        public int DamagePerHit = 10;
        public float Interval = 0.4f;
        public float ConeAngle = 45f;
        public float Radius = 12f;
    }
}
