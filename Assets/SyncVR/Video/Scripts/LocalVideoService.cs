using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Newtonsoft.Json;
using SyncVR.DLC;
using SyncVR.Analytics;

namespace SyncVR.Video
{
    public class LocalVideoService : MonoBehaviour
    {
        public static LocalVideoService Instance { get; private set; }

        private static readonly List<string> video3DModeOptions = new List<string> { "none", "over-under", "side-by-side" };
        private const string settings_filename = "video_settings.json";

        public List<VideoSettings> localVideos { get; private set; }
        public bool bundlesLoaded { get; private set; }
        public VideoSettings currentVideo;

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

        public List<string> GetCategories ()
        {
            return localVideos.Select(x => x.video_category).Distinct().ToList();
        }

        public List<VideoSettings> GetVideosByCategory (string category)
        {
            return localVideos.Where(x => x.video_category == category).ToList();
        }

        public VideoSettings NextVideo ()
        {
            int i = localVideos.IndexOf(currentVideo);
            return localVideos[(i + 1) % localVideos.Count];
        }

        public VideoSettings NextVideoByCategory ()
        {
            List<VideoSettings> playlist = GetVideosByCategory(currentVideo.video_category);
            int i = playlist.IndexOf(currentVideo);
            return playlist[(i + 1) % playlist.Count];
        }

        public void ProcessAssetBundles (List<DLCBundle> bundles)
        {
            bundlesLoaded = false;
            localVideos = new List<VideoSettings>();
            StartCoroutine(ProcessAssetBundlesRoutine(bundles));
        }

        private IEnumerator ProcessAssetBundlesRoutine (List<DLCBundle> bundles)
        {
            foreach (DLCBundle bundle in bundles)
            {
                VideoSettings v;
                AssetBundleCreateRequest loadBundleRequest = AssetBundle.LoadFromFileAsync(bundle.path);
                yield return loadBundleRequest;

                if (loadBundleRequest.assetBundle == null)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "couldn't load assetbundle: " + bundle.name } });
                    continue;
                }

                try
                {
                    v = ReadSettingsFromFile(loadBundleRequest.assetBundle);
                    v.loadedBundle = loadBundleRequest.assetBundle;
                }
                catch (Exception e)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "couldn't read json file for " + bundle.name + ": " + e.Message } });
                    continue;
                }

                AssetBundleRequest loadThumbnailSprite = v.loadedBundle.LoadAssetAsync<Sprite>(v.thumbnail_file);
                yield return loadThumbnailSprite;

                if (loadThumbnailSprite.asset == null)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "couldn't load sprite for video: " + v.title } });
                    continue;
                }

                v.thumbnail_sprite = (Sprite)loadThumbnailSprite.asset;

                AssetBundleRequest loadVideo = v.loadedBundle.LoadAssetAsync<VideoClip>(v.video_file);
                yield return loadVideo;

                if (loadVideo.asset == null)
                {
                    AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "couldn't load video: " + v.title } });
                    continue;
                }

                v.video = (VideoClip)loadVideo.asset;

                localVideos.Add(v);
            }

            bundlesLoaded = true;
        }

        public VideoSettings GetLocalVideo (string video_id)
        {
            if (localVideos != null)
            {
                return localVideos.First(x => x.vid == video_id);
            }
            else
            {
                return null;
            }
        }

        private VideoSettings ReadSettingsFromFile (AssetBundle b)
        {
            TextAsset meta = b.LoadAsset<TextAsset>(settings_filename);
            VideoSettings video_settings = JsonConvert.DeserializeObject<VideoSettings>(meta.text);

            return video_settings;
        }

        public void ClearAssetBundles ()
        {
            localVideos.ForEach(x => x.loadedBundle.Unload(true));
            bundlesLoaded = false;
            localVideos = null;
        }
    }
}