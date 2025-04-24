using UnityEngine;

namespace Magia.Enemy
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/StageEnemySetting")]
    public class EnemySO : ScriptableObject
    {
        public BaseEnemy Prefab;
        public string Name;
        [TextArea] public string Description;
        public Sprite Icon;
    }
}
