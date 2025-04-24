using System;
using Magia.GameLogic;
using Magia.Vfx;
using UnityEditor;
using UnityEngine;

namespace Magia.Environment
{
    [Serializable]
    public class BuildingStructure
    {
        public BuildingFloor[] Floors;
    }

    [Serializable]
    public class BuildingFloor
    {
        public GameObject Base;
        public int Level = 1;
    }

    public class Building : MonoBehaviour, IDamageable
    {
        [SerializeField] private BuildingStructure structure;
        [SerializeField] private Transform buildingContainer;

        [Header("Parameters")]
        [SerializeField] private int damageRequired = 100;
        [SerializeField] private ElementalWeakness weakness;
        [SerializeField] private float explosionMultiplier = 2;

        private int colapsedFloor;
        private ParticleSystem smoke;

        private BuildingColapseState colapseState;

        public DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (damage.Type == DamageType.Projectile || damage.Type == DamageType.Explosion)
            {
                VfxManager.Instance.RentVfx(damage.Element, hitPoint, Quaternion.LookRotation(hitDirection));
            }

            if (colapseState != null) return new(damage, -1);

            float multiplier = damage.Type == DamageType.Explosion ? explosionMultiplier : 1;
            if (weakness.CalculateDamage(damage) * multiplier < damageRequired) return new(damage, -1);

            StartColapse(hitPoint);
            return new(damage, -1);
        }

        private void FixedUpdate()
        {
            colapseState?.PhysicsUpdate();
        }

        private void StartColapse(Vector3 hitPoint)
        {
            var buildings = buildingContainer.GetComponentsInChildren<Transform>();
            var colliders = GetComponents<BoxCollider>();
            var size = structure.Floors[0].Base.GetComponentInChildren<MeshRenderer>().bounds.size;

            colapsedFloor = (int)((hitPoint.y - transform.position.y) / size.y);
            var renderer = buildings[colapsedFloor].GetComponentInChildren<MeshRenderer>();

            if (renderer == null) return;

            colapseState = new();
            colapseState.EventColapseComplete += OnColapseComplete;
            colapseState.Initialize(buildings, colliders, colapsedFloor);

            SpawnSmoke(buildings[colapsedFloor], renderer);
        }

        private void SpawnSmoke(Transform building, MeshRenderer renderer)
        {
            if (smoke != null)
            {
                smoke.Play();
            }
            else
            {
                smoke = Instantiate(EnvironmentController.Instance.SmokePrefab, transform);
            }

            var buildingSize = renderer.bounds.size;
            smoke.transform.SetPositionAndRotation(building.position + buildingSize.y * Vector3.up, building.rotation);
            var smokeShape = smoke.shape;
            smokeShape.scale = new Vector3(buildingSize.x, buildingSize.z, buildingSize.y * 3);
        }

        #region subscribe events
        private void OnColapseComplete(int floor)
        {
            colapseState.Dispose();
            colapseState.EventColapseComplete -= OnColapseComplete;
            colapseState = null;

            smoke.Stop();
        }
        #endregion

#if UNITY_EDITOR
        #region editor tools
        [ContextMenu("Construct")]
        private void Construct()
        {
            Clear();

            float totalHeight = 0;
            int floorNumber = 0;

            foreach (var floor in structure.Floors)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                var mesh = floor.Base.GetComponentInChildren<MeshRenderer>();
                var originalSize = mesh.bounds.size;
                var size = originalSize;
                size.y *= floor.Level;
                col.size = size;

                float bottomY = mesh.bounds.min.y - floor.Base.transform.position.y;
                col.center = new Vector3(mesh.bounds.center.x - floor.Base.transform.position.x,
                                         totalHeight + size.y / 2 - bottomY,
                                         mesh.bounds.center.z - floor.Base.transform.position.z);

                for (int i = 0; i < floor.Level; i++)
                {
                    var position = new Vector3(floor.Base.transform.position.x, totalHeight + originalSize.y * i, floor.Base.transform.position.z);
                    var building = Instantiate(floor.Base, position, Quaternion.identity, buildingContainer);
                    building.name = $"Floor {floorNumber + i + 1} - {floor.Base.name}";
                }

                floorNumber += floor.Level;
                totalHeight += originalSize.y * floor.Level;

                floor.Base.SetActive(false);
            }

            EditorUtility.SetDirty(gameObject);
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            var buildings = buildingContainer.GetComponentsInChildren<Transform>();
            var colliders = GetComponents<BoxCollider>();

            for (int i = 1; i < buildings.Length; i++)
            {
                DestroyImmediate(buildings[i].gameObject);
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                DestroyImmediate(colliders[i]);
            }

            foreach (var floor in structure.Floors)
            {
                floor.Base.SetActive(true);
            }

            EditorUtility.SetDirty(gameObject);
        }
        #endregion
#endif
    }
}
