using Magia.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Magia.UI.Gameplay
{
    public class UISkillIcon : MonoBehaviour
    {
        protected const string FILL_KEY = "_Fill";

        [Header("Main")]
        [SerializeField] protected Image baseImage;
        [SerializeField] private Material baseMaterial;

        protected Material material;

        private float cooldown;
        private float initialCooldown;

        public virtual void Initialize()
        {

        }

        public virtual void Dispose()
        {
            Destroy(baseImage.material);
            material = null;
        }

        public virtual void Equip(ElementType element)
        {
            GetMaterial();
            gameObject.SetActive(true);
        }

        public virtual void Unequip()
        {
            gameObject.SetActive(false);
        }

        public virtual void UpdateLogic()
        {
            if (cooldown <= 0) return;

            cooldown -= Time.deltaTime;
            material.SetFloat(FILL_KEY, cooldown / initialCooldown);

            if (cooldown > 0) return;

            UpdateActive(true);
        }

        public void StartCooldown(float duration)
        {
            if (duration == 0) return;

            cooldown = duration;
            initialCooldown = duration;
            UpdateActive(false);
        }

        public virtual void PerformSkill()
        {

        }

        public virtual void RepeatSkill()
        {

        }

        public virtual void ReleaseSkill()
        {

        }

        public virtual void UpdateActive(bool isActive)
        {

        }

        protected Material GetMaterial()
        {
            if (material != null) return material;

            material = new(baseMaterial);
            baseImage.material = material;
            return material;
        }
    }
}
