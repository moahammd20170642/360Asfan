using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CrizGames.Vour.Location;

namespace CrizGames.Vour
{
    public abstract class MediaLocation : LocationBase
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        [SerializeField] private protected Material Material;

        public int Size = 4;

        public bool ResizeWidth = false;

        public bool _3D = false;

        private protected float rotationVelocity;

        public MeshRenderer[] rends;
        

        /// <summary>
        /// SetMedia
        /// </summary>
        protected virtual void SetMedia(Texture texture)
        {
            // Set texture
            if (_3D)
            {
                for (int i = 0; i < rends.Length; i++)
                {
                    // Create a material instance
                    rends[i].sharedMaterial = new Material(Material);
                    // Set texture
                    rends[i].sharedMaterial.SetTexture(MainTex, texture);

                    // Set scale and offset
                    GetScaleAndOffset(i, out Vector2 scale, out Vector2 offset);

                    rends[i].sharedMaterial.SetTextureScale(MainTex, scale);
                    rends[i].sharedMaterial.SetTextureOffset(MainTex, offset);
                }
            }
            else
            {
                Material.SetTexture(MainTex, texture);
            }

            // Rotate
            RotateMedia();
        }

        protected void RotateMedia()
        {
            for (int i = 0; i < rends.Length; i++)
            {
                if(location.locationType.Is360())
                    rends[i].transform.eulerAngles = rotOffset;
            }
        }

        protected void GetScaleAndOffset(int rendIdx, out Vector2 scale, out Vector2 offset)
        {
            switch (location._3D_Layout)
            {
                case _3DLayout.OverUnder:
                    scale = new Vector2(1f, 0.5f);
                    offset = new Vector2(0f, Mathf.RoundToInt(Mathf.Repeat((rendIdx + 1) * 0.5f, 0.99f) * 2) / 2f);
                    break;

                case _3DLayout.SideBySide:
                    scale = new Vector2(0.5f, 1f);
                    offset = new Vector2(rendIdx * 0.5f, 0f);
                    break;

                default:
                    scale = Vector2.one;
                    offset = Vector2.zero;
                    break;
            }
        }

        /// <summary>
        /// UpdateSize
        /// </summary>
        protected virtual void UpdateSize(Vector2 sourceSize)
        {
            foreach (MeshRenderer rend in rends)
            {
                Transform t = rend.transform;
                Vector3 scale = new Vector3(Size, Size, Size);

                float fullscreenMultiplier = Size;
                var cam = PlayerBase.Instance ? PlayerBase.Instance.GetCamera() : null;
                if (cam != null) // Screen height
                    fullscreenMultiplier = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, t.position.z)).y - cam.ScreenToWorldPoint(new Vector3(0, 0, t.position.z)).y;

                if (ResizeWidth)
                {
                    scale.x = sourceSize.x / sourceSize.y * scale.y;

                    if (_3D)
                    {
                        switch (location._3D_Layout)
                        {
                            case _3DLayout.OverUnder:
                                scale.y /= 2;
                                break;

                            case _3DLayout.SideBySide:
                                scale.x /= 2;
                                break;
                        }
                    }
                }

                if (location.locationType.Is360())
                    scale.x *= -1f;
                else if(location.scaleToFullscreen)
                    scale *= fullscreenMultiplier / Size;

                t.localScale = scale;
            }
        }
    }
}