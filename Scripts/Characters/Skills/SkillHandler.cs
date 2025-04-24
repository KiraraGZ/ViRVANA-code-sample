using System;
using System.Collections.Generic;
using Magia.GameLogic;
using Magia.Player;
using Magia.UI.Gameplay;
using Magia.UI.Gameplay.Crosshair;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Magia.Skills
{
    public class SkillHandler : MonoBehaviour
    {
        public event Action<DamageFeedback, Vector3> EventDamageDealt;
        public event Action EventSkillEnd;

        #region for tutorial
        public event Action<int> EventAttackComboPerformed;
        public event Action EventDashPerformed;
        public event Action<int> EventSkillPerformed;
        public event Action EventSkillSetEquipped;
        #endregion

        private const float SKILL_SWITCH_COOLDOWN = 3f;

        //TODO: rearrange how to store each skill.
        [SerializeField] private AttackSkill attackSkill;
        [SerializeField] private DashSkill dashSkill;
        public DashSkill DashSkill => dashSkill;

        [SerializeField] private MagiaSkillSet magiaSkillSet;
        [SerializeField] private List<SupportSkill> supportSkillSet;

        private SkillSet equippedSkillSet;
        private int currentSkillIndex;
        private int currentSkillSet;
        private float lastSwitchTime;

        private bool preventSwitchSkillSet;

        private UICrosshairManager crosshair;
        private SkillHandlerData data;

        public void Initialize(SkillHandlerData _data)
        {
            data = _data;
            crosshair = data.UICrosshair;
            currentSkillIndex = -1;
            currentSkillSet = 0;
            lastSwitchTime = Time.time;

            attackSkill.Initialize(new(data.UISkillIcons.Attack, data.UICrosshair.Crosshairs[SkillType.ATTACK], data.Player), data.SkillUpgrade.Attack);
            attackSkill.EventAttackComboUpdate += OnAttackPerformed;
            attackSkill.EventDamageDealt += OnDamageDealt;
            attackSkill.EventSkillHit += OnAttackHit;
            attackSkill.EventSkillEnd += OnSkillEnd;

            dashSkill.Initialize(new(data.UISkillIcons.Dash, data.UICrosshair.Crosshairs[SkillType.DASH], data.Player));
            dashSkill.EventSkillPerformed += OnDashPerformed;
            dashSkill.EventDamageDealt += OnDamageDealt;
            dashSkill.EventSkillHit += OnAttackHit;

            magiaSkillSet.Initialize(data);

            foreach (var skillSet in GetSkillSets())
            {
                skillSet.EventSkillEnergyChanged += OnSkillEnergyChanged;
                skillSet.EventSkillPerformed += OnSkillPerformed;
                skillSet.EventSkillReleased += OnSkillReleased;
                skillSet.EventSkillEnd += OnSkillEnd;
                skillSet.EventDamageDealt += OnDamageDealt;
            }

            foreach (var skill in supportSkillSet)
            {
                skill.Initialize(new SupportSkillData(data.Player));
            }

            attackSkill.Equip();
            dashSkill.Equip();
            EquipSkillSet(currentSkillSet);
        }

        public void Dispose()
        {
            attackSkill.Dispose();
            attackSkill.EventAttackComboUpdate -= OnAttackPerformed;
            attackSkill.EventDamageDealt -= OnDamageDealt;
            attackSkill.EventSkillHit -= OnAttackHit;
            attackSkill.EventSkillEnd -= OnSkillEnd;

            dashSkill.Dispose();
            dashSkill.EventSkillPerformed -= OnDashPerformed;
            dashSkill.EventDamageDealt -= OnDamageDealt;
            dashSkill.EventSkillHit -= OnAttackHit;

            UnequipSkillSet();

            foreach (var skillSet in GetSkillSets())
            {
                skillSet.Dispose();

                skillSet.EventSkillEnergyChanged -= OnSkillEnergyChanged;
                skillSet.EventSkillPerformed -= OnSkillPerformed;
                skillSet.EventSkillReleased -= OnSkillReleased;
                skillSet.EventSkillEnd -= OnSkillEnd;
                skillSet.EventDamageDealt -= OnDamageDealt;
            }

            foreach (var skill in supportSkillSet)
            {
                skill.Dispose();
            }

            data = null;
        }

        public void PhysicsUpdate()
        {
            attackSkill.PhysicsUpdate();
            equippedSkillSet?.PhysicsUpdate(currentSkillIndex);

            CheckAvailableSkillSet();
        }

        public void SwitchSkill()
        {
            if (preventSwitchSkillSet == true) return;
            if (Time.time < lastSwitchTime) return;
            if (!TryEquipNextSkillSet()) return;

            data.UISkillIcons.Switch.StartCooldown(SKILL_SWITCH_COOLDOWN);
            lastSwitchTime = Time.time + SKILL_SWITCH_COOLDOWN;

            EventSkillSetEquipped?.Invoke();
        }

        private void CheckAvailableSkillSet()
        {
            if (preventSwitchSkillSet == true)
            {
                data.UISkillIcons.Switch.UpdateActive(false);
                return;
            }

            var otherSkillSetAvailable = TryGetNextSkillSetIndex(out var index);
            data.UISkillIcons.Switch.UpdateActive(otherSkillSetAvailable);

            if (!otherSkillSetAvailable) return;

            data.UISkillIcons.Switch.Equip(GetSkillSet(index).Element);
        }

        private bool TryEquipNextSkillSet()
        {
            if (!TryGetNextSkillSetIndex(out var index)) return false;

            EquipSkillSet(index);
            return true;
        }

        private void EquipSkillSet(int index)
        {
            equippedSkillSet = GetSkillSet(index);
            equippedSkillSet.EquipActiveSkills();
            currentSkillIndex = index;

            data.Player.AnimatorController.SetElement(equippedSkillSet.Element);
        }

        private void UnequipSkillSet()
        {
            if (equippedSkillSet == null) return;

            equippedSkillSet.UnequipActiveSkills();
            equippedSkillSet = null;
            currentSkillSet = -1;

            data.Player.AnimatorController.DisableElementTrail();
        }

        private bool TryGetNextSkillSetIndex(out int index)
        {
            var skillSets = GetSkillSets();
            index = -1;

            for (int i = 0; i < skillSets.Length; i++)
            {
                if (i == currentSkillSet && !skillSets[i].IsPostUlt) continue;
                if (!skillSets[i].IsAvailable()) continue;

                index = i;
                return true;
            }

            return false;
        }

        private SkillSet GetSkillSet(int index)
        {
            return index switch
            {
                0 => magiaSkillSet,
                _ => null,
            };
        }

        private SkillSet[] GetSkillSets()
        {
            return new SkillSet[] { magiaSkillSet };
        }

        #region using skill
        public void PerformAttack()
        {
            attackSkill.PerformSkill();

            equippedSkillSet.PerformPassiveSkillAttack();
        }

        public void ReleaseAttack()
        {
            attackSkill.ReleaseSkill();
        }

        public bool IsSkillAvailable(int index)
        {
            if (equippedSkillSet == null) return false;

            return equippedSkillSet.IsSkillAvailable(index);
        }

        public void PerformSkill(int index)
        {
            if (equippedSkillSet == null) return;

            var perform = equippedSkillSet.PerformSkill(index);
            currentSkillIndex = index;

            data.Player.PerformSkill(perform);

            EventSkillPerformed?.Invoke(currentSkillIndex);
        }

        public void PerformSupportSkill()
        {
            var skill = RandomPlay();
            skill.PerformSkill();
        }

        public void RepeatSkill(int index)
        {
            equippedSkillSet?.RepeatSkill(index);
        }

        public void ReleaseSkill(int index)
        {
            if (currentSkillIndex != index) return;

            equippedSkillSet?.ReleaseSkill(index);
            currentSkillIndex = -1;

            data.Player.ReleaseSkill();
        }

        public SupportSkill RandomPlay()
        {
            int idx = Random.Range(0, supportSkillSet.Count);
            return supportSkillSet[idx];
        }
        #endregion

        #region barrier
        public bool CheckBarrier(Damage damage, Vector3 direction)
        {
            var skillSet = GetSkillSet(currentSkillSet);

            if (skillSet == null) return false;

            return skillSet.CheckBarrier(damage, direction);
        }
        #endregion

        #region availability
        public void RecoverSkills()
        {
            RefreshEnergy();
            RefreshCooldown();
        }

        public void RefreshEnergy()
        {
            foreach (var skillSet in GetSkillSets())
            {
                skillSet.RefreshEnergy();
            }
        }

        public void RefreshCooldown()
        {
            foreach (var skillSet in GetSkillSets())
            {
                skillSet.RefreshSkillCooldown();
            }
        }

        public void PauseSwitchSkillSet()
        {
            preventSwitchSkillSet = true;
        }

        public void UnpauseSwitchSkillSet(bool immediateAvailable = false)
        {
            preventSwitchSkillSet = false;

            if (immediateAvailable == false) return;

            foreach (var skillSet in GetSkillSets())
            {
                skillSet.RefreshCooldown();
            }
        }
        #endregion

        #region subscribe events
        private void OnDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            EventDamageDealt?.Invoke(feedback, hitPos);
        }

        private void OnAttackPerformed(AttackUpdateData data)
        {
            crosshair.UpdateAttackCombo(data);

            if (data.IsPerformed == false) return;

            EventAttackComboPerformed?.Invoke(data.ComboNumber);
        }

        private void OnAttackHit(int amount)
        {
            equippedSkillSet?.AttackRecoverEnergy(amount);
        }

        private void OnDashPerformed(SkillType type)
        {
            EventDashPerformed?.Invoke();
        }

        private void OnSkillEnergyChanged(int index, float value)
        {
            var energySkillIcon = data.UISkillIcons.Skills[index] as UIEnergySkillIcon;
            if (energySkillIcon != null)
            {
                energySkillIcon.UpdateEnergy(value);
            }

            crosshair.UpdateEnergy(index, value);

            if (index == 2)
            {
                data.Player.AnimatorController.UpdateEnergy(value);
            }
        }

        private void OnSkillPerformed(int index, SkillType type)
        {
            data.UISkillIcons.Skills[index].PerformSkill();
            data.UICrosshair.PerformSkill(type);

            switch (index)
            {
                case 1:
                    data.Player.AnimatorController.ToggleBarrier(true);
                    break;
                case 2:
                    data.Player.AnimatorController.ToggleWing(true);
                    break;
            }
        }

        private void OnSkillReleased(int index, SkillType type)
        {
            data.UISkillIcons.Skills[index].ReleaseSkill();
            data.UICrosshair.ReleaseSkill(type);

            switch (index)
            {
                case 1:
                    data.Player.AnimatorController.ToggleBarrier(false);
                    break;
                case 2:
                    data.Player.AnimatorController.ToggleWing(false);
                    // UnequipSkillSet();
                    // var result = TryEquipNextSkillSet();
                    // data.UISkillIcons.Switch.UpdateActive(result);
                    break;
            }
        }

        private void OnSkillEnd()
        {
            EventSkillEnd?.Invoke();
        }
        #endregion
    }

    public class SkillHandlerData
    {
        public PlayerController Player;
        public UISkillIconGroup UISkillIcons;
        public UICrosshairManager UICrosshair;
        public SkillUpgradeData SkillUpgrade;

        public SkillHandlerData(PlayerController owner, UISkillIconGroup uiSkillIcons, UICrosshairManager uiCrosshair, SkillUpgradeData skillUpgrade)
        {
            Player = owner;
            UISkillIcons = uiSkillIcons;
            UICrosshair = uiCrosshair;
            SkillUpgrade = skillUpgrade;
        }
    }

    public class SkillPerformedData
    {
        public SkillType Type;
        public bool CameraZoomOut;

        public SkillPerformedData(SkillType type, bool cameraZoomOut)
        {
            Type = type;
            CameraZoomOut = cameraZoomOut;
        }
    }

    public class SkillUpgradeData
    {
        public AttackSkillUpgradeData Attack;
        public ClockSkillUpgradeData Clock;
    }
}