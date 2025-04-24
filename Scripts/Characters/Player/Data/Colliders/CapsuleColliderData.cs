using UnityEngine;

namespace Magia.Player.Data
{
    public class CapsuleColliderData
    {
        public CapsuleCollider Collider { get; private set; }
        public Vector3 ColldierCenterLocalPosition { get; private set; }

        public void Init(Collider collider)
        {
            if (Collider != null) return;

            Collider = collider as CapsuleCollider;

            UpdateColliderData();
        }

        public void UpdateColliderData()
        {
            ColldierCenterLocalPosition = Collider.center;
        }
    }
}
