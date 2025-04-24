using System;
using System.Collections.Generic;
using Magia.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Enemy.Angel
{
    [Serializable]
    public class AngelPillarSkill
    {
        public enum Mode
        {
            SCATTER,
            TRACK,
            FOLLOW,
            SHAPE,
        }

        [SerializeField] private AngelPillarSkillData data;
        private Mode currentMode;

        private BaseAngel baseAngel;
        private ProjectilePoolManager projectilePool;

        private int pillarAmount;
        private float nextCastTime;
        private AngelPillarSpawnData spawnData;

        public void Initialize(BaseAngel _baseAngel)
        {
            baseAngel = _baseAngel;

            projectilePool = new(data.PillarPrefab);

            currentMode = Mode.SCATTER;
            Setup();
            nextCastTime = Time.time + spawnData.Cooldown;
        }

        public void Dispose()
        {
            baseAngel = null;

            projectilePool.Dispose();
            projectilePool = null;
        }

        public void PhysicsUpdate()
        {
            if (Time.time < nextCastTime) return;

            if (currentMode != Mode.SHAPE)
            {
                CastPillar();
                pillarAmount--;

                if (pillarAmount == 0)
                {
                    currentMode = (Mode)((int)(currentMode + 1) % 4);
                    Setup();
                }

                nextCastTime = Time.time + (pillarAmount > 1 ? spawnData.Interval : spawnData.Cooldown);
            }
            else
            {
                CastPillars();
                currentMode = (Mode)((int)(currentMode + 1) % 4);
                Setup();
                nextCastTime = Time.time + spawnData.Cooldown;
            }
        }

        private void Setup()
        {
            switch (currentMode)
            {
                case Mode.TRACK:
                    {
                        spawnData = data.TrackModeData;
                        break;
                    }
                case Mode.FOLLOW:
                    {
                        spawnData = data.FollowModeData;
                        break;
                    }
                case Mode.SCATTER:
                    {
                        spawnData = data.ScatterModeData;
                        break;
                    }
                case Mode.SHAPE:
                    {
                        spawnData = data.ShapeModeData;
                        break;
                    }
                default:
                    {
                        spawnData = data.ScatterModeData;
                        break;
                    }
            }

            pillarAmount = spawnData.Amount;
        }

        private void CastPillar()
        {
            Vector3 spawnPosition;
            Vector3 direction;

            switch (currentMode)
            {
                case Mode.TRACK:
                    {
                        spawnPosition = baseAngel.Player.transform.position;
                        direction = Vector3.down;
                        break;
                    }
                default:
                    {
                        spawnPosition = RandomSpawnPosition(spawnData.Radius);
                        direction = Vector3.down;
                        break;
                    }
            }

            spawnPosition.y = 80;
            var pillar = projectilePool.Rent(spawnPosition, Quaternion.Euler(direction)) as AngelPillar;
            pillar.Initialize(spawnData.PillarData, baseAngel.Player.transform, baseAngel);
        }

        private void CastPillars()
        {
            List<Vector3> spawnPositions = new();

            switch (currentMode)
            {
                case Mode.SHAPE:
                    SpawnPositions(spawnData.Amount, spawnData.Radius, ref spawnPositions);
                    break;
            }

            foreach (var pos in spawnPositions)
            {
                var pillar = projectilePool.Rent(pos, Quaternion.Euler(Vector3.down)) as AngelPillar;
                pillar.Initialize(spawnData.PillarData, baseAngel.Player.transform, baseAngel, baseAngel.transform.position);
            }
        }

        private Vector3 RandomSpawnPosition(float radius)
        {
            Vector2 rand = Random.insideUnitCircle;
            return baseAngel.transform.position +
                   radius * rand.x * baseAngel.transform.forward +
                   radius * rand.y * baseAngel.transform.right;
        }

        private void SpawnPositions(int amount, float radius, ref List<Vector3> positions)
        {
            Vector3 centerPosition = baseAngel.transform.position;
            for (int i = 0; i != amount; i++)
            {
                float degree = (float)360 * i / amount;
                Vector3 localPosition = radius * new Vector3(Mathf.Sin(degree), 0, Mathf.Cos(degree));
                Vector3 pos = centerPosition + localPosition;
                pos.y = 80;
                positions.Add(pos);
            }
        }
    }

    [Serializable]
    public class AngelPillarSkillData
    {
        public AngelPillar PillarPrefab;

        public AngelPillarSpawnData ScatterModeData;
        public AngelPillarSpawnData TrackModeData;
        public AngelPillarSpawnData FollowModeData;
        public AngelPillarSpawnData ShapeModeData;
    }

    [Serializable]
    public class AngelPillarSpawnData
    {
        public AngelPillarData PillarData;
        public float Radius = 30f;
        public int Amount = 15;
        public float Interval = 0.05f;
        public float Cooldown = 10f;
    }
}