using Magia.Player;
using System;

namespace Magia.Skills
{
    public class SupportSkill : Skill
    {
        public event Action EventSkillPerformed;

        protected SupportSkillData skillData;

        public virtual void Initialize(SupportSkillData _data)
        {
            skillData = _data;
        }

        public virtual void Dispose()
        {
            skillData = null;
        }

        public virtual void PerformSkill()
        {
            EventSkillPerformed?.Invoke();
        }
    }

    public class SupportSkillData
    {
        public PlayerController Player;

        public SupportSkillData(PlayerController player)
        {
            Player = player;
        }

        public SupportSkillData(SkillData data)
        {
            Player = data.Player;
        }
    }
}