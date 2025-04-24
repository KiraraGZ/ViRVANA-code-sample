using System;
using System.Collections;
using Magia.GameLogic;
using Magia.Player;
using Magia.UI.Gameplay;
using Magia.UI.Gameplay.Crosshair;
using UnityEngine;

namespace Magia.Skills
{
    public enum SkillType
    {
        //Number is being used as animator controller params.
        ATTACK = 0,
        DASH = 100,
        CLOCK = 10,
        BARRIER = 11,
        ENERGY = 20,
        CHARGE = 30,
    }

    public class ActiveSkill : Skill
    {
        //TODO: Find better way to add and remove every Event.
        public event Action<SkillType> EventSkillPerformed;
        public event Action<SkillType> EventSkillReleased;
        public event Action EventSkillEnd;

        //TODO: Find properly way to cache skill data.
        [SerializeField] protected SkillType type;
        public SkillType Type => type;

        protected ElementType element;
        public ElementType Element => element;

        [SerializeField] protected float cooldown = 0.5f;
        protected float availableTime;

        protected bool isEquipped;
        protected bool isSkillPerformed;

        protected SkillData skillData;

        public virtual void Initialize(SkillData _data)
        {
            skillData = _data;
            isSkillPerformed = false;
        }

        public virtual void Dispose()
        {
            Unequip();

            skillData = null;

            StopAllCoroutines();
        }

        public virtual void Equip()
        {
            isEquipped = true;

            skillData.Icon.Equip(element);
        }

        public virtual void Unequip()
        {
            skillData.Icon.Unequip();
            isEquipped = false;
        }

        public virtual void PhysicsUpdate()
        {

        }

        public virtual bool IsSkillAvailable()
        {
            return Time.time >= availableTime;
        }

        public virtual void PerformSkill()
        {
            isSkillPerformed = true;
            EventSkillPerformed?.Invoke(type);
        }

        public virtual void RepeatSkill()
        {
            // skillData.Icon.RepeatSkill();
            // skillData.Crosshair.RepeatSkill();
        }

        public virtual void ReleaseSkill()
        {
            isSkillPerformed = false;
            availableTime = Time.time + cooldown;
            EventSkillReleased?.Invoke(type);
        }

        #region availability
        public void RefreshCooldown()
        {
            availableTime = Time.time;
        }

        protected virtual float GetCooldown()
        {
            return cooldown;
        }
        #endregion

        #region reusable methods
        protected IEnumerator Downtime(float downtime)
        {
            yield return new WaitForSeconds(downtime);

            OnSkillEnd();
        }
        #endregion

        #region subscribe events
        protected virtual void OnSkillEnd()
        {
            skillData.Icon.StartCooldown(GetCooldown());
            EventSkillEnd?.Invoke();
        }
        #endregion
    }

    public class SkillData
    {
        public UISkillIcon Icon;
        public UICrosshair Crosshair;
        public PlayerController Player;

        public SkillData(UISkillIcon icon, UICrosshair crosshair, PlayerController player)
        {
            Icon = icon;
            Crosshair = crosshair;
            Player = player;
        }

        public SkillData(SkillData data)
        {
            Icon = data.Icon;
            Crosshair = data.Crosshair;
            Player = data.Player;
        }
    }
}