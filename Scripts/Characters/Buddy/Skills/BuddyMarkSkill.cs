using System;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.Mark;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Magia.Buddy
{
    [Serializable]
    public class BuddyMarkSkill
    {
        [SerializeField] private BuddyMark markPrefab;
        [SerializeField] private BuddyMarkSkillData data;
        private BuddyMark currentMark;
        private float lastMarkTime;

        private BuddyController buddy;

        public void Initialize(BuddyController _buddy)
        {
            buddy = _buddy;

            lastMarkTime = Time.time;
        }

        public void Dispose()
        {
            buddy = null;
        }

        public bool IsAvailable()
        {
            if (currentMark != null)
            {
                lastMarkTime = Time.time;
                return false;
            }

            return Time.time > lastMarkTime + data.Cooldown;
        }

        public void MarkEnemy(BaseEnemy target)
        {
            //TODO: fix this to use new mark pool manager.
            // currentMark = markPrefab.Rent(target.transform.position, target.transform.rotation, target.transform);
            currentMark.Initialize(data.MarkData, target);
            PickRandomMark(currentMark);
            currentMark.EventDispose += ReturnMark;

            lastMarkTime = Time.time;
        }

        private void PickRandomMark(BuddyMark currentMark)
        {
            int rand = Random.Range(0, 2);

            switch (rand)
            {
                case 0:
                    currentMark.EventResolveEffectOnPlayer += RecoverEnergy;
                    break;
                case 1:
                    currentMark.EventResolveEffectOnEnemy += Explode;
                    break;
                default:
                    currentMark.EventResolveEffectOnPlayer += RecoverEnergy;
                    break;
            }

        }

        #region subscribe events
        //TODO: old gameplay design, need to be updated
        private void RecoverEnergy(ElementType element)
        {
            if (buddy == null) return;

            // buddy.Player.SkillHandler.RecoverEnergy(20);
        }

        private void Explode(BaseEnemy enemy, ElementType element)
        {
            enemy.TakeDamage(new(50, element, DamageType.Effect), enemy.transform.position, -enemy.transform.forward, buddy);
        }

        private void ReturnMark(BuddyMark mark)
        {
            currentMark.EventResolveEffectOnPlayer -= RecoverEnergy;
            currentMark.EventDispose -= ReturnMark;
            currentMark = null;
        }
        #endregion
    }

    [Serializable]
    public class BuddyMarkSkillData
    {
        public float Cooldown = 6f;
        public BuddyMarkData MarkData;
    }
}
