using Magia.Buddy.Data;
using Magia.StateMachines;
using UnityEngine;

namespace Magia.Buddy.StateMachines.MovementStates
{
    public class BuddyMovementState : IState
    {
        protected BuddyMovementStateMachine stateMachine;
        protected BuddyMovementData movementData;

        public BuddyMovementState(BuddyMovementStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            movementData = stateMachine.Buddy.Data.MovementData;
        }

        public virtual void Enter()
        {

        }

        public virtual void Exit()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void PhysicsUpdate()
        {

        }

        protected void HoverInPlace()
        {
            Vector3 hoverPosition = stateMachine.Buddy.transform.position;
            hoverPosition.y += Mathf.Sin(Time.time * movementData.VerticalLerp) * movementData.HoverHeight;

            stateMachine.Buddy.Rigidbody.velocity = Vector3.zero;
            stateMachine.Buddy.transform.position = Vector3.Lerp(stateMachine.Buddy.transform.position, hoverPosition, Time.deltaTime * movementData.VerticalLerp);

            Rotate(stateMachine.Buddy.Player.transform.position - stateMachine.Buddy.transform.position);
        }

        protected void Move(Vector3 direction, float speedMultiplier)
        {
            direction = FindBestDirection(direction);
            Rotate(direction);
            Vector3 velocity = movementData.BaseSpeed * speedMultiplier * direction - stateMachine.Buddy.Rigidbody.velocity;
            stateMachine.Buddy.Rigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }

        private void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            stateMachine.Buddy.transform.rotation = Quaternion.RotateTowards(
                stateMachine.Buddy.transform.rotation,
                targetRotation,
                movementData.AngularRotationSpeed * Time.deltaTime
            );
        }

        protected void MoveToward(Vector3 targetPosition, float speedMultiplier)
        {
            Vector3 direction = (targetPosition - stateMachine.Buddy.transform.position).normalized;
            Move(direction, speedMultiplier);
        }

        private Vector3 FindBestDirection(Vector3 desiredDirection)
        {
            float speedFactor = stateMachine.Buddy.Rigidbody.velocity.magnitude / movementData.BaseSpeed;
            float dynamicRange = Mathf.Lerp(movementData.ObstacleAvoidanceRange * 0.5f, movementData.ObstacleAvoidanceRange * 1.5f, speedFactor);

            float bestScore = float.MinValue;
            Vector3 bestDirection = Vector3.zero;

            float maxClearance = 0f;
            Vector3 safestDirection = desiredDirection;

            Vector3 targetPosition = stateMachine.Buddy.ReusableData.Destination;
            float verticalDistance = targetPosition.y - stateMachine.Buddy.transform.position.y;
            desiredDirection.y = Mathf.Clamp(verticalDistance, -1f, 1f);

            for (int i = 0; i < movementData.RaycastSampleCount; i++)
            {
                float angle = 360f / movementData.RaycastSampleCount * i;
                Vector3 sampleDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                sampleDirection.y = desiredDirection.y;

                bool hit = Physics.Raycast(stateMachine.Buddy.transform.position, sampleDirection, out RaycastHit hitInfo, dynamicRange);

                float alignmentScore = Vector3.Dot(sampleDirection.normalized, desiredDirection.normalized);
                float clearanceScore = hit ? hitInfo.distance / dynamicRange : 1f;

                if (hit && hitInfo.distance < dynamicRange * 0.2f)
                {
                    clearanceScore = -1f;
                }

                clearanceScore += Random.Range(-0.05f, 0.05f);
                float totalScore = (clearanceScore * 0.7f) + (alignmentScore * 0.3f);

                if (Vector3.Dot(sampleDirection, stateMachine.Buddy.ReusableData.LastDirection) > 0.8f)
                {
                    totalScore += 0.5f;
                }

                if (totalScore > bestScore)
                {
                    bestScore = totalScore;
                    bestDirection = sampleDirection;
                }

                if (clearanceScore > maxClearance)
                {
                    maxClearance = clearanceScore;
                    safestDirection = sampleDirection;
                }

                Color rayColor = hit ? Color.red : Color.green;
                Debug.DrawRay(stateMachine.Buddy.transform.position, sampleDirection * dynamicRange, rayColor);
            }

            if (bestDirection == Vector3.zero) bestDirection = safestDirection;

            stateMachine.Buddy.ReusableData.LastDirection = Vector3.Slerp(stateMachine.Buddy.ReusableData.LastDirection, bestDirection, Time.deltaTime * 5f);
            Debug.DrawRay(stateMachine.Buddy.transform.position, bestDirection * dynamicRange, Color.white);
            Debug.DrawRay(stateMachine.Buddy.transform.position, stateMachine.Buddy.ReusableData.LastDirection * dynamicRange, Color.blue);

            return stateMachine.Buddy.ReusableData.LastDirection.normalized;
        }

        public void HandleInput() { }
        public void OnAnimationEnterEvent() { }
        public void OnAnimationEndEvent() { }
        public void OnAnimationTransitionEvent() { }

    }
}
