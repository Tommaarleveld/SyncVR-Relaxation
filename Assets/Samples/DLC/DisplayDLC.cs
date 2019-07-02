using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using SyncVR.DLC;
using SyncVR.GeoGuesser;
using SyncVR.Util;

public class DisplayDLC : MonoBehaviour
{
    public Text assetBundleListText;
    public Text geoguesserBundleContentText;

    public Text toDownloadText;
    public Text toDeleteText;
    public Button downloadButton;
    public Button deleteButton;

    private List<UnityWebRequestAsyncOperation> asyncOperations = new List<UnityWebRequestAsyncOperation>();

    public void Start ()
    {
        downloadButton.interactable = false;
        deleteButton.interactable = false;
        Debug.Log("Initial free space: " + SystemInfoUtil.GetAvailableStorage());
        Debug.Log("Device ID: " + SystemInfoUtil.DeviceID());
    }

    public void RefreshContent ()
    {
        DisplayLocalBundles();
        StartCoroutine(LoadGeoGuesserBundles());
        StartCoroutine(DisplayAllocatedPackages());
    }

    public void DisplayLocalBundles ()
    {
        DLCLocalService.Instance.ReadDLCList(true);

        string s = "";
        List<string> categories = DLCLocalService.Instance.GetBundlesCategories();

        categories.ForEach(
            x =>
            {
                s += x + ":\n";
                List<DLCBundle> assets = DLCLocalService.Instance.GetBundlesByCategory(x);
                assets.ForEach(
                    y =>
                    {
                        s += "   " + y.name + "\n";
                    }
                );
            }
        );

        assetBundleListText.text = s;
    }

    private IEnumerator DisplayAllocatedPackages ()
    {
        DLCManager.Instance.RetrieveBundleAllocations();
        while (!DLCManager.Instance.allocationsRetrieved)
        {
            yield return null;
        }

        // display the packages we are allocated

        DLCManager.Instance.CalculateBundleDiff();
        while (!DLCManager.Instance.diffCalculated)
        {
            yield return null;
        }

        // display what we need to download and/or delete
        string s = "";

        foreach (DLCBundle bundle in DLCManager.Instance.toDownload)
        {
            s += bundle.ToString() + ": " + bundle.byteSize / 1000000f + "MB \n";
        }
        toDownloadText.text = s;

        s = "";
        DLCManager.Instance.toDelete.ForEach(x => s += x.ToString() + "\n");
        toDeleteText.text = s;

        // enable the button
        downloadButton.interactable = true;
        deleteButton.interactable = true;
    }


    private IEnumerator LoadGeoGuesserBundles ()
    {
        List<DLCBundle> bundles = DLCLocalService.Instance.GetBundlesByCategory("geoguesser");

        GeoGuesserService.Instance.ProcessAssetBundles(bundles);
        while (!GeoGuesserService.Instance.bundlesLoaded)
        {
            yield return null;
        }

        string s = "";
        GeoGuesserService.Instance.geoGuesserLocations.ForEach(x => s += x.location_name + "\n");
        geoguesserBundleContentText.text = s;
    } 

    public void DownloadBundles ()
    {
        downloadButton.interactable = false;

        // download stuff and delete stuff
        StartCoroutine(DownloadBundlesRoutine());
    }

    public void DeleteBundles ()
    {
        deleteButton.interactable = false;
        DLCManager.Instance.toDelete.ForEach(x => DLCManager.Instance.DeleteBundle(x));
    }

    private IEnumerator DownloadBundlesRoutine ()
    {
        for (int i = 0; i < DLCManager.Instance.toDownload.Count; i++)
        {
            DLCBundle bundle = DLCManager.Instance.toDownload[i];
            Debug.Log("Start downloading bundle: " + bundle.ToString());

            UnityWebRequest r = DLCManager.Instance.DownloadBundleRequest(bundle);
            UnityWebRequestAsyncOperation op = r.SendWebRequest();

            asyncOperations.Add(op);
        }

        while (asyncOperations.Where(x => !x.isDone).Count() > 0)
        {
            string text = "";
            for (int i = 0; i < asyncOperations.Count; i++)
            {
                text += "" + asyncOperations[i].priority + " - " + DLCManager.Instance.toDownload[i].name + ": " + asyncOperations[i].progress + " \n";
            }

            toDownloadText.text = text;
            yield return new WaitForSeconds(1f);
        }

        for (int i= 0; i < DLCManager.Instance.toDownload.Count; i++)
        {
            UnityWebRequestAsyncOperation op = asyncOperations[i];

            if (op.webRequest.isHttpError)
            {
                Debug.Log("HTTP Error for: " + op.webRequest.url + " " + op.webRequest.error);
            }
            else if (op.webRequest.isNetworkError)
            {
                Debug.Log("NetworkError for: " + op.webRequest.url + " " + op.webRequest.error);
            }
            else
            {
                Debug.Log("Succes for: " + op.webRequest.url);
            }

            Debug.Log("Finalizing request!");
            yield return StartCoroutine(DLCManager.Instance.FinalizeDownloadRequest(op.webRequest, DLCManager.Instance.toDownload[i]));
        }

        Debug.Log("Finished!");
    }

    public void OnApplicationQuit ()
    {
        asyncOperations.ForEach(x => x.webRequest.Abort());
    }
}
