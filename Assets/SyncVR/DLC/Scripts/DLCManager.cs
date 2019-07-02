using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using SyncVR.Util;
using SyncVR.Analytics;

namespace SyncVR.DLC
{
    public class DLCManager : MonoBehaviour
    {
        public static DLCManager Instance { get; private set; }
        public List<DLCBundle> allocatedBundles { get; private set; }

        private const string hostPath = "https://storage.googleapis.com/syncvr-dlc/";

        public List<DLCBundle> toDelete { get; private set; }
        public List<DLCBundle> toDownload { get; private set; }
        public bool allocationsRetrieved { get; private set; }
        public bool diffCalculated { get; private set; }

        private List<UnityWebRequest> requiresUnpacking = new List<UnityWebRequest>();

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

        public void Start ()
        {
            allocationsRetrieved = false;
            diffCalculated = false;
        }

        public void RetrieveBundleAllocations ()
        {
            allocationsRetrieved = false;
            allocatedBundles = null;
            StartCoroutine(RetrieveBundleAllocationsRoutine());
        }

        public void CalculateBundleDiff ()
        {
            diffCalculated = false;
            toDownload = new List<DLCBundle>();
            toDelete = new List<DLCBundle>();
            DLCLocalService.Instance.ReadDLCList();
            StartCoroutine(CalculateBundleDiffRoutine());
        }

        private IEnumerator RetrieveBundleAllocationsRoutine ()
        {
            using (UnityWebRequest www = new UnityWebRequest(hostPath + "devices/" + SystemInfoUtil.DeviceID() + ".json"))
            {
                www.method = "GET";
                www.downloadHandler = new DownloadHandlerBuffer();

                yield return www.SendWebRequest();

                if (www.isHttpError)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "Http error retrieving bundle allocations: " + www.responseCode} });
                }
                else if (www.isNetworkError)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "Network error retrieving bundle allocations!" } });
                }
                else
                {
                    try
                    {
                        allocatedBundles = JsonConvert.DeserializeObject<List<DLCBundle>>(www.downloadHandler.text);
                    }
                    catch
                    {
                        AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "Error reading json response for bundle allocations!" } });
                    }
                }
            }
            allocationsRetrieved = true;
        }

        private IEnumerator CalculateBundleDiffRoutine ()
        {
            while (!allocationsRetrieved)
            {
                yield return null;
            }

            if (allocatedBundles != null)
            {
                foreach(DLCBundle bundle in allocatedBundles)
                { 
                    if (!DLCLocalService.Instance.localBundles.Contains(bundle))
                    {
                        yield return StartCoroutine(GetRemoteBundleSize(bundle));
                        toDownload.Add(bundle);
                    }
                }

                DLCLocalService.Instance.localBundles.ForEach(x =>
                {
                    if (!allocatedBundles.Contains(x))
                    {
                        toDelete.Add(x);
                    }
                });
            }

            diffCalculated = true;
        }

        private IEnumerator GetRemoteBundleSize (DLCBundle bundle)
        {
            UnityWebRequest r = HeadBundleRequest(bundle);
            yield return r.SendWebRequest();
            if (!r.isHttpError && !r.isNetworkError)
            {
                bundle.byteSize = int.Parse(r.GetResponseHeader("Content-Length"));
            }
        }

        public UnityWebRequest DownloadBundleRequest (DLCBundle bundle)
        {
            string path = hostPath + SystemInfoUtil.PlatformName() + "/" + bundle.category + "/" + bundle.name;
            UnityWebRequest www = UnityWebRequest.Get(path);

            bundle.path = Path.Combine(DLCLocalService.Instance.GetDLCPath(), Path.Combine(bundle.category, bundle.name));
            DownloadHandlerFile handler = new DownloadHandlerFile(bundle.path);
            handler.removeFileOnAbort = true;
            www.downloadHandler = handler;

            if (bundle.IsZip())
            {
                requiresUnpacking.Add(www);
            }

            return www;
        }

        public IEnumerator FinalizeDownloadRequest (UnityWebRequest request, DLCBundle bundle)
        {
            if (requiresUnpacking.Contains(request) && !(request.isHttpError|| request.isNetworkError)) 
            {
                // construct the parameters to decompress file
                DecompressZipfileParams p = new DecompressZipfileParams(bundle.path, bundle.category);
                // create a new thread so we dont block the main thread while we decompress
                Thread t = new Thread(DecompressZipfile);
                // start the work
                t.Start(p);

                // wait till the thread indicates it's ready
                yield return new WaitUntil(() => p.isFinished);
                // join the thread
                t.Join();

                // remove the request from the requiresUnpacking list
                requiresUnpacking.Remove(request);
                // delete the zip file we downloaded earlier
                DeleteFile(bundle.path);
            }

            request.Dispose();
        }

        public UnityWebRequest HeadBundleRequest (DLCBundle bundle)
        {
            string path = hostPath + SystemInfoUtil.PlatformName() + "/" + bundle.category + "/" + bundle.name;
            UnityWebRequest www = UnityWebRequest.Head(path);
            return www;
        }

        public void DeleteBundle (DLCBundle bundle)
        {
            AnalyticsService.Instance.LogEvent("dlc_manager", new Dictionary<string, object> { { "msg", "Deleting bundle: " + bundle.ToString() } });
            DeleteFile(bundle.path);
            DeleteFile(bundle.DLLPath());
        }

        public void CleanUpDLCBundle (DLCBundle bundle)
        {
            AnalyticsService.Instance.LogEvent("dlc_manager", new Dictionary<string, object> { { "msg", "Cleaning Up bundle: " + bundle.ToString() } });
            DeleteFile(bundle.path);
            DeleteFile(bundle.DLLPath());
            DeleteFile(bundle.ZipPath());
        }
         
        private void DeleteFile (string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "Deleting file " + path + " failed: " + e.Message } });
                }
            }
        }

        private class DecompressZipfileParams
        {
            public string compressedFile;
            public string bundleCategory;
            public bool isFinished;

            public DecompressZipfileParams (string file, string cat)
            {
                compressedFile = file;
                bundleCategory = cat;
                isFinished = false;
            }
        }

        private void DecompressZipfile (object data)
        {
            DecompressZipfileParams p = (DecompressZipfileParams)data;
            string targetDir = Path.Combine(DLCLocalService.Instance.GetDLCPath(), p.bundleCategory);

            ZipFile.ExtractToDirectory(p.compressedFile, targetDir);
            p.isFinished = true;
        } 
    }
}
