using AYellowpaper.SerializedCollections;
using Magia.Skills;
using UnityEngine;

namespace Magia.UI.Gameplay.Crosshair
{
    public class UICrosshairManager : MonoBehaviour
    {
        [SerializedDictionary("SkillType", "UICrosshair")]
        public SerializedDictionary<SkillType, UICrosshair> Crosshairs;
        [SerializeField] private UICrosshairBasicAttack basicAttack;
        public UICrosshairBasicAttack BasicAttack => basicAttack;
        [SerializeField] private UICrosshairConeSkill coneSkill;
        [SerializeField] private UICrosshairChargeSkill chargeSkill;

        [SerializeField] private UICrosshairEnergyRadialBar[] energyBars;

        private UICrosshair currentCrosshair;

        public void Initialize()
        {
            currentCrosshair = basicAttack;

            basicAttack.Initialize(0);
            coneSkill.Initialize(1);
            chargeSkill.Initialize(2);

            currentCrosshair = basicAttack;
            basicAttack.Show();

            foreach (var energyBar in energyBars)
            {
                energyBar.Initialize();
            }
        }

        public void Dispose()
        {
            ExitCurrentMode();

            basicAttack.Dispose();
            coneSkill.Dispose();
            chargeSkill.Dispose();

            foreach (var energyBar in energyBars)
            {
                energyBar.Dispose();
            }
        }

        public void UpdateLogic()
        {

        }

        private void ExitCurrentMode()
        {
            if (currentCrosshair == null) return;

            currentCrosshair.Hide();
        }

        public void UpdateAttackCombo(AttackUpdateData data)
        {
            OnSwitchCrosshair(0);
            basicAttack.UpdateCombo(data);
        }

        public void UpdateEnergy(int index, float value)
        {
            energyBars[index].UpdateEnergy(value);
        }

        public void PerformSkill(SkillType type)
        {
            Crosshairs[type].PerformSkill();
            SwitchCrosshair(Crosshairs[type]);
        }

        public void ReleaseSkill(SkillType type)
        {
            Crosshairs[type].ReleaseSkill();
            OnStopCrosshair();
        }

        private void SwitchCrosshair(UICrosshair crosshair)
        {
            if (crosshair == currentCrosshair) return;

            ExitCurrentMode();

            currentCrosshair = crosshair;
            currentCrosshair.Show();
        }

        #region subscribe events
        private void OnSwitchCrosshair(int index)
        {
            UICrosshair crosshair = index switch
            {
                0 => basicAttack,
                1 => coneSkill,
                2 => chargeSkill,
                _ => basicAttack,
            };

            SwitchCrosshair(crosshair);
        }

        private void OnStopCrosshair()
        {
            ExitCurrentMode();

            currentCrosshair = basicAttack;
            currentCrosshair.Show();
        }
        #endregion
    }
}
