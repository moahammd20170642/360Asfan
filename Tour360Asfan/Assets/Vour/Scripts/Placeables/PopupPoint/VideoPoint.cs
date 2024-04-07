using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour
{
    /// <summary>
    /// VideoPoint
    /// </summary>
    public class VideoPoint : PopupPoint
    {
        [SerializeField] private RawImage videoImg;
        private RenderTexture renderTexture;

        private VideoController controller;
        private VideoUIController UIcontroller;

        public VideoClip video;
        public VideoPlayer videoPlayer;
        public string videoURL;
        public string streamingAssetsVidPath;
        public VideoType videoType;
        public bool loopVideo = true;
        public bool videoUI = true;
        public bool videoUIAudioVolume = true;
        public bool videoUILoopButton = true;
        [Range(0f, 1f)]
        public float videoVolume = 1f;

        public override Transform panelParent => transform;

        private void Awake()
        {
            panel.gameObject.SetActive(false);
        }

        public override void InitPanel()
        {
            this.CreateVideoPlayer();

            panel.localScale = new Vector3(0, 0, 1);
            panel.gameObject.SetActive(false);

            controller = panel.GetComponent<VideoController>();
            UIcontroller = panel.GetComponent<VideoUIController>();

            // Set loading texture while video is loading
            if (Application.isPlaying)
            {
                if (!videoPlayer.isPrepared)
                {
                    SetTex(VourSettings.Instance.loadingTexture);

                    // When video loaded
                    controller.OnVideoLoaded.AddListener(SetupLoadedVideo);
                }
                else
                {
                    // When video is already loaded
                    SetupLoadedVideo();
                }
            }

            GetVideoSize((width, height) => UpdateSize(new Vector2(width, height)));

            // Enable and initialize videoController
            panel.gameObject.SetActive(true);
            if (videoUI)
                UIcontroller.EnableUI(videoPlayer, videoUIAudioVolume, videoUILoopButton);
            else
            {
                UIcontroller.DisableUI();
                controller.Init(videoPlayer);
            }
        }

        public override void Interact()
        {
            base.Interact();

            if (open)
                videoPlayer.GoToFirstFrame();
            controller.SetPlayingState(open);
        }

        private void GetVideoSize(System.Action<int, int> callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (video != null && videoType == VideoType.Local)
                    callback.Invoke((int)video.width, (int)video.height);
                else
                    callback.Invoke(128, (int)(128 * (16f / 9f)));
                return;
            }
#endif
            switch (videoType)
            {
                case VideoType.Local:
                    if (video == null)
                        Debug.LogError("There is no video to play!");

                    callback.Invoke((int)video.width, (int)video.height);
                    break;

                case VideoType.StreamingAssets:
                case VideoType.URL:
                    void CallbackTextureSize()
                    {
                        Texture tex = videoPlayer.texture;
                        callback.Invoke(tex.width, tex.height);
                    }

                    if (videoPlayer.isPrepared)
                        CallbackTextureSize();
                    else
                        controller.OnVideoLoaded.AddListener(CallbackTextureSize);
                    break;
            }
        }

        private void UpdateSize(Vector2 size)
        {
            RectTransform t = panel.GetComponent<RectTransform>();
            t.sizeDelta = new Vector2(size.x / size.y * t.sizeDelta.y, t.sizeDelta.y);

            // Adjust video UI width
            RectTransform vidUIT = t.GetChild(1).GetComponent<RectTransform>();
            vidUIT.sizeDelta = new Vector2(t.sizeDelta.x / vidUIT.localScale.x - 3, vidUIT.sizeDelta.y);
        }

        private void SetupLoadedVideo()
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

            SetTex(renderTexture);

            controller.SetAudioVolume(videoVolume);
        }

        private void SetTex(Texture tex)
        {
            videoImg.texture = tex;

            if (tex.GetType() == typeof(RenderTexture))
                videoPlayer.targetTexture = (RenderTexture)tex;
        }

        protected override Transform FindPanel()
        {
            return transform.GetChild(2);
        }
    }
}