using Magia.Buddy.Data;
using Magia.Enemy;
using Magia.StateMachines;
using UnityEngine;

namespace Magia.Buddy.StateMachines.CombatState
{
    public class BuddyAttackState : IState
    {
        private BuddyCombatStateMachine stateMachine;

        private float angularRotationSpeed;
        private BaseEnemy target;
        private BuddyCombatData combatData;
        private readonly BuddySkillHandler skillHandler;

        public BuddyAttackState(BuddyCombatStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            skillHandler = stateMachine.Buddy.SkillHandler;
            angularRotationSpeed = stateMachine.Buddy.Data.MovementData.AngularRotationSpeed;
        }

        public void Enter()
        {
            target = stateMachine.Buddy.ReusableData.Target;
            combatData = stateMachine.Buddy.Data.CombatData;

            stateMachine.Buddy.ReusableData.NextShoot = Time.time + combatData.AttackInterval;
        }

        public void Exit()
        {
            target = null;
            combatData = null;
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
            Vector3 directionToEnemy = GetHorizontalDirectionToEnemy();
            float angleToEnemy = Vector3.Angle(stateMachine.Buddy.transform.forward, directionToEnemy);

            if (angleToEnemy > 10f)
            {
                RotateTowardsEnemy(directionToEnemy);
                return;
            }

            if (Time.time > stateMachine.Buddy.ReusableData.NextShoot)
            {
                if (!TryAttackTarget())
                {
                    stateMachine.ChangeState(stateMachine.BlockedState);
                    return;
                }

                stateMachine.Buddy.ReusableData.NextShoot += combatData.AttackInterval;
            }
        }

        private Vector3 GetHorizontalDirectionToEnemy()
        {
            Vector3 direction = target.transform.position - stateMachine.Buddy.transform.position;
            direction.y = 0;
            return direction.normalized;
        }

        private void RotateTowardsEnemy(Vector3 directionToEnemy)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
            Quaternion smoothedRotation = Quaternion.RotateTowards(stateMachine.Buddy.transform.rotation, targetRotation, angularRotationSpeed * Time.deltaTime);

            stateMachine.Buddy.Rigidbody.MoveRotation(smoothedRotation);
        }

        private bool TryAttackTarget()
        {
            Vector3 directionToEnemy = target.transform.position - stateMachine.Buddy.transform.position;

            if (Physics.Raycast(stateMachine.Buddy.transform.position, directionToEnemy.normalized, out RaycastHit hit, combatData.AttackRange))
            {
                if (!hit.collider.TryGetComponent<EnemyHitbox>(out var _)) return false;

                skillHandler.CastSkill(target);

                return true;
            }

            return false;
        }
    }
}
