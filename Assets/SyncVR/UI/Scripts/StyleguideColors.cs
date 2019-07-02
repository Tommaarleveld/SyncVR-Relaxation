using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyncVR.UI
{
    public static class StyleguideColors
    {
        public static readonly Color purple = new Color(77f / 255f, 68f / 255f, 110f / 255f);
        public static readonly Color orange = new Color(253f / 255f, 189f / 255f, 119f / 255f);
        public static readonly Color pink = new Color(1f, 105f / 255f, 116f / 255f);
        public static readonly Color grey = new Color(184f / 255f, 180f / 255f, 197f / 255f);

        public static readonly Color orange_fade = new Color(253f / 255f, 189f / 255f, 119f / 255f, 155f / 255f);
        public static readonly Color purple_fade = new Color(77f / 255f, 68f / 255f, 110f / 255f, 155f / 255f);

        [System.Serializable]
        public enum ColorSchemes
        {
            Orange,
            Pink
        }
    }
}
