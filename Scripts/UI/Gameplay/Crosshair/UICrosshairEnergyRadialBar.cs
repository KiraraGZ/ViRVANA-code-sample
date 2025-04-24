using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay.Crosshair
{
    public class UICrosshairEnergyRadialBar : MonoBehaviour
    {
        [SerializeField] private Image gauge;
        [SerializeField] private float range = 0.25f;

        public void Initialize()
        {

        }

        public void Dispose()
        {

        }

        public void UpdateEnergy(float value)
        {
            gauge.fillAmount = value * range;
        }
    }
}
