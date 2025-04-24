using UnityEngine;

namespace Magia.GameLogic.Progression
{
    [CreateAssetMenu(fileName = "SkillNodeData", menuName = "ScriptableObject/Progression/SkillNode")]
    public class SkillNodeDataSO : ScriptableObject
    {
        public string SkillUpgradeNameKey;
        public Sprite Icon;
        [TextArea(3, 5)] public string SkillUpgradeDescriptionKey;
    }
}
