using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UIUltimateSkillIcon : UISkillIcon
    {
        private const string HIGHLIGHT_KEY = "_Highlight";

        [Header("Ult")]
        [SerializeField] private Image energy;
        [SerializeField] private Material energyMaterial;
        [SerializeField] private Image extra;
        [SerializeField] private Material extraMaterial;
        [SerializeField] private Image star;
        [SerializeField] private Animator animator;

        private bool isActive;

        public override void Initialize()
        {
            base.Initialize();

            energy.material = new(energyMaterial);
            extra.material = new(extraMaterial);

            baseImage.DOFade(0.4f, 0);
            extra.DOFade(0.4f, 0);
        }

        public override void Dispose()
        {
            base.Dispose();

            Destroy(energy.material);
            Destroy(extra.material);
        }

        public void UpdateEnergy(float amount, float skillCost, float MaxEnergy)
        {
            baseImage.material.SetFloat(HIGHLIGHT_KEY, amount >= skillCost ? 1 : 0);
            material.SetFloat(FILL_KEY, amount / skillCost);
            energy.material.SetFloat(FILL_KEY, amount / skillCost);
            extra.material.SetFloat(FILL_KEY, (amount - skillCost) / (MaxEnergy - skillCost));
            star.enabled = amount >= skillCost;

            if (isActive == false && amount >= skillCost)
            {
                isActive = true;
                baseImage.DOFade(1f, 0.5f);
                extra.DOFade(1f, 0.5f);
            }
            else if (isActive && amount < skillCost)
            {
                isActive = false;
                baseImage.DOFade(0.4f, 0.5f);
                extra.DOFade(0.4f, 0.5f);
            }
        }
    }
}
