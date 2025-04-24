using UnityEngine;

namespace Magia.Enemy.Angel
{
    public class AngelMovement : MonoBehaviour
    {
        private BaseAngel baseAngel;
        private Rigidbody rb => baseAngel.Rigidbody;

        public void Initialize(BaseAngel _baseAngel)
        {
            baseAngel = _baseAngel;
        }

        public void Dispose()
        {
            baseAngel = null;
        }

        public void PhysicsUpdate()
        {
            Vector3 playerDirection = (baseAngel.Player.transform.position - transform.position).normalized;

            Floating();
            LerpLookRotation(playerDirection);
        }

        private void Floating()
        {
            if (transform.position.y < 50f)
            {
                rb.velocity = Vector3.up;
            }
        }

        private void LerpLookRotation(Vector3 direction)
        {
            direction.y = direction.y / 3;
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(direction), 1 * Time.deltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }
}
