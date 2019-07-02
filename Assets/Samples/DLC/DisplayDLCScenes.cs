using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SyncVR.DLC;
using SyncVR.UI;

public class DisplayDLCScenes : MonoBehaviour
{
    public RectTransform buttonContainer;
    public GameObject buttonPrefab;

    public void Start()
    {
        DLCLocalService.Instance.ReadDLCList(true);
        List<DLCBundle> appBundles = DLCLocalService.Instance.GetBundlesByCategory("apps");

        foreach (DLCBundle bundle in appBundles)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => DLCSceneLoader.Instance.LoadDLCScene(bundle, ErrorCallback));
            button.GetComponent<MainMenuButton>().title.text = bundle.ToString();
            button.transform.SetParent(buttonContainer, false);
        }
    }

    public void ErrorCallback (string error)
    {
        Debug.Log("ErrorCallback: " + error);
    }

}
