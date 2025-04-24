using System;
using System.Collections.Generic;
using Magia.GameLogic;
using Magia.Mark;
using Magia.Player;
using Magia.UI.Gameplay;
using UnityEngine;

namespace Magia.Enemy
{
    public class BaseEnemy : MonoBehaviour, IDamageable
    {
        public event Action EventReady;
        public event Action<ProgressBarInfo> EventHealthUpdate;
        public event Action<Damage> EventTakeDamage;
        public event Action<BaseEnemy> EventDefeated;

        [Header("Setup")]
        [SerializeField] protected EnemyHitboxSetting[] hitboxDatas;

        [Header("Read Only")]
        [SerializeField] protected int health;
        public int Health => health;
        [SerializeField] protected int maxHealth;

        protected bool isReady;

        public PlayerController Player { get; private set; }
        [SerializeField] protected Rigidbody rb;
        public Rigidbody Rigidbody => rb;

        public Queue<BaseEnemy> PoolQueue;

        public BuddyMark Mark { get; private set; }

        public virtual void Initialize(PlayerController player)
        {
            Player = player;

            for (int i = 0; i < hitboxDatas.Length; i++)
            {
                if (hitboxDatas == null || hitboxDatas.Length <= i)
                {
                    hitboxDatas[i].Hitbox.Initialize(new(), this);
                    continue;
                }

                hitboxDatas[i].Hitbox.Initialize(hitboxDatas[i].Data, this);
            }

            isReady = false;
        }

        public virtual void Dispose()
        {
            Player = null;

            if (PoolQueue != null)
            {
                PoolQueue.Enqueue(this);
                PoolQueue = null;
            }

            if (Mark != null)
            {
                Mark.Dispose();
                Mark = null;
            }

            gameObject.SetActive(false);
        }

        public virtual DamageFeedback TakeDamage(Damage damage, Vector3 hitPoint, Vector3 hitDirection, IDamageable owner)
        {
            if (this as IDamageable == owner)
            {
                return new DamageFeedback(false);
            }

            if (health <= 0)
            {
                return new DamageFeedback(damage, -1);
            }

            health -= damage.Amount;

            EventTakeDamage?.Invoke(damage);
            OnHealthUpdate(health, maxHealth);

            if (health <= 0)
            {
                OnDefeated();
            }

            return new DamageFeedback(damage, 1);
        }

        public void ApplyMark(BuddyMark mark)
        {
            Mark = mark;
        }

        public void RemoveMark()
        {
            Mark = null;
        }

        #region invoke methods
        protected virtual void OnReady()
        {
            isReady = true;
            EventReady?.Invoke();
        }

        protected virtual void OnDefeated()
        {
            EventDefeated?.Invoke(this);
        }

        protected virtual void OnHealthUpdate(int health, int maxHealth, bool barrier = false)
        {
            EventHealthUpdate?.Invoke(new ProgressBarInfo(health, maxHealth, barrier));
        }
        #endregion
    }

    [Serializable]
    public class EnemyHitboxSetting
    {
        public EnemyHitbox Hitbox;
        public EnemyHitboxData Data;
    }
}
