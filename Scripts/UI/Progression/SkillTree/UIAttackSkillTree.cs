using Magia.GameLogic.Progression;

namespace Magia.UI.Progression
{
    public class UIAttackSkillTree : UISkillTree
    {
        private AttackSkillTree attackSkillTree => skillTree as AttackSkillTree;
    }
}
