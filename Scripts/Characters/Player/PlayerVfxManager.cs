using Magia.GameLogic;
using Magia.Vfx;
using UnityEngine;

namespace Magia.Player
{
    public class PlayerVfxManager : MonoBehaviour
    {
        [SerializeField] private RibbonVfx ribbon;

        [Header("Skill")]
        [SerializeField] private Barrier barrier;
        [SerializeField] private GameObject wing;

        private ElementType currentElement;

        public void Initialize()
        {
            ribbon.Initialize();
            barrier.Initialize();
        }

        public void Dispose()
        {
            ribbon.Dispose();
            barrier.Dispose();
        }

        #region combat
        public void StartPerformSkill()
        {
            ribbon.StartPerformSkill();
        }

        public void ReleaseSkill()
        {
            ribbon.ReleaseSkill();
        }
        #endregion

        #region barrier
        public void TriggerBarrier(float duration)
        {
            barrier.LifeTrigger(duration);
        }

        public void ToggleBarrier(bool isOn)
        {
            if (isOn)
            {
                barrier.BuildUp();
                return;
            }

            barrier.Dissolve();
        }
        #endregion

        #region wing
        public void ToggleWing(bool isOn)
        {
            wing.SetActive(isOn);
        }
        #endregion

        #region ribbon
        public void UpdatePlayerSpeed(float speed)
        {
            ribbon.UpdatePlayerSpeed(speed);
        }

        public void UpdateEnergy(float lerp)
        {
            ribbon.UpdateEnergy(lerp);
        }

        public void SetElement(ElementType element)
        {
            currentElement = element;
            ribbon.SetElement(currentElement);
        }

        public void DisableElement()
        {
            currentElement = ElementType.None;
            ribbon.SetElement(currentElement);
        }
        #endregion
    }
}
