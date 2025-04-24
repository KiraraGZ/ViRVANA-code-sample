using Magia.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay.Crosshair
{
    public class UICrosshairBasicAttack : UICrosshair
    {
        [SerializeField] private Image[] streakImages;
        [SerializeField] private RectTransform streakContainer;

        [SerializeField] private Animator lightningAnim;

        public override void Initialize(int _index)
        {
            foreach (var image in streakImages)
            {
                image.enabled = false;
            }

            base.Initialize(_index);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void UpdateLogic(float value)
        {
            streakContainer.Rotate(Vector3.back, 30f * Time.deltaTime);

            base.UpdateLogic(value);
        }

        #region skill events
        public void UpdateCombo(AttackUpdateData data)
        {
            if (data.IsPerformed)
            {
                lightningAnim.SetTrigger("Perform");
            }

            for (int i = 0; i < 3; i++)
            {
                streakImages[i].enabled = data.ComboNumber > i;
            }
        }
        #endregion
    }
}
