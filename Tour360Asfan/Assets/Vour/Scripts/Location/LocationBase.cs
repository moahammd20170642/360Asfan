using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour
{
    /// <summary>
    /// Base for all locations
    /// </summary>
    public abstract class LocationBase : MonoBehaviour
    {
        public LocationType locationType;

        public Texture2D texture;

        public VideoClip video;

        public SceneReference scene;

        public Vector3 rotOffset;

        [HideInInspector] public Location location;
        
        public bool IsReady { get; protected set; }

        /// <summary>
        /// Init
        /// </summary>
        public virtual void Init()
        {
            IsReady = false;
        }

        /// <summary>
        /// UpdateLocation
        /// </summary>
        public abstract void UpdateLocation();
    }
}