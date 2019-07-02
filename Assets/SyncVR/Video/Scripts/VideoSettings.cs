using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace SyncVR.Video
{
    public class VideoSettings
    {
        public string vid;
        public string title;
        public string video_description;
        public string video_category;
        public string video_file;
        public bool is_3d;
        public int video_length_seconds;
        public int video_aspect_x;
        public int video_aspect_y;
        public string video_3d_mode;
        public string thumbnail_file;
        public Sprite thumbnail_sprite;
        public VideoClip video;
        public AssetBundle loadedBundle;

        public class VideoSettingsComparer : IEqualityComparer<VideoSettings>
        {
            public bool Equals (VideoSettings x, VideoSettings y)
            {
                return x.vid == y.vid && x.title == y.title;
            }

            public int GetHashCode (VideoSettings v)
            {
                return v.vid.GetHashCode();
            }
        }

        public VideoSettings ()
        {
            vid = "";
            title = "";
            video_description = "";
            video_category = "";
            video_file = "";
            is_3d = true;
            video_length_seconds = 0;
            video_aspect_x = 0;
            video_aspect_y = 0;
            video_3d_mode = "";
            thumbnail_file = "";
        }

        public VideoSettings (string video_id, string video_title)
        {
            vid = video_id;
            title = video_title;
        }

        public override string ToString ()
        {
            return "" + vid + ", " + title + ", " + video_file + ", " + thumbnail_file;
        }
    }
}