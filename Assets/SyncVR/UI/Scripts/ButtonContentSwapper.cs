using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SyncVR.UI
{
    public class ButtonContentSwapper : MonoBehaviour
    {
        public List<Image> images;
        public Sprite primary;
        public Sprite secondary;

        public List<Text> texts;
        public string primaryText;
        public string secondaryText;

        public void Start()
        {

        }

        public void SetPrimary()
        {
            images.ForEach(x => x.sprite = primary);
            texts.ForEach(x => x.text = primaryText);
        }

        public void SetSecondary()
        {
            images.ForEach(x => x.sprite = secondary);
            texts.ForEach(x => x.text = secondaryText);
        }

        public void Swap()
        {
            images.ForEach(x => x.sprite = (x.sprite == primary ? secondary : primary));
            texts.ForEach(x => x.text = (x.text == primaryText ? secondaryText : primaryText));
        }
    }
}
