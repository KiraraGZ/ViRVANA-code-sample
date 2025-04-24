using UnityEngine;

namespace Magia.Buddy.Data
{
    [CreateAssetMenu(fileName = "Buddy", menuName = "ScriptableObject/Buddy")]
    public class BuddySO : ScriptableObject
    {
        public BuddyMovementData MovementData;
        public BuddyCombatData CombatData;
    }
}