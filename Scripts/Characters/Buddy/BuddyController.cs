using Magia.Player;
using Magia.Buddy.Data;
using UnityEngine;
using Magia.GameLogic;
using System;

namespace Magia.Buddy
{
    public class BuddyController : MonoBehaviour, IDamageable
    {
        public event Action EventUseAttack;
        public event Action EventUseMark;

        public BuddySO Data;
        public BuddyStateReusableData ReusableData;

        [SerializeField] private Rigidbody rb;
        public Rigidbody Rigidbody => rb;
        public PlayerController Player { get; private set; }

        [SerializeField] private BuddySkillHandler skillHandler;
        public BuddySkillHandler SkillHandler => skillHandler;

        private BuddyMovementStateMachine movementStateMachine;
        private BuddyCombatStateMachine combatStateMachine;

        public void Initialize(PlayerController _player)
        {
            Player = _player;

            ReusableData = new BuddyStateReusableData();
            movementStateMachine = new BuddyMovementStateMachine(this);
            combatStateMachine = new BuddyCombatStateMachine(this);

            skillHandler.Initialize(this);
            skillHandler.EventUseAttack += OnUseAttack;
            skillHandler.EventUseMark += OnUseMark;

            movementStateMachine.ChangeState(movementStateMachine.IdleState);
            // combatStateMachine.ChangeState(combatStateMachine.FindingState);
            combatStateMachine.ChangeState(combatStateMachine.IdleState);
        }

        public void Dispose()
        {
            movementStateMachine.Dispose();

            ReusableData = null;
            movementStateMachine = null;
            combatStateMachine = null;

            skillHandler.Dispose();
            skillHandler.EventUseAttack -= OnUseAttack;
            skillHandler.EventUseMark -= OnUseMark;
        }

        private void Update()
        {
            movementStateMachine.Update();
            combatStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
            combatStateMachine.PhysicsUpdate();
        }

        public void SetDestination(Vector3 destination)
        {
            movementStateMachine.ChangeState(movementStateMachine.GuideState);
            ReusableData.Destination = destination + Vector3.up;
        }

        public virtual DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            return new DamageFeedback(false);
        }

        #region subscribe events
        private void OnUseAttack()
        {
            EventUseAttack?.Invoke();
        }

        private void OnUseMark()
        {
            EventUseMark?.Invoke();
        }
        #endregion
    }
}