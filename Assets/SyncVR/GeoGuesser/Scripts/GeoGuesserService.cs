using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SyncVR.DLC;
using SyncVR.Analytics;

namespace SyncVR.GeoGuesser
{
    public class GeoGuesserService : MonoBehaviour
    {
        public static GeoGuesserService Instance { get; private set; }

        private const string settings_filename = "location_settings";
        private const int QUIZ_SIZE = 5;
        private const int QUIZ_NUM_ANSWERS = 4;

        public List<GeoGuesserLocation> geoGuesserLocations { get; private set; }
        public bool bundlesLoaded { get; protected set; }

        public int totalBundles { get; private set; }
        public int loadedBundles { get; private set; }

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

        void Start ()
        {
            bundlesLoaded = false;
        }

        public GeoGuesserLocation GetLocation (string location_name)
        {
            return geoGuesserLocations.FirstOrDefault(x => x.location_name == location_name);
        }

        public List<string> GetCategories()
        {
            return geoGuesserLocations.Select(x => x.category).Distinct().ToList();
        }

        public List<string> GetCountries()
        {
            return geoGuesserLocations.Select(x => x.country).Distinct().ToList();
        }

        public List<string> GetCities()
        {
            return geoGuesserLocations.Select(x => x.city).Distinct().ToList();
        }

        public List<GeoGuesserQuestion> GetWorldQuiz ()
        {
            if (geoGuesserLocations.Count < QUIZ_SIZE)
            {
                return null;
            }

            return MakeQuiz(geoGuesserLocations);
        }

        public List<GeoGuesserQuestion> GetCountryQuiz (string country)
        {
            List<GeoGuesserLocation> eligible = geoGuesserLocations.Where(x => x.country == country).ToList();

            if (eligible.Count < QUIZ_SIZE)
            {
                return null;
            }

            return MakeQuiz(eligible);
        }

        public List<GeoGuesserQuestion> GetCityQuiz (string city)
        {
            List<GeoGuesserLocation> eligible = geoGuesserLocations.Where(x => x.city == city).ToList();

            if (eligible.Count < QUIZ_SIZE)
            {
                return null;
            }

            return MakeQuiz(eligible);
        }

        public List<GeoGuesserQuestion> GetCategoryQuiz (string category)
        {
            List<GeoGuesserLocation> eligible = geoGuesserLocations.Where(x => x.category == category).ToList();

            if (eligible.Count < QUIZ_SIZE)
            {
                return null;
            }

            return MakeQuiz(eligible);
        }

        private List<GeoGuesserQuestion> MakeQuiz (List<GeoGuesserLocation> options)
        {
            options.Shuffle();
            List<GeoGuesserQuestion> quiz = new List<GeoGuesserQuestion>();

            for (int i = 0; i < QUIZ_SIZE; i++)
            {
                GeoGuesserQuestion q = new GeoGuesserQuestion();
                q.correctAnswer = options[i].location_name;

                while (q.wrongAnswers.Count < QUIZ_NUM_ANSWERS - 1)
                {
                    string s = options[UnityEngine.Random.Range(0, options.Count)].location_name;
                    if (s != q.correctAnswer && !q.wrongAnswers.Contains(s))
                    {
                        q.wrongAnswers.Add(s);
                    }
                }

                quiz.Add(q);
            }

            return quiz;
        }

        private IEnumerator LocationsFromAssetBundle (DLCBundle bundle)
        {
            AssetBundleCreateRequest loadBundleRequest = AssetBundle.LoadFromFileAsync(bundle.path);
            yield return loadBundleRequest;

            if (loadBundleRequest.assetBundle == null)
            {
                AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "couldn't load assetbundle: " + bundle.path } });
                yield break;
            }

            AssetBundle b = loadBundleRequest.assetBundle;
            List<GeoGuesserLocation> newLocations;

            try
            {
                TextAsset meta = b.LoadAsset<TextAsset>(settings_filename);
                newLocations = JsonConvert.DeserializeObject<List<GeoGuesserLocation>>(meta.text);
            }
            catch (Exception e)
            {
                AnalyticsService.Instance.LogEvent(AnalyticsService.EventType.Error, new Dictionary<string, object> { { "msg", "couldn't read json for " + bundle.name + ": " + e.Message } });
                yield break;
            }

            foreach(GeoGuesserLocation l in newLocations)
            {
                AssetBundleRequest bundleRequest = b.LoadAssetAsync<Texture2D>(l.filename);
                yield return bundleRequest;
                l.texture = (Texture2D)bundleRequest.asset;
            }
            b.Unload(false);

            geoGuesserLocations.AddRange(newLocations);
        }

        private IEnumerator ProcessAssetBundlesRoutine (List<DLCBundle> bundles)
        {
            totalBundles = bundles.Count;
            loadedBundles = 0;
            foreach (DLCBundle bundle in bundles)
            {
                yield return StartCoroutine(LocationsFromAssetBundle(bundle));
                loadedBundles++;
            }

            geoGuesserLocations = geoGuesserLocations
                .GroupBy(x => x.location_name)
                .Select(g => g.First())
                .ToList();

            bundlesLoaded = true;
        }

        public void ProcessAssetBundles (List<DLCBundle> bundles)
        {
            bundlesLoaded = false;
            geoGuesserLocations = new List<GeoGuesserLocation>();
            StartCoroutine(ProcessAssetBundlesRoutine(bundles));
        }

        public void ClearAssetBundles ()
        {
            Resources.UnloadUnusedAssets();
            bundlesLoaded = false;
            geoGuesserLocations = null;
        }
    }
}
