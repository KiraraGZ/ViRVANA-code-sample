using System;
using Magia.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Slug
{
    [RequireComponent(typeof(Rigidbody))]
    public class SlugMovement : MonoBehaviour
    {
        private SlugMovementData data;

        private BaseSlug slug;
        private Rigidbody Rb => slug.Rigidbody;
        private PlayerController Player => slug.Player;

        public void Initialize(BaseSlug _basePiller, SlugMovementData movementData)
        {
            slug = _basePiller;
            data = movementData;
            Rb.drag = 0.3f;
        }

        public void Dispose()
        {
            slug = null;
        }

        public void UpdateLogic()
        {
            if (transform.position.y < data.StandbyHeightRange.x)
            {
                Move(Vector3.up);
                return;
            }

            if (transform.position.y > data.StandbyHeightRange.y)
            {
                Move(Vector3.down);
                return;
            }
        }

        public void UpdateCombatLogic()
        {
            float higher = transform.position.y - Player.transform.position.y;

            if (higher < 5f && transform.position.y < data.StandbyHeightRange.y)
            {
                Move(Vector3.up);
                return;
            }

            if (higher < -5f && transform.position.y > data.StandbyHeightRange.x)
            {
                Move(Vector3.down + transform.forward);
            }
        }

        public void Move(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            Rb.AddForce(direction * data.LevitateSpeed);
            Rb.velocity = Vector3.ClampMagnitude(Rb.velocity, 20f);
        }

        public void LerpLookRotation()
        {
            var direction = Player.transform.position - transform.position;
            direction.y = 0;
            Quaternion smoothRotation = Quaternion.Slerp(Rb.rotation, Quaternion.LookRotation(direction), data.RotationSpeed * Time.deltaTime);
            Rb.MoveRotation(smoothRotation);
        }
    }

    [Serializable]
    public class SlugMovementData
    {
        public float LevitateSpeed = 10f;
        public float RotationSpeed = 0.5f;
        public float MinEnterHeight = 20f;
        public float MaxEnterHeight = 40f;

        //TODO: Remove this params and improve movement behavior.
        public Vector2 StandbyHeightRange;

        public float GetRandomEnterHeight()
        {
            return Random.Range(MinEnterHeight, MaxEnterHeight);
        }
    }
}