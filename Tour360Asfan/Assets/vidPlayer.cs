using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class vidPlayer : MonoBehaviour
{
    public string videofileName;
    // Start is called before the first frame update
    //private void OnEnable()
    //{
    //    Playvideo();
    //}

    // Update is called once per frame

    public void StopVideo()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.Stop();
    }
    public void Playvideo()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();

        if (player)
        {
            string videopath = System.IO.Path.Combine(Application.streamingAssetsPath, videofileName);
            Debug.Log(videopath);
            player.url = videopath;
            player.Play();
        }

       
    }
}
