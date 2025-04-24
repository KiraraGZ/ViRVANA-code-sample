using Magia.Enemy;
using Magia.StateMachines;
using UnityEngine;

namespace Magia.Buddy.StateMachines.CombatState
{
    public class BuddyBlockedState : IState
    {
        private BuddyCombatStateMachine stateMachine;

        private BaseEnemy target;

        public BuddyBlockedState(BuddyCombatStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            target = stateMachine.Buddy.ReusableData.Target;
            stateMachine.Buddy.ReusableData.BlockedStateTimeOut = Time.time + stateMachine.Buddy.Data.CombatData.BlockedDuration;
        }

        public void Exit()
        {

        }

        public void HandleInput()
        {

        }

        public void OnAnimationEndEvent()
        {

        }

        public void OnAnimationEnterEvent()
        {

        }

        public void OnAnimationTransitionEvent()
        {

        }

        public void Update()
        {

        }

        public void PhysicsUpdate()
        {
            if (Time.time > stateMachine.Buddy.ReusableData.NextShoot)
            {
                if (CanAttackTarget())
                {
                    stateMachine.ChangeState(stateMachine.AttackState);
                }

                stateMachine.Buddy.ReusableData.NextShoot += stateMachine.Buddy.Data.CombatData.AttackInterval;
            }

            if (Time.time > stateMachine.Buddy.ReusableData.BlockedStateTimeOut)
            {
                target = null;
                stateMachine.Buddy.ReusableData.Target = null;
                stateMachine.ChangeState(stateMachine.FindingState);
            }
        }

        private bool CanAttackTarget()
        {
            Vector3 directionToEnemy = target.transform.position - stateMachine.Buddy.transform.position;

            if (Physics.Raycast(stateMachine.Buddy.transform.position, directionToEnemy.normalized, out RaycastHit hit, stateMachine.Buddy.Data.CombatData.AttackRange))
            {
                if (!hit.collider.TryGetComponent<EnemyHitbox>(out var hitted)) return false;

                BaseEnemy baseEnemy = hitted.GetOwner();

                return baseEnemy == target;
            }

            return false;
        }
    }
}