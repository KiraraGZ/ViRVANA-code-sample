using UnityEngine;

namespace Magia.Utilities.Gameplay
{
    public class GizmosVisualizer : MonoBehaviour
    {
        [SerializeField] private bool on = true;
        [SerializeField] private Vector3 direction;
        [SerializeField] private float sphereRadius;

        [SerializeField] private Color color;

        void OnDrawGizmos()
        {
            if (!on) return;

            Gizmos.color = color;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * direction.magnitude);
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
    }
}
