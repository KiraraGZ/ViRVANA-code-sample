using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay.Crosshair
{
    public class UICrosshairConeSkill : UICrosshair
    {
        [SerializeField] private Image gauge;

        public override void UpdateLogic(float value)
        {
            gauge.fillAmount = value;

            base.UpdateLogic(value);
        }
    }
}
