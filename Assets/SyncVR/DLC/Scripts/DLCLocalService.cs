using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SyncVR.Util;

namespace SyncVR.DLC
{
    public class DLCLocalService : MonoBehaviour
    {
        public static DLCLocalService Instance { get; private set; }
        public List<DLCBundle> localBundles { get; private set; }

        public string windowsDLCPath;
        private string dlc_path = "";
        private const string dlc_list_filename = "assetbundles.json";
        private const string to_download_key_prefix = "download-bundle-";

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

        public List<DLCBundle> GetBundlesByCategory (string cat)
        {
            return localBundles.Where(x => x.category == cat).ToList();
        }

        public List<string> GetBundlesCategories ()
        {
            return localBundles.Select(x => x.category).Distinct().ToList();
        }
        
        public void ReadDLCList (bool forceReload = false)
        {
            if (localBundles != null && !forceReload)
            {
                return;
            }

            localBundles = new List<DLCBundle>();

            // glob directories: these are the different categories
            DirectoryInfo[] categories = new DirectoryInfo(GetDLCPath()).GetDirectories();

            // per directory find all files: these are the assets
            foreach (DirectoryInfo dir in categories)
            {
                FileInfo[] files = dir.GetFiles("*.unity3d");

                // construct an asset object for each file and add it to the list
                foreach (FileInfo file in files)
                {
                    DLCBundle bundle = BundleFromFile(file, dir);

                    if (!PlayerPrefs.HasKey(GetDownloadKey(bundle)))
                    {
                        localBundles.Add(bundle);
                    }
                    else
                    {
                        DLCManager.Instance.CleanUpDLCBundle(bundle);
                    }
                }
            }
        }

        public string GetDLCPath ()
        {
            if (dlc_path != "")
            {
                return dlc_path;
            }

#if UNITY_ANDROID
            dlc_path = AndroidDLCPath();
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            dlc_path =  windowsDLCPath;
#endif

            ExistOrCreateDirectory(dlc_path);
            return dlc_path;
        }

        public string GetDownloadKey (DLCBundle bundle)
        {
            return to_download_key_prefix + bundle.category + "-" + bundle.name;
        }

        private DLCBundle BundleFromFile (FileInfo file, DirectoryInfo dir) 
        {
            DLCBundle bundle = new DLCBundle();
            bundle.name = file.Name;
            bundle.category = dir.Name;
            bundle.path = file.FullName;
            bundle.has_dll = File.Exists(file.FullName.Replace(".unity3d", ".dll"));
            bundle.byteSize = file.Length;

            return bundle;
        } 

        private string AndroidDLCPath ()
        {
            return Path.Combine(SystemInfoUtil.StoragePath(), Path.Combine("SyncVR", "DLC"));
        }

        private void ExistOrCreateDirectory (string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

}
