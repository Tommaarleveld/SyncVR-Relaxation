using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactivityControllerHider : MonoBehaviour
{
    public bool hideOnInactivity;
    public Renderer controllerRenderer;
    public LineRenderer lineRenderer;
    public Renderer iconRenderer;

    private const float fadeInTime = 0.5f;
    private const float fadeOutTime = 2f;

    private ControllerInactivityDetector detector;
    private float currentAlpha = 1f;

    public void Start ()
    {
        detector = GetComponent<ControllerInactivityDetector>();
        detector.ControllerBecameActive += OnControllerBecameActive;
        detector.ControllerBecameInactive += OnControllerBecameInactive;
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
        float startAlpha = currentAlpha;
        float startTime = Time.time;

        while (Time.time < startTime + fadeInTime)
        {
            currentAlpha = startAlpha + ((1f - startAlpha) * ((Time.time - startTime) / fadeInTime));
            SetAlphas(currentAlpha);
            yield return null;
        }
    }

    private IEnumerator FadeOut ()
    {
        float startAlpha = currentAlpha;
        float startTime = Time.time;

        while (Time.time < startTime + fadeOutTime)
        {
            currentAlpha = startAlpha - (startAlpha * ((Time.time - startTime) / fadeOutTime));
            SetAlphas(currentAlpha);
            yield return null;
        }
    }

    private void SetAlphas (float alpha)
    {
        controllerRenderer.material.color = new Color(1f, 1f, 1f, alpha);
        lineRenderer.startColor = new Color(1f, 1f, 1f, alpha);
        lineRenderer.endColor = new Color(1f, 1f, 1f, alpha);
        iconRenderer.material.color = new Color(1f, 1f, 1f, alpha);
    }
}
