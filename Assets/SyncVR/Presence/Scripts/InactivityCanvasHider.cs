using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactivityCanvasHider : MonoBehaviour
{
    private const float fadeInTime = 0.5f;
    private const float fadeOutTime = 2f;

    private CanvasGroup canvasGroup;
    private ControllerInactivityDetector controller;

    public void Start ()
    {
        controller = FindObjectOfType<ControllerInactivityDetector>();
        controller.ControllerBecameActive += OnControllerBecameActive;
        controller.ControllerBecameInactive += OnControllerBecameInactive;

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
	}

    public void OnControllerBecameActive ()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void OnControllerBecameInactive ()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn ()
    {
        float startAlpha = canvasGroup.alpha;
        float startTime = Time.time;

        while (Time.time < startTime + fadeInTime)
        {
            canvasGroup.alpha = startAlpha + ((1f - startAlpha) * ((Time.time - startTime) / fadeInTime));
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut ()
    {
        float startAlpha = canvasGroup.alpha;
        float startTime = Time.time;

        while (Time.time < startTime + fadeOutTime)
        {
            canvasGroup.alpha = startAlpha - (startAlpha * ((Time.time - startTime) / fadeOutTime));
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
