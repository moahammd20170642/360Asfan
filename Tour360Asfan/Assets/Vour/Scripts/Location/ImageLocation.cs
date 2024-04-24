using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    /// <summary>
    /// Image
    /// </summary>
    public class ImageLocation : MediaLocation
    {
        /// <summary>
        /// UpdateLocation
        /// </summary>
        public override void UpdateLocation()
        {
            SetMedia(texture);

            UpdateSize(texture ? new Vector2(texture.width, texture.height) : Vector2.one * Size);
            IsReady = true;
        }
    }
}