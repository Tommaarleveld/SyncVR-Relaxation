using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SyncVR.Video;
using SyncVR.DLC;

public class MovieService : MonoBehaviour
{
    public Button displayMoviesButton;
    public Text videoListText;
    public Image thumbnail;

    private VideoPlayer videoPlayer;

    public void Start ()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        DisplayLocalMovies();
    }

    public void DisplayLocalMovies ()
    {
        displayMoviesButton.interactable = false;
        DLCLocalService.Instance.ReadDLCList();
        StartCoroutine(DisplayLocalMoviesRoutine());
    }

    private IEnumerator DisplayLocalMoviesRoutine ()
    {
        List<DLCBundle> bundles = DLCLocalService.Instance.GetBundlesByCategory("movies");
        LocalVideoService.Instance.ProcessAssetBundles(bundles);
        while (!LocalVideoService.Instance.bundlesLoaded)
        {
            yield return null;
        }

        VideoSettings toPlay = LocalVideoService.Instance.localVideos[0];

        videoListText.text = toPlay.title;

        thumbnail.sprite = toPlay.thumbnail_sprite;

        RenderTexture texture = new RenderTexture(toPlay.video_aspect_x, toPlay.video_aspect_y, 24);
        texture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        texture.Create();

        // set texture for the correct material
        RenderSettings.skybox.SetTexture("_MainTex", texture);

        // set the VideoPlayer to render to the new texture
        videoPlayer.targetTexture = texture;
        videoPlayer.clip = toPlay.video;
        videoPlayer.Prepare();

        float t = Time.time;
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
        displayMoviesButton.interactable = true;
    }

}
