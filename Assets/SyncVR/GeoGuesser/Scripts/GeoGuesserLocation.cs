using UnityEngine;

namespace SyncVR.GeoGuesser
{
    public class GeoGuesserLocation
    {
        public string location_name;
        public string country;
        public string city;
        public string category;
        public string filename;
        public Texture2D texture;
        public float rotation;

        public GeoGuesserLocation ()
        {
            location_name = "";
            country = "";
            city = "";
            category = "";
            filename = "";
            texture = null;
            rotation = 0f;
        }
    }
}


