using System;
using Magia.Enemy;
using Magia.Player;
using Magia.Projectiles;
using UnityEngine;

namespace Magia.Skills
{
    public class MagiaPassiveSkill : Skill
    {
        [SerializeField] protected MagiaPassiveSkillData data;

        private PlayerController owner;

        private ProjectilePoolManager extraPool;
        private Transform[] funnels;

        private int index;
        private Vector3 lastOwnerPos;
        private Vector3 lastOwnerDir;
        private float nextFollowTime;
        private FunnelState state;

        private enum FunnelState
        {
            LERP,
            IDLE,
            FOLLOW,
        }

        public void Initialize(PlayerController _owner)
        {
            owner = _owner;
            extraPool = new(data.ProjectilePrefab);
        }

        public void Dispose()
        {
            extraPool.Dispose();
            extraPool = null;
        }

        public void UpdateLogic()
        {
            var isPlayerMove = lastOwnerPos != owner.transform.position;

            switch (state)
            {
                case FunnelState.LERP:
                    {
                        if (isPlayerMove && Vector3.Distance(owner.transform.position, funnels[0].transform.position) < 0.9f)
                        {
                            state = FunnelState.IDLE;
                            nextFollowTime = Time.time + data.FunnelFollowInterval;
                            break;
                        }

                        UpdateFunnelLerp();
                        break;
                    }
                case FunnelState.IDLE:
                    {
                        if (isPlayerMove && Time.time > nextFollowTime)
                        {
                            state = FunnelState.FOLLOW;
                            break;
                        }

                        if (Vector3.Distance(owner.transform.position, funnels[0].transform.position) > 3f)
                        {
                            state = FunnelState.LERP;
                            break;
                        }

                        break;
                    }
                case FunnelState.FOLLOW:
                    {
                        if (!isPlayerMove)
                        {
                            state = FunnelState.LERP;
                            break;
                        }

                        UpdateFunnelFollow();
                        break;
                    }
            }

            lastOwnerPos = owner.transform.position;
            lastOwnerDir = owner.transform.forward;
        }

        private void UpdateFunnelLerp()
        {
            for (int i = 0; i < funnels.Length; i++)
            {
                var targetPosition = owner.transform.position + GetFunnelOffset(data.SpawnOffsets[i]);
                funnels[i].position = Vector3.Lerp(funnels[i].position, targetPosition, Time.deltaTime * data.FunnelLerpSpeed);
            }
        }

        private void UpdateFunnelFollow()
        {
            for (int i = 0; i < funnels.Length; i++)
            {
                Vector3 posOffset = owner.transform.position - lastOwnerPos;
                Quaternion rotationOffset = Quaternion.FromToRotation(lastOwnerDir, owner.transform.forward);
                Vector3 dirOffset = GetFunnelOffset(data.SpawnOffsets[i]);

                funnels[i].position += posOffset + (rotationOffset * dirOffset - dirOffset);
            }
        }

        public void Equip()
        {
            if (funnels != null) return;

            funnels = new Transform[data.SpawnOffsets.Length];

            for (int index = 0; index < funnels.Length; index++)
            {
                var pos = GetFunnelOffset(data.SpawnOffsets[index]);
                funnels[index] = Instantiate(data.FunnelPrefab).transform;
                funnels[index].SetPositionAndRotation(owner.transform.localPosition + pos, Quaternion.LookRotation(GetCameraDirection()));
            }

            state = FunnelState.LERP;
        }

        public void Unequip()
        {
            if (funnels == null) return;

            for (int i = 0; i < funnels.Length; i++)
            {
                Destroy(funnels[i].gameObject);
            }

            funnels = null;
        }

        public void PerformPassiveAttack()
        {
            Vector3 spawnPosition = funnels[index].position;
            Vector3 direction = GetCameraDirection();
            BaseEnemy[] targets = GetCameraClosestEnemies(1);
            GameObject target = targets.Length > 0 ? targets[index % targets.Length].gameObject : null;

            var projectile = extraPool.Rent(spawnPosition, Quaternion.LookRotation(direction)) as HomingProjectile;
            projectile.Initialize(data.HomingData, direction, owner, target);

            index = (index + 1) % funnels.Length;
        }

        private Vector3 GetFunnelOffset(Vector3 offset)
        {
            return transform.forward * offset.z + transform.right * offset.x + transform.up * offset.y;
        }
    }

    [Serializable]
    public class MagiaPassiveSkillData
    {
        [Header("Funnel")]
        public GameObject FunnelPrefab;
        public float FunnelLerpSpeed = 5f;
        public float FunnelFollowInterval = 0.06f;
        public Vector3[] SpawnOffsets;

        [Header("Projectile")]
        public HomingProjectile ProjectilePrefab;
        public HomingProjectileData HomingData;
    }
}
