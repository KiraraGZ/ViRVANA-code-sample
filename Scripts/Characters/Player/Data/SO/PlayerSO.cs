using Magia.Utilities;
using UnityEngine;

namespace Magia.Player.Data
{
    [CreateAssetMenu(fileName = "Player", menuName = "ScriptableObject/Player")]
    public class PlayerSO : ScriptableObject
    {
        public PlayerStatsData StatsData;

        public PlayerMovementData MovementData;
        public PlayerCombatData CombatData;
        public PlayerUncontrollableData UncontrollableData;
        public CapsuleColliderUtility ColliderUtility;
        public PlayerLayerData LayerData;
    }
}
