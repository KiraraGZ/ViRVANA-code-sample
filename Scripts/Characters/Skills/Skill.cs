using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Skills
{
    public abstract class Skill : MonoBehaviour
    {
        public event Action<DamageFeedback, Vector3> EventDamageDealt;

        protected Vector3 GetCameraDirection()
        {
            Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            return ray.origin + ray.direction * 100f - transform.position;
        }

        protected BaseEnemy GetCameraClosestEnemy(float screenRatio = 2.5f)
        {
            //TODO: Try to use Raycast or Collision
            BaseEnemy toReturn = null;
            List<BaseEnemy> enemies = GameplayController.Instance.CharacterManager.Enemies;
            float minDistance = Mathf.Infinity;

            Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);
            float radius = Mathf.Min(Screen.width / screenRatio, Screen.height / screenRatio);

            foreach (BaseEnemy candidate in enemies)
            {
                if (!CheckPosition(screenCenter, radius, candidate.transform.position)) continue;

                float distance = Vector3.Distance(transform.position, candidate.transform.position);

                if (distance >= minDistance) continue;

                minDistance = distance;
                toReturn = candidate;
            }

            return toReturn;
        }

        protected BaseEnemy[] GetCameraClosestEnemies(int amount, float screenRatio = 2.5f)
        {
            List<BaseEnemy> enemies = GameplayController.Instance.CharacterManager.Enemies;

            List<(BaseEnemy enemy, float distance)> candidates = new();
            Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);
            float radius = Mathf.Min(Screen.width / screenRatio, Screen.height / screenRatio);

            foreach (BaseEnemy candidate in enemies)
            {
                if (candidate == null) continue;
                if (!candidate.gameObject.activeInHierarchy) continue;
                if (!CheckPosition(screenCenter, radius, candidate.transform.position)) continue;

                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                Vector3 direction = candidate.transform.position - ray.origin;
                float distance = direction.magnitude;

                if (Physics.Raycast(ray.origin, direction.normalized, out RaycastHit hit, distance))
                {
                    if (!hit.collider.TryGetComponent<EnemyHitbox>(out var hitEnemy)) continue;
                }

                candidates.Add((candidate, distance));
            }

            candidates.Sort((a, b) => a.distance.CompareTo(b.distance));

            int count = Mathf.Min(amount, candidates.Count);
            BaseEnemy[] toReturn = new BaseEnemy[count];

            for (int i = 0; i < count; i++)
            {
                toReturn[i] = candidates[i].enemy;
            }

            return toReturn;
        }

        private bool CheckPosition(Vector2 screenCenter, float radius, Vector3 pos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            if (screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height) return false;

            float screenDistance = Vector2.Distance(screenCenter, screenPos);
            if (screenDistance > radius) return false;

            return true;
        }

        #region subscribe events
        protected virtual void OnDamageDealt(DamageFeedback feedback, Vector3 hitPos)
        {
            EventDamageDealt?.Invoke(feedback, hitPos);
        }
        #endregion
    }
}
