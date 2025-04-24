using UnityEngine;

namespace Magia.Spirits
{
    public class Spirit : ContactableObject
    {
        [SerializeField] private Rigidbody rb;

        private void FixedUpdate()
        {
            Vector3 hoverPosition = transform.position;
            hoverPosition.y += Mathf.Sin(Time.time);

            rb.position = Vector3.Lerp(transform.position, hoverPosition, Time.deltaTime);
            rb.MoveRotation(Quaternion.Euler(0, Time.time * 10, 0));
        }
    }
}
