﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SyncVR.UI
{
    public class MainMenuButton : SyncVR.UI.Button, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Image highlightBorder;
        public Image textArea;
        public Image arrow;
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
                SetNotHighlighted();
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

        public override void SetHighlighted ()
        {
            highlightBorder.gameObject.SetActive(true);
            arrow.color = StyleguideColors.orange;
            title.color = StyleguideColors.orange;
        }

        public override void SetNotHighlighted ()
        {
            highlightBorder.gameObject.SetActive(false);
            arrow.color = Color.white;
            title.color = Color.white;
        }

        public override void SetPressed ()
        {
            SetNotHighlighted();
        }
    }
}
