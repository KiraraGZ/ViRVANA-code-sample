using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay.Crosshair
{
    public class UICrosshairChargeSkill : UICrosshair
    {
        [SerializeField] private Image gauge;
        [SerializeField] private Material materialPrefab;

        private Material material;

        public override void Initialize(int _index)
        {
            base.Initialize(_index);

            gauge.material = new(materialPrefab);
            material = gauge.material;
        }

        public override void UpdateLogic(float value)
        {
            base.UpdateLogic(value);

            material.SetFloat("_Scale", value);
        }

        public override void ReleaseSkill()
        {
            base.ReleaseSkill();

            material.SetFloat("_Scale", 0);
        }
    }
}
