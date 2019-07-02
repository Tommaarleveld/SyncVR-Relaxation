using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using SyncVR.Util;

namespace SyncVR.Analytics
{
    public class AnalyticsService : MonoBehaviour
    {
        public static AnalyticsService Instance { get; private set; }
        public string windowsLogFilePath = "";

        private string logDirectory = "";
        private string logfilePath = "";
        private bool continueUpload = true;

        private const string uploadUrl = "https://www.googleapis.com/upload/storage/v1/b/syncvr-analytics/o";

        public enum EventType
        {
            AppStart,
            AppGetFocus,
            AppLoseFocus,
            VideoStart,
            VideoStop,
            VideoEnd,
            SceneLoad,
            Error
        }

        public void Awake()
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

        public void Start()
        {
            logfilePath = GetFilePath();
            SceneManager.sceneLoaded += OnSceneLoaded;
            LogEvent(EventType.AppStart, new Dictionary<string, object>());

            //TODO: start uploading of files
            StartCoroutine(UploadAnalyticsFiles());
        }

        public void LogEvent(EventType eventType, Dictionary<string, object> data)
        {
            data.Add("event_type", eventType.ToString());
            LogEvent(data);
        }

        public void LogEvent(string eventType, Dictionary<string, object> data)
        {
            data.Add("event_type", eventType);
            LogEvent(data);
        }

        private void LogEvent (Dictionary<string, object> data)
        {
            data.Add("time", DateTime.Now.ToString("s"));
            data.Add("battery", SystemInfo.batteryLevel);

            if (Debug.isDebugBuild || logfilePath == "")
            {
                Debug.Log(JsonConvert.SerializeObject(data));
            }
            else
            {
                if (logfilePath != "")
                {
                    File.AppendAllText(logfilePath, JsonConvert.SerializeObject(data) + "\n");
                }
            }
        }

        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                LogEvent(EventType.AppLoseFocus, new Dictionary<string, object>());
            }
            else
            {
                LogEvent(EventType.AppGetFocus, new Dictionary<string, object>());
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LogEvent(EventType.SceneLoad, new Dictionary<string, object>() { { "scene_name", scene.name } });
        }

        private string GetFilePath ()
        {
            return Path.Combine(GetLogDirectory(), DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss", DateTimeFormatInfo.InvariantInfo) + ".log");
        }

        private string GetLogDirectory ()
        {
            if (logDirectory == "")
            {
#if UNITY_ANDROID
                logDirectory = AndroidLogFilePath();
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                logDirectory = WindowsLogFilePath();
#else
                logDirectory = "";
#endif
                ExistOrCreateDirectory(logDirectory);
            }
            return logDirectory;
        }

        private string AndroidLogFilePath ()
        {
            string path = Path.Combine(SystemInfoUtil.StoragePath(), Path.Combine("SyncVR", Path.Combine("Logs", SystemInfoUtil.PackageName()))); ;
            ExistOrCreateDirectory(path);
            return path;
        }

        private string WindowsLogFilePath ()
        {
            return windowsLogFilePath;
        }

        private void ExistOrCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private IEnumerator UploadAnalyticsFiles ()
        {
            DirectoryInfo dir = new DirectoryInfo(GetLogDirectory());
            FileInfo[] files = dir.GetFiles("*.log");

            foreach (FileInfo f in files)
            {
                if (f.Name != Path.GetFileName(logfilePath))
                {
                    yield return StartCoroutine(UploadFile(f));
                    if (!continueUpload)
                    {
                        break;
                    }
                }
            }
        }

        private IEnumerator UploadFile (FileInfo file)
        {
            WWWForm form = new WWWForm();
            form.AddField("uploadType", "media");
            form.AddField("name", SystemInfoUtil.PackageName() + "/" + SystemInfoUtil.DeviceID() + "/" + file.Name);

            bool delete = false;

            using (UnityWebRequest request = new UnityWebRequest(uploadUrl + "?" + Encoding.UTF8.GetString(form.data)))
            {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/json");

                // set uploadHandler
                UploadHandlerFile uploadHandler = new UploadHandlerFile(file.FullName);
                request.uploadHandler = uploadHandler;

                yield return request.SendWebRequest();

                if (request.isHttpError)
                {
                    LogEvent(EventType.Error, new Dictionary<string, object> { { "msg", "Http error uploading analytics file: " + request.responseCode } });
                }
                else if (request.isNetworkError)
                {
                    continueUpload = false;
                    LogEvent(EventType.Error, new Dictionary<string, object> { { "msg", "Network error uploading analytics file!" } });
                }
                else
                {
                    delete = true;
                    LogEvent("Analytics", new Dictionary<string, object> { { "msg", "Uploaded file: " + file.Name } });
                }
            }

            // wait 1 frame for request to have been cleaned up and released the file
            yield return null;
            
            if (delete)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    LogEvent(EventType.Error, new Dictionary<string, object> { { "msg", "Deleting analytics file: " + file.Name + " failed: " + e.Message } });
                }
            }
        }
    }
}