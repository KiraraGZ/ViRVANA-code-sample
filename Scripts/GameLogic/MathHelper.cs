using UnityEngine;

namespace Magia.GameLogic
{
    public static class MathHelper
    {
        public static float GetDirectionAngleX(Vector3 direction)
        {
            if (direction.z < 0.2f)
            {
                return 0;
            }

            return -Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
        }

        public static float GetDirectionAngleY(Vector3 direction)
        {
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        public static float ClampAngle(float angle)
        {
            if (angle > 360f)
            {
                return angle - 360f;
            }

            if (angle < 0)
            {
                return angle + 360f;
            }

            return angle;
        }
    }
}
