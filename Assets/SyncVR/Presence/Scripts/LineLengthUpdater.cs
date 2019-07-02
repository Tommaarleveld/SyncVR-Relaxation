using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineLengthUpdater : MonoBehaviour
{
    public Transform pointer;

    public void Update()
    {
        float d = Vector3.Distance(pointer.position, transform.position);
        transform.localScale = new Vector3(1f, 1f, Mathf.Max(d, 5f));
    }
}
