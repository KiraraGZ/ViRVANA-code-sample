using System.Collections.Generic;
using Magia.Enemy;
using Magia.Enemy.Dummy;
using Magia.Player;
using Magia.Buddy;
using Magia.UI;
using UnityEngine;

namespace Magia.GameLogic
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        public PlayerController Player => player;
        [SerializeField] private BuddyController buddy;
        public BuddyController Buddy => buddy;

        [SerializeField] private Dummy dummyPrefab;

        private class EnemySpawner
        {
            public float NextSpawnTime;
            public EnemySpawnData Data;
            public Queue<BaseEnemy> Queue;

            public EnemySpawner(EnemySpawnData data)
            {
                NextSpawnTime = 0f;
                Data = data;
                Queue = new();
            }
        }

        private List<BaseEnemy> preparedEnemies;
        public List<BaseEnemy> Enemies => preparedEnemies;
        private Dummy dummy;

        private void Start()
        {
            player.gameObject.SetActive(false);
            buddy.gameObject.SetActive(false);
        }

        public void Initialize(StageSettingData setting)
        {
            SetupPlayerAndBuddy(setting);

            preparedEnemies = new();
        }

        public void Dispose()
        {
            player.gameObject.SetActive(false);
            buddy.gameObject.SetActive(false);
            player.Dispose();
            //TODO: uncomment this line after the buddy behavior is fully implemented.
            buddy.Dispose();

            preparedEnemies = null;

            if (dummy != null)
            {
                Destroy(dummy.gameObject);
            }
        }

        #region temp spawn methods
        public void SpawnDummy()
        {
            if (dummy != null)
            {
                Destroy(dummy.gameObject);
            }

            var position = new Vector3(120, 30, 0);
            var rotation = Quaternion.LookRotation(player.transform.position - position);
            dummy = Instantiate(dummyPrefab, position, rotation, transform);
            dummy.Initialize(player);
            preparedEnemies.Add(dummy);
            SetBuddyDestination(position);
            UIManager.Instance.CreateDestinationIndicator(dummy.transform);
        }

        public void StartDummyAttack()
        {
            dummy.StartAttack();
        }

        public void DespawnDummy()
        {
            preparedEnemies.Remove(dummy);
            dummy.Dispose();
            UIManager.Instance.RemoveIndicator(dummy.transform);
        }

        public void SetBuddyDestination(Vector3 destination)
        {
            buddy.SetDestination(destination);
        }
        #endregion

        public void SetPreparedEnemies(List<BaseEnemy> enemies)
        {
            preparedEnemies = enemies;
        }

        #region on initial
        private void SetupPlayerAndBuddy(StageSettingData data)
        {
            player.gameObject.SetActive(true);
            player.transform.position = data.PlayerSpawnPosition;

            //TODO: uncomment this line after the buddy behavior is fully implemented.
            buddy.gameObject.SetActive(true);
            buddy.transform.position = data.PlayerSpawnPosition + Vector3.right * 3;
            buddy.Initialize(player);
        }
        #endregion
    }
}
