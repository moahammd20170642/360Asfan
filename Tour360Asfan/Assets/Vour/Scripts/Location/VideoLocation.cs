using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace CrizGames.Vour
{
    public class VideoLocation : MediaLocation
    {
        private RenderTexture renderTexture;

        /// <summary>
        /// Init
        /// </summary>
        public override void Init()
        {
            base.Init();

            gameObject.SetActive(true);

            if (Application.isPlaying)
            {
                if (location.videoUI)
                {
                    MainVideoUIController.Instance.EnableUI(location.videoPlayer, location.videoUIAudioVolume, location.videoUILoopButton);
                    MainVideoUIController.Instance.GetComponent<VideoController>().SetAudioVolume(location.videoVolume);
                }

                location.videoPlayer.Play();
            }
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        public virtual void OnDisable()
        {
            if (location == null || location.videoPlayer == null)
                return;

            if (location.videoUI)
                MainVideoUIController.Instance.DisableUI();
        }

        /// <summary>
        /// UpdateLocation
        /// </summary>
        public override void UpdateLocation()
        {
#if UNITY_EDITOR
            // Set video in case the user changed it while playing
            if (Application.isPlaying)
            {
                location.Init();
                location.videoPlayer.Play();
            }
#endif

            // Set loading texture while video is loading
            if (Application.isPlaying)
            {
                if (!location.videoPlayer.isPrepared)
                {
                    SetMedia(VourSettings.Instance.loadingTexture);

                    // When video loaded
                    location.videoPlayer.prepareCompleted += SetupLoadedVideo;
                }
                else
                {
                    // When video is already loaded
                    SetupLoadedVideo(location.videoPlayer);
                }
            }
#if UNITY_EDITOR
            else
            {
                // Set a gray texture so you don't get a flashbang in your face
                Texture2D grayTex = new Texture2D(1, 1);
                grayTex.SetPixel(0, 0, Color.gray);
                grayTex.Apply();

                SetMedia(grayTex);
            }
#endif

            GetVideoSize((width, height) => UpdateSize(new Vector2(width, height)));
        }

        private void SetupLoadedVideo(VideoPlayer videoPlayer)
        {
            Texture tex = videoPlayer.texture;
            int width = tex.width;
            int height = tex.height;

            // Make new renderTexture if none there yet or size changed
            if (renderTexture == null || width != renderTexture.width || height != renderTexture.height)
            {
                if (renderTexture != null)
                {
                    renderTexture.DiscardContents();
                    renderTexture.Release();
                }
                renderTexture = new RenderTexture(width, height, 1);
            }

            videoPlayer.targetTexture = renderTexture;

            SetMedia(renderTexture);

            SetVideoVolume();
            
            IsReady = true;
        }

        private void GetVideoSize(System.Action<int, int> callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (video != null && location.videoType == Location.VideoType.Local)
                    callback.Invoke((int)video.width, (int)video.height);
                else
                    callback.Invoke(128, 128);
                return;
            }
#endif
            switch (location.videoType)
            {
                case Location.VideoType.Local:
                    if (video == null)
                        Debug.LogError("There is no video to play!");

                    callback.Invoke((int)video.width, (int)video.height);
                    break;

                case Location.VideoType.StreamingAssets:
                case Location.VideoType.URL:
                    void CallbackTextureSize()
                    {
                        Texture tex = location.videoPlayer.texture;
                        callback.Invoke(tex.width, tex.height);
                    }

                    if (location.videoPlayer.isPrepared)
                        CallbackTextureSize();
                    else
                        location.videoPlayer.prepareCompleted += (_) => CallbackTextureSize();
                    break;
            }
        }

        private void SetVideoVolume()
        {
            if (location.videoPlayer == null)
                return;

            for (ushort i = 0; i < location.videoPlayer.audioTrackCount; i++)
                location.videoPlayer.SetDirectAudioVolume(i, location.videoVolume);
        }
    }
}