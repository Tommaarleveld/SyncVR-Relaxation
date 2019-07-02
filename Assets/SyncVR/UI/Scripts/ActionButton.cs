using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SyncVR.UI
{
    public class ActionButton : SyncVR.UI.Button, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public StyleguideColors.ColorSchemes colorScheme;
        public Image background;
        public Image icon;
        public Text title;

        private bool isPointerOver = false;

        public void Start ()
        {
            SetNotHighlighted();
        }

        public override void OnPointerEnter (PointerEventData eventData)
        {
            if (interactable)
            {
                isPointerOver = true;
                SetHighlighted();
            }
        }

        public override void OnPointerExit (PointerEventData eventData)
        {
            if (interactable)
            {
                isPointerOver = false;
                SetNotHighlighted();
            }
        }

        public override void OnPointerDown (PointerEventData eventData)
        {
            if (interactable)
            {
                SetPressed();
            }
        }

        public override void OnPointerUp (PointerEventData eventData)
        {
            if (interactable)
            {
                if (isPointerOver)
                {
                    SetHighlighted();
                }
                else
                {
                    SetNotHighlighted();
                }
            }
        }

        public override void SetHighlighted()
        {
            background.color = Color.white;

            if (colorScheme == StyleguideColors.ColorSchemes.Pink)
            {
                icon.color = StyleguideColors.pink;
                title.color = StyleguideColors.pink;
            }
            else if (colorScheme == StyleguideColors.ColorSchemes.Orange)
            {
                icon.color = StyleguideColors.purple;
                title.color = StyleguideColors.purple;
            }
        }

        public override void SetNotHighlighted()
        {
            if (colorScheme == StyleguideColors.ColorSchemes.Pink)
            {
                background.color = StyleguideColors.pink;
                icon.color = Color.white;
                title.color = Color.white;
            }
            else if (colorScheme == StyleguideColors.ColorSchemes.Orange)
            {
                background.color = StyleguideColors.orange;
                icon.color = StyleguideColors.purple;
                title.color = StyleguideColors.purple;
            }
        }

        public override void SetPressed ()
        {
            background.color = StyleguideColors.grey;

            if (colorScheme == StyleguideColors.ColorSchemes.Pink)
            {
                icon.color = StyleguideColors.pink;
                title.color = StyleguideColors.pink;
            }
            else if (colorScheme == StyleguideColors.ColorSchemes.Orange)
            {
                icon.color = StyleguideColors.purple;
                title.color = StyleguideColors.purple;
            }
        }
    }
}