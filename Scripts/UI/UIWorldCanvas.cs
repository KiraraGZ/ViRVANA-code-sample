using Magia.GameLogic;
using Magia.UI.Gameplay;
using UnityEngine;

namespace Magia.UI
{
    public class UIWorldCanvas : UICanvas
    {
        private Transform playerTransform;

        protected override void Start()
        {
            base.Start();

            playerTransform = GameplayController.Instance.GetPlayer().transform;
        }

        public void DisplayDamageFeedback(UIDamageIndicator indicator, Vector3 position)
        {
            float distance = (position - playerTransform.position).magnitude;

            indicator.transform.SetParent(transform);
            indicator.transform.SetPositionAndRotation(position, Quaternion.Euler(canvas.worldCamera.transform.forward));
            indicator.transform.localScale = Vector3.one * GetDamageNumberScale(distance);
        }

        public void DisplayMarkIndicator(UIMarkIndicator indicator, Transform target)
        {
            indicator.transform.SetParent(transform);
            indicator.transform.SetPositionAndRotation(target.position, Quaternion.Euler(canvas.worldCamera.transform.forward));
        }

        private float GetDamageNumberScale(float distance)
        {
            return Mathf.Clamp(distance / 20f, 0.4f, 1f);
        }
    }
}
