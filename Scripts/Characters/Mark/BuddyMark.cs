using System;
using Magia.Enemy;
using Magia.GameLogic;
using Magia.UI;
using Magia.UI.Gameplay;
using UnityEngine;

namespace Magia.Mark
{
    public class BuddyMark : MonoBehaviour
    {
        public event Action<ElementType> EventResolveEffectOnPlayer;
        public event Action<BaseEnemy, ElementType> EventResolveEffectOnEnemy;
        public event Action<BuddyMark> EventDispose;

        [SerializeField] private ParticleSystem onHitVfx;

        private BaseEnemy enemy;
        private BuddyMarkData data;
        private UIMarkIndicator indicator;

        private int stackCollect;

        public MarkPoolManager PoolManager;

        public void Initialize(BuddyMarkData _data, BaseEnemy _enemy)
        {
            data = _data;
            enemy = _enemy;
            stackCollect = 0;

            enemy.EventTakeDamage += OnDamageTake;
            enemy.EventDefeated += OnDefeated;

            indicator = UIManager.Instance.DisplayMarkIndicator(_enemy.transform);
        }

        public void Dispose()
        {
            enemy = null;

            if (indicator != null)
            {
                indicator.Dispose();
                indicator = null;
            }

            if (PoolManager != null)
            {
                PoolManager.Return(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void ResolveEffect()
        {
            enemy.EventTakeDamage -= OnDamageTake;
            enemy.EventDefeated -= OnDefeated;
            enemy.RemoveMark();

            indicator.Pop();
            indicator = null;

            EventResolveEffectOnPlayer?.Invoke(data.Element);
            EventResolveEffectOnEnemy?.Invoke(enemy, data.Element);
            EventDispose?.Invoke(this);

            Dispose();
        }

        #region subscribe events
        private void OnDamageTake(Damage damage)
        {
            if (damage.Element == ElementType.Magia)
            {
                stackCollect = data.StackLimit - 1;
            }
            else
            {
                stackCollect += 1;
            }

            onHitVfx.Play();
            indicator.UpdateStack(stackCollect, data.StackLimit);

            if (stackCollect < data.StackLimit) return;

            ResolveEffect();
        }

        private void OnDefeated(BaseEnemy baseEnemy)
        {
            ResolveEffect();
        }
        #endregion
    }

    [Serializable]
    public class BuddyMarkData
    {
        public ElementType Element;
        public int StackLimit = 4;
    }
}
