using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Whale
{
    public enum WhaleState
    {
        Approach,
        Decelerate,
        Reposition,
        RevolveTakeoff,
        RevolveAttack,
        Retreat,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class WhaleMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform model;

        private WhaleSO data;

        private float currentSpeed;
        private Quaternion maxTiltRotation;

        private BaseWhale baseWhale;
        private PlayerController player => baseWhale.Player;

        public void Initialize(WhaleSO _data, BaseWhale _baseWhale)
        {
            data = _data;
            baseWhale = _baseWhale;
        }

        public void Dispose()
        {
            baseWhale = null;
        }

        #region update state logic
        public void ApproachPlayer()
        {
            Vector3 directionToPlayer = GetDirectionTowardPlayer();

            rb.velocity += data.ApproachPlayerData.Acceleration * Time.deltaTime * directionToPlayer;
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, data.ApproachPlayerData.MaxApproachSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            rb.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, data.ApproachPlayerData.AngularSpeed * Time.deltaTime);

            model.localRotation = Quaternion.identity;
        }

        public void DecelerateAfterPassing()
        {
            rb.velocity -= data.DecelerationData.Deceleration * Time.deltaTime * GetCurrentDirection();
        }

        public void RepositionForNextFling()
        {
            Vector3 directionToPlayer = GetDirectionTowardPlayer();

            rb.velocity = transform.forward * data.MovementData.MinSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            rb.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, data.MovementData.AngularSpeed * Time.deltaTime);

            float tilt = CalculateTiltAngle(data.MovementData.AngularSpeed);
            model.localRotation = Quaternion.Euler(new Vector3(0, 0, tilt));
        }

        public void TakeoffRevolve()
        {
            Vector3 directionToPlayer = GetDirectionTowardPlayer();

            currentSpeed = rb.velocity.magnitude + data.RevolveData.Acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, data.RevolveData.MaxSpeed);
            rb.velocity = transform.forward * currentSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            float angularSpeed = CalculateAngularSpeed(currentSpeed, data.RevolveData.RevolveDistance);
            rb.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);

            float tilt = CalculateTiltAngle(angularSpeed);
            model.localRotation = Quaternion.Euler(0, 0, tilt);
        }

        public void MaintainRevolve()
        {
            Vector3 flatDelta = new(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
            Vector3 orbitDirection = Vector3.Cross(Vector3.up, flatDelta).normalized;
            orbitDirection *= Vector3.Dot(transform.forward, orbitDirection) < 0 ? -1 : 1;

            currentSpeed = rb.velocity.magnitude + data.RevolveData.Acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, data.RevolveData.MaxSpeed);
            rb.velocity = currentSpeed * orbitDirection;

            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
            float angularSpeed = 1.5f * CalculateAngularSpeed(currentSpeed, data.RevolveData.RevolveDistance);
            rb.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);

            float tilt = CalculateTiltAngle(angularSpeed);
            model.localRotation = Quaternion.Euler(0, 0, tilt);
        }

        public void Retreat()
        {
            rb.rotation = Quaternion.RotateTowards(transform.rotation, maxTiltRotation, data.RetreatData.TileAcceleration * Time.fixedDeltaTime);
            rb.velocity = transform.forward * data.RetreatData.Speed;
        }

        #endregion

        #region enter state
        public void EnterApproachState()
        {
            rb.velocity = transform.forward * data.ApproachPlayerData.ApproachSpeed;
        }

        public void EnterRetreatState()
        {
            Vector3 target = rb.rotation.eulerAngles;
            target.x = -data.RetreatData.MaxTilt;
            maxTiltRotation = Quaternion.Euler(target);
        }

        #endregion

        #region public methods
        public float GetCurrentSpeed()
        {
            return rb.velocity.magnitude;
        }

        public float GetAngleToPlayer()
        {
            return Vector3.Dot(GetCurrentDirection(), GetDirectionTowardPlayer());
        }

        public float GetDistanceFromPlayer()
        {
            return new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z).magnitude;
        }
        #endregion

        #region private methods
        private Vector3 GetCurrentDirection()
        {
            return rb.velocity.normalized;
        }

        private Vector3 GetDirectionTowardPlayer()
        {
            return new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z).normalized;
        }

        private float CalculateAngularSpeed(float speed, float orbitRadius)
        {
            return speed / orbitRadius * Mathf.Rad2Deg;
        }

        private float CalculateTiltAngle(float angularSpeed)
        {
            return Mathf.Clamp(-angularSpeed * data.MovementData.MaxTiltAngle / data.MovementData.AngularSpeed, -data.MovementData.MaxTiltAngle, data.MovementData.MaxTiltAngle);
        }
        #endregion
    }
}
