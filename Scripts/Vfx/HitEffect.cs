using UnityEngine;

namespace Magia.Projectiles
{
    public class HitEffect : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 3f);
        }
    }
}
