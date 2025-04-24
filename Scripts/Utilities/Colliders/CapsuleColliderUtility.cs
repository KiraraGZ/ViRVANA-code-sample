using System;
using Magia.Player.Data;
using UnityEngine;

namespace Magia.Utilities
{
    [Serializable]
    public class CapsuleColliderUtility
    {
        private CapsuleColliderData CapsuleColliderData;
        [SerializeField] private DefaultColliderData DefaultColliderData;
        [SerializeField] private SlopeData SlopeData;

        public void Init(Collider collider)
        {
            if (CapsuleColliderData != null) return;

            CapsuleColliderData = new CapsuleColliderData();

            CapsuleColliderData.Init(collider);
        }

        public void UpdateCollider(PlayerStateReusableData data, float speedRatio)
        {
            speedRatio = Mathf.Clamp(speedRatio, 1, data.MovementSpeedModifier + 1);
            speedRatio = data.IsGrounded ? (speedRatio - 1f) / 3 + 1 : speedRatio;

            SetCapsuleColliderHeight(DefaultColliderData.Height / speedRatio, data.IsGrounded);
        }

        public Vector3 GetCenterPosition()
        {
            return CapsuleColliderData.Collider.bounds.center;
        }

        public Vector3 GetLocalCenterPosition()
        {
            return CapsuleColliderData.ColldierCenterLocalPosition;
        }

        public SlopeData GetSlopeData()
        {
            return SlopeData;
        }

        private void SetCapsuleColliderHeight(float height, bool IsGrounded)
        {
            CapsuleColliderData.Collider.height = height;
            var center = DefaultColliderData.CenterY - (IsGrounded ? (DefaultColliderData.Height - height) / 2 : 0);
            CapsuleColliderData.Collider.center = new Vector3(0f, center, 0f);

            if (CapsuleColliderData.Collider.height / 2f < CapsuleColliderData.Collider.radius)
            {
                SetCapsuleColliderRadius(CapsuleColliderData.Collider.height / 2f);
            }

            CapsuleColliderData.UpdateColliderData();
        }

        private void SetCapsuleColliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }
    }
}
