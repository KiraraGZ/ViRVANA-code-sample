using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Magia.Environment
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private GameObject titleScene;
        [SerializeField] private Transform titleSceneCamera;
        public Transform TitleSceneCamera => titleSceneCamera;

        [SerializeField] private GameObject fog;

        private AsyncOperationHandle<GameObject> handle;
        private GameObject gameplayScene;
        private StageMap stageMap;
        public StageMap StageMap => stageMap;

        //TODO: Improvement is needed
        public void LoadTitleScene()
        {
            titleScene.SetActive(true);
        }

        public void UnloadTitleScene()
        {
            titleScene.SetActive(false);
        }

        public void LoadGameplayScene(string key, Action callback)
        {
            Load(key, callback);

            fog.SetActive(true);
        }

        public void UnloadScene()
        {
            Destroy(gameplayScene);
            Addressables.ReleaseInstance(gameplayScene);
            gameplayScene = null;

            fog.SetActive(false);
        }

        #region private methods
        private void Load(string key, Action callback = null)
        {
            StartCoroutine(LoadAddressableAsset(key, callback));
        }

        private IEnumerator LoadAddressableAsset(string key, Action callback = null)
        {
            handle = Addressables.LoadAssetAsync<GameObject>(key);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                gameplayScene = Instantiate(handle.Result, transform);
                stageMap = gameplayScene.GetComponent<StageMap>();
            }

            callback.Invoke();
        }
        #endregion
    }
}
