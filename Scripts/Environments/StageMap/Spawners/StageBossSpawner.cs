using Magia.Enemy;
using Magia.Player;
using Magia.UI;
using UnityEngine;

namespace Magia.Environment
{
    public class StageBossSpawner : StageMapSpawner
    {
        [SerializeField] private Vector3 rotation;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);

            foreach (var enemy in enemies)
            {
                UIManager.Instance.CreateBossIndicator(enemy.transform);
            }
        }

        public BaseEnemy SpawnBoss()
        {
            var boss = SpawnEnemy(transform.position, Quaternion.Euler(rotation));
            return boss;
        }

        protected override void OnEnemyDefeated(BaseEnemy enemy)
        {
            base.OnEnemyDefeated(enemy);
            UIManager.Instance.RemoveIndicator(enemy.transform);
        }
    }
}
