using System;
using System.Collections.Generic;
using Magia.Projectiles;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Magia.Enemy.Seraph
{
    [Serializable]
    public class SeraphPillarSkill
    {
        [SerializeField] private SeraphPillarSkillData data;

        private BaseSeraph baseSeraph;
        protected SeraphCombat combat;
        private ProjectilePoolManager projectilePool;

        private int pillarAmount;
        private float nextCastTime;
        [SerializeField] private SeraphPillarSpawnData spawnData;

        public void Initialize(BaseSeraph _baseSeraph, SeraphCombat _combat)
        {
            baseSeraph = _baseSeraph;
            combat = _combat;

            projectilePool = new(data.PillarPrefab);

            Setup();
            nextCastTime = Time.time + 1f;
        }

        public void Dispose()
        {
            baseSeraph = null;

            projectilePool.Dispose();
            projectilePool = null;
        }

        public void PhysicsUpdate()
        {
            if (Time.time < nextCastTime) return;

            CastPillars();

            nextCastTime = Time.time + spawnData.Cooldown;
        }

        private void Setup()
        {
            pillarAmount = spawnData.Amount;
        }

        private void CastPillars()
        {
            List<Vector3> destinationPositions = new();
            List<Vector3> spawnPositions = new();

            SpawnPositions(spawnData.Amount, ref spawnPositions, ref destinationPositions);

            for (int i = 0; i != destinationPositions.Count; i++)
            {
                var pillar = projectilePool.Rent(spawnPositions[i], UnityEngine.Quaternion.Euler(Vector3.down)) as AngelPillar;
                pillar.Initialize(spawnData.PillarData, destinationPositions[i], baseSeraph);
                spawnData.PillarData.LaunchDelay += spawnData.LauchDelayIncreasement;
            }

            spawnData.PillarData.LaunchDelay = spawnData.LaunchDelayInitialize;
        }

        private void SpawnPositions(int amount, ref List<Vector3> positions, ref List<Vector3> destination)
        {
            Vector3 distanceUnitVector = (baseSeraph.Player.transform.position - baseSeraph.transform.position).normalized;
            Vector3 perpendicularUnitVector = Vector3.Cross(distanceUnitVector, Vector3.up).normalized;
            Vector3 interval = distanceUnitVector * spawnData.DistanceInteval;
            Vector3 startPosition;
            Vector3 currentDestinationPosition = baseSeraph.Player.transform.position - ((spawnData.CenterOffset - 1) * interval);
            int a = 1;
            for (int i = 0; i != amount; i++)
            {
                startPosition = baseSeraph.transform.position + (distanceUnitVector * spawnData.InitialVerticalInterval * (1 + i / 2)) + (perpendicularUnitVector * spawnData.InitialHorizontalInterval * a * (1 + i / 2));
                positions.Add(startPosition);
                destination.Add(new Vector3(currentDestinationPosition.x, currentDestinationPosition.y, currentDestinationPosition.z));

                currentDestinationPosition += interval;
                a *= -1;
            }
        }
    }

    [Serializable]
    public class SeraphPillarSkillData
    {
        public AngelPillar PillarPrefab;
    }

    [Serializable]
    public class SeraphPillarSpawnData
    {
        public AngelPillarData PillarData;
        public float Radius = 30f;
        public int Amount = 4;
        public float Interval = 0.05f;
        public float DistanceInteval = 12f;

        public float InitialVerticalInterval = 4f;
        public float InitialHorizontalInterval = 7f;

        public float CenterOffset = 3;
        public float Cooldown = 10f;
        public float GlobalCooldown = 2.5f;

        public float LaunchDelayInitialize = 2f;
        public float LauchDelayIncreasement = 0.3f;
    }
}