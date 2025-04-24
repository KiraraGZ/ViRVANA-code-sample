using Magia.Enemy;
using Magia.GameLogic;
using Magia.StateMachines;
using System.Collections.Generic;
using UnityEngine;

namespace Magia.Buddy.StateMachines.CombatState
{
    public class BuddyFindingState : IState
    {
        private BuddyCombatStateMachine stateMachine;

        public BuddyFindingState(BuddyCombatStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            stateMachine.Buddy.ReusableData.NextFindNewTarget = Time.time;
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
            if (Time.time > stateMachine.Buddy.ReusableData.NextFindNewTarget)
            {
                FindNewTarget();
            }
        }

        #region main method
        private void FindNewTarget()
        {
            List<BaseEnemy> possibleTargets = GameplayController.Instance.CharacterManager.Enemies;
            BaseEnemy toReturn = null;
            float distance = stateMachine.Buddy.Data.CombatData.AttackRange;

            foreach (BaseEnemy target in possibleTargets)
            {
                float distanceToEnemy = (target.transform.position - stateMachine.Buddy.transform.position).magnitude;

                if (distanceToEnemy > distance) continue;

                Vector3 directionToEnemy = (target.transform.position - stateMachine.Buddy.transform.position).normalized;
                Ray ray = new(stateMachine.Buddy.transform.position, directionToEnemy);

                if (Physics.Raycast(ray, out RaycastHit hit, distanceToEnemy))
                {
                    if (!hit.collider.TryGetComponent<EnemyHitbox>(out var hitted)) continue;

                    BaseEnemy baseEnemy = hitted.GetOwner();

                    if (baseEnemy != null)
                    {
                        toReturn = baseEnemy;
                        distance = distanceToEnemy;
                    }
                }
            }

            if (toReturn != null)
            {
                stateMachine.Buddy.ReusableData.Target = toReturn;
                stateMachine.ChangeState(stateMachine.AttackState);
            }
            else
            {
                stateMachine.Buddy.ReusableData.NextFindNewTarget = Time.time + stateMachine.Buddy.Data.CombatData.FindNewTargetInterval;
            }
        }
        #endregion
    }
}