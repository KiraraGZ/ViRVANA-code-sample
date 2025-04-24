using System;
using System.Collections.Generic;
using Magia.Enemy;
using Magia.GameLogic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Magia.Mark
{
    public class MarkPoolManager
    {
        public event Action<BaseEnemy, ElementType> EventResolveEffectOnEnemy;

        private BuddyMark markPrefab;
        private Queue<BuddyMark> markQueue;
        private List<BuddyMark> marksInScene;

        public MarkPoolManager(BuddyMark mark)
        {
            Initialize(mark);
        }

        public void Initialize(BuddyMark mark)
        {
            markPrefab = mark;
            markQueue = new();
            marksInScene = new();
        }

        public void Dispose()
        {
            for (int i = 0; i < marksInScene.Count; i++)
            {
                marksInScene[i].EventResolveEffectOnEnemy -= OnResolveEffectOnEnemy;
                Object.Destroy(marksInScene[i].gameObject);
            }

            markPrefab = null;
            markQueue = null;
            marksInScene = null;
        }

        public BuddyMark Rent(Transform transform)
        {
            var mark = Rent(transform.position, Quaternion.Euler(transform.forward));
            mark.transform.parent = transform;

            return mark;
        }

        public BuddyMark Rent(Vector3 position, Quaternion rotation)
        {
            var mark = Rent();
            mark.transform.SetPositionAndRotation(position, rotation);

            return mark;
        }

        public BuddyMark Rent()
        {
            if (markQueue.Count == 0)
            {
                var mark = Object.Instantiate(markPrefab);
                mark.PoolManager = this;
                mark.EventResolveEffectOnEnemy += OnResolveEffectOnEnemy;
                marksInScene.Add(mark);

                return mark;
            }
            else
            {
                var mark = markQueue.Dequeue();
                mark.gameObject.SetActive(true);

                return mark;
            }
        }

        public void Return(BuddyMark mark)
        {
            mark.gameObject.SetActive(false);

            if (markQueue == null) return;

            markQueue.Enqueue(mark);
        }

        #region subscribe events
        private void OnResolveEffectOnEnemy(BaseEnemy enemy, ElementType element)
        {
            EventResolveEffectOnEnemy?.Invoke(enemy, element);
        }
        #endregion
    }
}
