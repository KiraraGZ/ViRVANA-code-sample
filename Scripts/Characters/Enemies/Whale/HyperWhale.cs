using System.Collections;
using Magia.Environment;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Whale
{
    public class HyperWhale : BaseWhale
    {
        private const string CLIP_KEY = "_Clip";

        [Header("Hyper Mechanics")]
        [SerializeField] private float transformTime = 5f;
        [SerializeField] private ParticleSystem smokeParticle;
        [SerializeField] private ParticleSystem flareParticle;
        [SerializeField] private Vector2 clipRange;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private SkinnedMeshRenderer tentacleRenderer;
        [SerializeField] private Material[] materials;
        [SerializeField] private Material tentacleMaterial;

        private float currentClip;
        private float targetClip;

        private Coroutine tentacleCoroutine;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);

            currentClip = clipRange.x;
            targetClip = clipRange.x;

            var newMaterials = new Material[materials.Length];

            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                newMaterials[i] = new(materials[i]);
            }

            meshRenderer.materials = newMaterials;
            tentacleRenderer.material = new(tentacleMaterial);
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Destroy(meshRenderer.materials[i]);
            }

            Destroy(tentacleRenderer.material);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            UpdateSkin();
        }

        protected override void ChangeState(WhaleState newState)
        {
            base.ChangeState(newState);

            if (newState == WhaleState.Retreat)
            {
                targetClip = 0;
                smokeParticle.Stop();
                EnvironmentController.Instance.SetVolumeHeat(0f, transformTime);
            }
        }

        public override void ChangePhase()
        {
            base.ChangePhase();

            if (currentPhase == 1)
            {
                targetClip = 1;
                smokeParticle.Play();
                EnvironmentController.Instance.SetVolumeHeat(0.8f, transformTime);
            }
        }

        //TODO: refactor material updating logic. Check example at puffer animator controller.
        private void UpdateSkin()
        {
            currentClip += (targetClip == clipRange.x ? -1 : 1) / transformTime * Time.deltaTime;
            currentClip = Mathf.Clamp(currentClip, clipRange.x, clipRange.y);
            meshRenderer.materials[0].SetFloat("_Clip", currentClip);
            meshRenderer.materials[1].SetFloat("_ColorClip", currentClip);
        }

        private IEnumerator SetTantacle(float target, float duration)
        {
            float start = tentacleRenderer.material.GetFloat(CLIP_KEY);
            float lerp = 0;

            while (lerp < 1)
            {
                tentacleRenderer.material.SetFloat(CLIP_KEY, Mathf.Lerp(start, target, lerp));
                lerp += Time.deltaTime / duration;
                yield return null;
            }
        }

        #region subscribe events
        protected override void OnSkillPerformed()
        {
            //TODO: only lit up the tentacle and flare when performing bomb skill.
            if (tentacleCoroutine != null)
            {
                StopCoroutine(tentacleCoroutine);
            }

            flareParticle.Play();
            tentacleCoroutine = StartCoroutine(SetTantacle(1, 0.3f));
        }

        protected override void OnSkillEnd()
        {
            if (tentacleCoroutine != null)
            {
                StopCoroutine(tentacleCoroutine);
            }

            StartCoroutine(SetTantacle(0, 0.3f));
        }
        #endregion
    }
}
