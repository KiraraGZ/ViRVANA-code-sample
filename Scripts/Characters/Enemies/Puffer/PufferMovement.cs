using System;
using Magia.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Puffer
{
    [RequireComponent(typeof(Rigidbody))]
    public class PufferMovement : MonoBehaviour
    {
        private PufferMovementData data;

        private float moveAngle = 0f;
        private bool isStrafe;
        private float modelWidth;

        private BasePuffer basePuffer;
        private Rigidbody Rb => basePuffer.Rigidbody;
        private PlayerController Player => basePuffer.Player;

        public void Initialize(BasePuffer _basePuffer, PufferMovementData movementData, float _modelWidth)
        {
            basePuffer = _basePuffer;
            data = movementData;

            isStrafe = false;
            modelWidth = _modelWidth;
        }

        public void Dispose()
        {
            basePuffer = null;
        }

        public void PhysicsUpdate(float speedMultiplier = 1f, bool isMove = true, bool isRotate = true, bool isClose = false, bool isDodge = true)
        {
            Vector3 playerDirection = (Player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.SignedAngle(transform.forward, playerDirection, Vector3.up);

            if (isMove)
            {
                if (isClose) Revolve(speedMultiplier, angleToPlayer < 0, isDodge);
                else Move(speedMultiplier, angleToPlayer, isDodge);
            }

            if (isRotate)
            {
                LerpLookRotation(playerDirection, isMove ? 1f : 2f);
            }
        }

        public void PhysicsUpdate(Vector3 direction, float speedMultiplier = 1f)
        {
            LerpLookRotation(direction, speedMultiplier);
            Move(speedMultiplier, 0, true);
        }

        public void EnterStrafingState()
        {
            isStrafe = true;

            float rand = Random.Range(0f, 1f);
            moveAngle = rand * 360f;
        }

        public void ExitStrafingState()
        {
            isStrafe = false;
            moveAngle = 0;
        }

        private void Move(float speedMultiplier, float angleToPlayer, bool isDodge)
        {
            Vector3 moveDirection = Quaternion.Euler(0, 0, moveAngle) * transform.forward;

            if (isDodge && Physics.SphereCast(transform.position, modelWidth, moveDirection, out _, modelWidth))
            {
                moveDirection = Vector3.Cross(moveDirection, Vector3.up).normalized;
            }

            float angleSpeedMultiplier = Mathf.Pow(1f - Mathf.Abs(angleToPlayer) / 180f, 2f);
            var currentSpeed = isStrafe ? data.StrafeSpeed : data.MovementSpeed;
            Vector3 velocity = currentSpeed * speedMultiplier * angleSpeedMultiplier * moveDirection - basePuffer.Rigidbody.velocity;
            Rb.AddForce(velocity, ForceMode.VelocityChange);
        }

        private void Revolve(float speedMultiplier, bool isRight, bool isDodge)
        {
            Vector3 moveDirection = transform.right * (isRight ? 1 : -1);

            if (isDodge && Physics.SphereCast(transform.position, 5f, moveDirection, out _, 3f))
            {
                moveDirection = Vector3.Cross(moveDirection, Vector3.up).normalized;
            }

            var currentSpeed = isStrafe ? data.StrafeSpeed : data.RevolveSpeed;
            Vector3 velocity = currentSpeed * speedMultiplier * moveDirection - basePuffer.Rigidbody.velocity;
            Rb.AddForce(velocity, ForceMode.VelocityChange);
        }

        private void LerpLookRotation(Vector3 direction, float speedMultiplier)
        {
            Quaternion smoothRotation = Quaternion.Slerp(Rb.rotation, Quaternion.LookRotation(direction), data.RotationSpeed * speedMultiplier * Time.deltaTime);
            Rb.MoveRotation(smoothRotation);
        }
    }

    [Serializable]
    public class PufferMovementData
    {
        public float MovementSpeed = 10f;
        public float StrafeSpeed = 15f;
        public float RevolveSpeed = 2f;
        public float RotationSpeed = 2f;
        public float StrafeDuration = 0.5f;
    }
}
