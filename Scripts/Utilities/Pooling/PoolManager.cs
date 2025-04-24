using System.Collections.Generic;
using UnityEngine;

namespace Magia.Utilities.Pooling
{
    public class PoolManager<T> where T : PoolObject<T>
    {
        private static PoolManager<T> instance;
        public static PoolManager<T> Instance => instance ?? (instance = new PoolManager<T>());

        private readonly Queue<T> pool = new Queue<T>();

        public void Initialize(T prefab, int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
            {
                T obj = Object.Instantiate(prefab);
                obj.isPoolInstance = true;
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T Rent(T prefab)
        {
            if (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                T obj = Object.Instantiate(prefab);
                obj.isPoolInstance = true;
                return obj;
            }
        }

        public void Return(T obj)
        {
            pool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
    }
}
