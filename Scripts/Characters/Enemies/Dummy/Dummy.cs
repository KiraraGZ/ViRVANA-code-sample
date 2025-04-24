using System;
using System.Collections;
using Magia.Enemy.Skills;
using Magia.GameLogic;
using Magia.Player;
using UnityEngine;

namespace Magia.Enemy.Dummy
{
    public class Dummy : BaseEnemy, IDamageable
    {
        private const string CLIP_KEY = "_Clip";
        private const float TRANSITION_DURATION = 1f;

        [SerializeField] private SkinnedMeshRenderer _renderer;
        [SerializeField] private Material[] materialPrefabs;
        [SerializeField] private Transform firePoint;
        [SerializeField] private DirectFireSkillData fireData;
        private DirectFireSkill fireSkill;

        private bool canAttack;
        private bool isFiring;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);

            fireSkill = new();
            fireSkill.Initialize(fireData, firePoint, this);
            fireSkill.EventSkillEnd += OnFiringEnd;

            canAttack = false;
            isFiring = false;

            var materials = new Material[materialPrefabs.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new(materialPrefabs[i]);
                materials[i].SetFloat(CLIP_KEY, 0);
            }

            _renderer.materials = materials;
            StartCoroutine(TransitionClip(1, TRANSITION_DURATION));
        }

        public override void Dispose()
        {
            fireSkill.EventSkillEnd -= OnFiringEnd;
            fireSkill.Dispose();
            fireSkill = null;
            canAttack = false;

            StartCoroutine(TransitionClip(0, TRANSITION_DURATION, () =>
            {
                base.Dispose();

                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    Destroy(_renderer.materials[i]);
                }
            }));
        }

        private void FixedUpdate()
        {
            rb.velocity = Vector3.zero;

            if (canAttack == false) return;

            var rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
            rb.MoveRotation(rotation);

            if (!isFiring)
            {
                if (fireSkill.IsAvailable() == false) return;

                fireSkill.Cast();
                isFiring = true;
            }
            else
            {
                fireSkill.UpdateLogic();
            }
        }

        public void StartAttack()
        {
            canAttack = true;
        }

        public override DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            return new DamageFeedback(damage, 1);
        }

        protected void OnFiringEnd()
        {
            isFiring = false;
        }

        private IEnumerator TransitionClip(float target, float duration, Action callback = null)
        {
            float start = _renderer.materials[0].GetFloat(CLIP_KEY);
            float lerp = 0;

            while (lerp < 1)
            {
                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].SetFloat(CLIP_KEY, Mathf.Lerp(start, target, lerp));
                }

                lerp += Time.deltaTime / duration;
                yield return null;
            }

            callback?.Invoke();
        }
    }
}
