using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class HyperPuffer : BasePuffer
    {
        [Header("Hyper Mechanics")]
        [SerializeField] private int rageRequire = 5;
        [SerializeField] private int damageToRage = 10;
        [Space(10)]
        [SerializeField] private ParticleSystem smokeParticle;
        [SerializeField] private GameObject explodePrefab;
        [SerializeField] private float explodeRadius;
        [Space(10)]
        [SerializeField] private float transformTime = 2f;
        [SerializeField] private float clipStart = 0.25f;
        [SerializeField] private float clipAttack = 0.75f;
        [SerializeField] private float clipRage = 0.9f;

        private int rageCount;
        private bool isRage;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);

            rageCount = 0;
            isRage = false;

            animator.SetTargetClip(clipStart, transformTime);
            smokeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        protected override void UpdateFollowState()
        {
            if (!isRage)
            {
                base.UpdateFollowState();
                return;
            }

            var distance = Vector3.Distance(Player.transform.position, transform.position);
            if (distance < revolveRange)
            {
                var speedMultiplier = 1 + Mathf.Pow(1f - distance / revolveRange, 2f);
                movement.PhysicsUpdate(speedMultiplier, isDodge: false);
            }
        }

        protected override void EnterHoldState()
        {
            base.EnterHoldState();

            animator.SetTargetClip(clipAttack, transformTime);
        }

        protected override bool ChangeState(PufferState state)
        {
            if (state == PufferState.FIRE)
            {
                rageCount++;
            }

            var changed = base.ChangeState(state);

            if (!isRage && currentState == PufferState.FIRE && rageCount >= rageRequire)
            {
                isRage = true;
                animator.SetTargetClip(clipRage, transformTime);
                smokeParticle.Play();
            }

            return changed;
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            var feedback = base.TakeDamage(damage, hitPoint, hitDirection, owner);

            if (feedback.Amount >= damageToRage) rageCount++;

            if (health <= 0 && isRage) Explode();

            return feedback;
        }

        private void Explode()
        {
            var explode = Instantiate(explodePrefab, transform.position, Quaternion.identity);
            explode.transform.localScale = Vector3.one * explodeRadius;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explodeRadius);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (!hitColliders[i].TryGetComponent<IDamageable>(out var damageable)) continue;

                Damage damage = new(100, ElementType.Fire, DamageType.Explosion);
                damageable.TakeDamage(damage, hitColliders[i].ClosestPoint(transform.position), hitColliders[i].transform.position - transform.position, this);
            }
        }

        #region subscribe events
        protected override void OnFiringEnd()
        {
            animator.SetTargetClip(clipStart, transformTime);
        }
        #endregion
    }
}
