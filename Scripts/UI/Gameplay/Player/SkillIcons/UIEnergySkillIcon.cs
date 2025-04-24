using Magia.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIEnergySkillIcon : UISkillIcon
    {
        private const string ACTIVE_KEY = "IsActive";
        private const string PERFORM_KEY = "Perform";
        private const string REPEAT_KEY = "Repeat";
        private const string RELEASE_KEY = "Release";

        private const string HIGHLIGHT_KEY = "_Highlight";

        [SerializeField] private Image energy;
        [SerializeField] private Material energyMaterial;
        [SerializeField] private Image highlight;
        [SerializeField] private Animator animator;

        public override void Initialize()
        {
            base.Initialize();

            energy.material = new(energyMaterial);
        }

        public override void Dispose()
        {
            base.Dispose();

            Destroy(energy.material);
        }

        public override void Equip(ElementType element)
        {
            base.Equip(element);

            foreach (var keyword in GetMaterial().enabledKeywords)
            {
                material.DisableKeyword(keyword);
            }

            material.EnableKeyword($"_ELEMENT_{element.ToString().ToUpper()}");
        }

        public void UpdateEnergy(float amount)
        {
            energy.fillAmount = amount;
            energy.material.SetFloat(HIGHLIGHT_KEY, amount == 1 ? 1 : 0);

            if (highlight == null) return;

            highlight.enabled = amount == 1;
        }

        public override void PerformSkill()
        {
            animator.SetTrigger(PERFORM_KEY);
        }

        public override void RepeatSkill()
        {
            animator.SetTrigger(REPEAT_KEY);
        }

        public override void ReleaseSkill()
        {
            animator.SetTrigger(RELEASE_KEY);
        }

        public override void UpdateActive(bool isActive)
        {
            if (gameObject.activeInHierarchy == false) return;

            animator.SetBool(ACTIVE_KEY, isActive);
        }
    }
}
