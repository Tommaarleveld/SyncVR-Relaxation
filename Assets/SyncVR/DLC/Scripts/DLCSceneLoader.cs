using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SyncVR.Presence;
using SyncVR.Analytics;
using SyncVR.Util;

namespace SyncVR.DLC
{
    public class DLCSceneLoader : MonoBehaviour
    {
        public static DLCSceneLoader Instance { get; private set; }
        public GameObject exitButtonPrefab;

        private string sourceScene;
        private AssetBundle loadedBundle;

        private List<string> loadedAssemblies = new List<string>();

        public void Awake ()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        public void LoadDLCScene (DLCBundle bundle, Action<string> errorCallback)
        {
            sourceScene = SceneManager.GetActiveScene().name;
            StartCoroutine(LoadSceneFromBundle(bundle, errorCallback));
        }

        private bool LoadDLL (DLCBundle bundle)
        {
            if (bundle.has_dll)
            {
                if (!loadedAssemblies.Contains(bundle.path))
                {
                    try
                    {
                        Assembly.LoadFrom(bundle.path.Replace(".unity3d", ".dll"));
                        loadedAssemblies.Add(bundle.path);
                        return true;
                    }
                    catch (Exception e)
                    {
                        AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "load-assembly", e.Message } });
                        return false;
                    }
                }
            }

            return true;
        }

        private IEnumerator LoadSceneFromBundle (DLCBundle bundle, Action<string> errorCallback)
        {
            if (!LoadDLL(bundle))
            {
                errorCallback("error: couldn't load assembly");
                yield break;
            }

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundle.path);
            yield return request;

            if (request.assetBundle == null)
            {
                errorCallback("error: could't read assetbundle");
                yield break;
            }

            loadedBundle = request.assetBundle;
            string[] scenePaths = loadedBundle.GetAllScenePaths();
            if (scenePaths.Length == 0)
            {
                errorCallback("error: no scenes in assetbundle");
                loadedBundle.Unload(true);
                yield break;
            }

            AnalyticsService.Instance.LogEvent("scene_load", new Dictionary<string, object> { { "scene_name", scenePaths[0] } });
            AsyncOperation async = SceneManager.LoadSceneAsync(scenePaths[0], LoadSceneMode.Single);

            SceneManager.sceneLoaded += OnExternalSceneLoaded;
        }

        public void OnExternalSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject exitButton = Instantiate<GameObject>(exitButtonPrefab);
            exitButton.GetComponentInChildren<Button>().onClick.AddListener(() => LoadSourceScene());
            PlayerRig playerRig = FindObjectOfType<PlayerRig>();
            exitButton.transform.SetParent(playerRig.transform, false);
        }

        public void OnSourceSceneLoaded (Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSourceSceneLoaded;
            loadedBundle.Unload(true);
        }

        public void LoadSourceScene ()
        {
            SceneManager.sceneLoaded -= OnExternalSceneLoaded;
            SceneManager.sceneLoaded += OnSourceSceneLoaded;
            SceneManager.LoadScene(sourceScene);
        }
    }
}