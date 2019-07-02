using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SyncVR.UI
{
    public abstract class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool interactable = true;

        public abstract void OnPointerEnter (PointerEventData eventData);
        public abstract void OnPointerExit (PointerEventData eventData);
        public abstract void OnPointerDown (PointerEventData eventData);
        public abstract void OnPointerUp (PointerEventData eventData);

        public abstract void SetHighlighted ();
        public abstract void SetNotHighlighted ();
        public abstract void SetPressed ();
    }
}
