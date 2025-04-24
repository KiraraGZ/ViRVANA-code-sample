using System;
using System.Collections;
using UnityEngine;

namespace Magia.Utilities.Pooling
{
    public class PoolObject<T> : MonoBehaviour where T : PoolObject<T>
    {
        private PoolManager<T> manager;
        public bool isPoolInstance;

        public T Rent()
        {
            if (isPoolInstance)
            {
                throw new Exception("Not instantiating from prefab");
            }

            return GetManager().Rent(this as T);
        }

        public T Rent(Vector3 position, Quaternion rotation)
        {
            T val = Rent();
            val.transform.SetPositionAndRotation(position, rotation);

            return val;
        }

        public T Rent(Vector3 position, Quaternion rotation, Transform parent)
        {
            T val = Rent();
            val.transform.SetParent(parent);
            val.transform.SetLocalPositionAndRotation(position, rotation);

            return val;
        }

        public T Rent(Transform parent)
        {
            T val = Rent();
            val.transform.SetParent(parent);

            return val;
        }

        public void Return()
        {
            GetManager().Return((T)this);
        }

        public void Return(float time)
        {
            StartCoroutine(StartLifetime(time));
        }

        private PoolManager<T> GetManager()
        {
            if (manager == null)
            {
                manager = PoolManager<T>.Instance;
            }

            return manager;
        }

        IEnumerator StartLifetime(float time)
        {
            yield return new WaitForSeconds(time);
            Return();
        }
    }
}
