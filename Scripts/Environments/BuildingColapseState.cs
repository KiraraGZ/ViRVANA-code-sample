using System;
using UnityEngine;

namespace Magia.Environment
{
    public class BuildingColapseState
    {
        public event Action<int> EventColapseComplete;

        private Transform[] buildings;
        private BoxCollider[] colliders;
        private int colapsedFloor;
        private int currentFloor;

        public void Initialize(Transform[] buildings, BoxCollider[] colliders, int colapsedFloor)
        {
            this.buildings = buildings;
            this.colliders = colliders;
            this.colapsedFloor = colapsedFloor;
            currentFloor = colapsedFloor + 1;
        }

        public void Dispose()
        {
            buildings = null;
            colliders = null;
        }

        public void PhysicsUpdate()
        {
            for (int i = buildings.Length - 1; i >= currentFloor; i--)
            {
                if (buildings[i] == null) continue;

                buildings[i].position += 9.8f * Time.deltaTime * Vector3.down;

                if (buildings[i].localPosition.y >= buildings[colapsedFloor].localPosition.y) continue;

                GameObject.Destroy(buildings[i].gameObject);
                currentFloor++;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].center += 9.8f * Time.deltaTime * Vector3.down;

                if (colliders[i].center.y + colliders[i].size.y / 2 >= 0) continue;

                colliders[i].enabled = false;
            }

            if (currentFloor < buildings.Length) return;

            EventColapseComplete?.Invoke(buildings.Length);
        }
    }
}
