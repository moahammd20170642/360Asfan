using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour
{
    public static class VideoUtils
    {
        public static void CreateVideoPlayer(this VideoPanel p)
        {
            CreateVideoPlayer(
                p.name,
                p.video,
                ref p.videoPlayer,
                p.videoURL,
                p.streamingAssetsVidPath,
                p.videoType,
                p.loopVideo,
                p.playAtStart);
        }

        public static void CreateVideoPlayer(this VideoPoint p)
        {
            CreateVideoPlayer(
                p.name,
                p.video,
                ref p.videoPlayer,
                p.videoURL,
                p.streamingAssetsVidPath,
                p.videoType,
                p.loopVideo,
                false);
        }

        public static void CreateVideoPlayer(this Location l)
        {
            CreateVideoPlayer(
                l.name,
                l.Video,
                ref l.videoPlayer,
                l.VideoURL,
                l.StreamingAssetsVidPath,
                l.videoType,
                l.loopVideo,
                false);
        }

        public static void CreateVideoPlayer(
            string objName,
            VideoClip video,
            ref VideoPlayer videoPlayer,
            string videoURL,
            string streamingAssetsVidPath,
            VideoType videoType,
            bool loopVideo,
            bool playAtStart)
        {
            if (videoPlayer == null)
            {
                videoPlayer = new GameObject().AddComponent<VideoPlayer>();
                videoPlayer.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
            }
            videoPlayer.playOnAwake = playAtStart;
            videoPlayer.isLooping = loopVideo;

            switch (videoType)
            {
                case VideoType.Local:
                    videoPlayer.source = VideoSource.VideoClip;
                    videoPlayer.clip = video;
                    videoPlayer.name = $"Video Player ({video.name})";

                    if (video == null)
                        Debug.LogError(objName + ": Video is not specified!");
                    break;

                case VideoType.StreamingAssets:
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, streamingAssetsVidPath);
                    videoPlayer.name = $"Video Player (StreamingAssets/{streamingAssetsVidPath})";

                    if (string.IsNullOrWhiteSpace(streamingAssetsVidPath))
                        Debug.LogError(objName + ": Streaming assets video path is empty!");
                    break;

                case VideoType.URL:
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = videoURL;
                    videoPlayer.name = $"Video Player ({videoURL})";

                    if (string.IsNullOrWhiteSpace(videoURL))
                        Debug.LogError(objName + ": Video URL is empty!");
                    break;
            }
        }

        public static void GoToFirstFrame(this VideoPlayer videoPlayer)
        {
            videoPlayer.frame = 0;
        }
    }
}
