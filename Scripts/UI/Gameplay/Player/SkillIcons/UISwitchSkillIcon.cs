using Magia.GameLogic;

namespace Magia.UI.Gameplay
{
    public class UISwitchSkillIcon : UISkillIcon
    {
        public override void Equip(ElementType element)
        {
            base.Equip(element);

            foreach (var keyword in GetMaterial().enabledKeywords)
            {
                material.DisableKeyword(keyword);
            }

            material.EnableKeyword($"_ELEMENT_{element.ToString().ToUpper()}");
        }

        public override void UpdateActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
