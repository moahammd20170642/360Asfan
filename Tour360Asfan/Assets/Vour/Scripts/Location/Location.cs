using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace CrizGames.Vour
{
    public class Location : MonoBehaviour
    {
        public enum LocationType
        {
            Empty = 0,
            Image = 1,
            Image3D = 2,
            Image360 = 3,
            Image3D360 = 4,
            Video = 5,
            Video3D = 6,
            Video360 = 7,
            Video3D360 = 8,
            Scene = 9
        }

        public enum _3DLayout
        {
            OverUnder,
            SideBySide
        }

        public enum VideoType
        {
            Local,
            StreamingAssets,
            URL
        }

        // IMPORTANT: When renaming these variables, you also need to update them in LocationEditor.OnEnable

        public LocationType locationType;

        public _3DLayout _3D_Layout;

        public Texture2D Texture;

        public VideoClip Video;
        public VideoPlayer videoPlayer;
        public string VideoURL;
        public string StreamingAssetsVidPath;
        public VideoType videoType;

        [Tooltip("Scale image/video to fullscreen height")]
        public bool scaleToFullscreen = false;

        public bool lockCamera = false;
        public bool loopVideo = true;
        public bool videoUI = true;
        public bool videoUIAudioVolume = true;
        public bool videoUILoopButton = true;
        [Range(0f, 1f)] public float videoVolume = 1f;

        public SceneReference scene;

        public Vector3 rotOffset;

        private TeleportPoint[] _teleportPoints;

        /// <summary>
        /// Init is called from LocationManager because the GameObject of Location is inactive
        /// </summary>
        public void Init()
        {
            // Create a video player object for the video location
            if (locationType.IsVideo())
                this.CreateVideoPlayer();

            _teleportPoints = GetComponentsInChildren<TeleportPoint>();
        }

        /// <summary>
        /// SetSourcesToLocation
        /// </summary>
        public void SetSourcesToLocation(LocationBase l)
        {
            l.location = this;
            l.texture = Texture;
            l.video = Video;
            l.scene = scene;
            l.rotOffset = rotOffset;

            // Reset player rotation if this is a non-360 location
            if (Application.isPlaying && !locationType.Is360())
                PlayerBase.Instance.ResetRotation();

            if (Application.isPlaying && PlayerBase.Instance is DesktopPlayer player)
                player.canMoveCam = !lockCamera || locationType.Is360();

            l.Init();
            l.UpdateLocation();
        }

        /// <summary>
        /// Smart preload.
        /// Preload video locations linked to this location via teleport points
        /// </summary>
        public void PreloadLinkedVideos()
        {
            foreach (var t in _teleportPoints)
            {
                if (t.TargetLocation != null && t.TargetLocation.locationType.IsVideo() && !t.TargetLocation.videoPlayer.isPrepared)
                    t.TargetLocation.videoPlayer.Prepare();
            }
        }
        
        /// <summary>
        /// Unloads all video locations the player didn't jump to
        /// </summary>
        public void UnloadLinkedVideos(Location nextLocation)
        {
            foreach (var t in _teleportPoints)
            {
                if (t.TargetLocation != null && t.TargetLocation.locationType.IsVideo() && t.TargetLocation != nextLocation)
                    t.TargetLocation.videoPlayer.Stop();
            }

            bool nextHasLinkToThisLoc = nextLocation._teleportPoints.Any(t => t.TargetLocation == this);
            if (locationType.IsVideo())
            {
                // Don't unload video if connected
                if (nextHasLinkToThisLoc)
                {
                    videoPlayer.Pause();
                    videoPlayer.time = 0;
                }
                // Unload video if not connected
                else
                    videoPlayer.Stop(); 
            }
        }

        private void OnApplicationQuit()
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
                videoPlayer.Stop();
        }
    }
}