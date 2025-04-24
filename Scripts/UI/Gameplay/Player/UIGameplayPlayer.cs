using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.UI.Gameplay
{
    public class UIGameplayPlayer : MonoBehaviour
    {
        [SerializeField] private UIPlayerStatus status;

        [Header("Skill Icons")]
        [SerializeField] private UISkillIcon attackIcon;
        [SerializeField] private UISkillIcon dashIcon;
        [SerializeField] private UISkillIcon switchIcon;
        [SerializeField] private UISkillIcon[] skillIcons;

        public void Initialize(PlayerController player)
        {
            status.Initialize(player);

            attackIcon.Initialize();
            dashIcon.Initialize();
            switchIcon.Initialize();
            switchIcon.Equip(ElementType.None);

            foreach (var skillIcon in skillIcons)
            {
                skillIcon.Initialize();
            }

            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            status.Dispose();

            attackIcon.Dispose();
            dashIcon.Dispose();
            switchIcon.Dispose();

            foreach (var skillIcon in skillIcons)
            {
                skillIcon.Dispose();
            }

            gameObject.SetActive(false);
        }

        public void UpdateLogic()
        {
            attackIcon.UpdateLogic();
            dashIcon.UpdateLogic();
            switchIcon.UpdateLogic();

            foreach (var skillIcon in skillIcons)
            {
                skillIcon.UpdateLogic();
            }
        }

        public UISkillIconGroup GetSkillIcons()
        {
            return new UISkillIconGroup(attackIcon, dashIcon, switchIcon, skillIcons);
        }
    }

    public class UISkillIconGroup
    {
        public UISkillIcon Attack;
        public UISkillIcon Dash;
        public UISkillIcon Switch;
        public UISkillIcon[] Skills;

        public UISkillIconGroup(UISkillIcon attack, UISkillIcon dash, UISkillIcon _switch, UISkillIcon[] skills)
        {
            Attack = attack;
            Dash = dash;
            Switch = _switch;
            Skills = skills;
        }
    }
}
