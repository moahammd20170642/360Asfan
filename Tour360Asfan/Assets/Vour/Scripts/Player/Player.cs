using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    public class Player : MonoBehaviour
    {
        public enum PlayerPlatformType
        {
            VR,
            WebVR,
            Desktop
        }

        public GameObject DesktopPlayer;
        public GameObject OculusPlayer;
        public GameObject WebXRPlayer;

        public bool CenterCamera = true;
        [Tooltip("The gaze pointer enables navigation without using VR controllers.")]
        public bool VRGazePointer = false;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            PlayerBase player = Instantiate(GetPlayerPrefab(), transform.position, transform.rotation).GetComponent<PlayerBase>();

            player.Init();
            player.SetCenterCam(CenterCamera);

            // Set pointer type
            switch (GetPlayerPlatform())
            {
                case PlayerPlatformType.VR:
                case PlayerPlatformType.WebVR:
                    player.GetComponent<VRPlayer>().pointer.SetPointerType(VRGazePointer ? PointerType.Gaze : PointerType.Laser);
                    break;
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// GetPlayerPrefab
        /// </summary>
        private GameObject GetPlayerPrefab()
        {
            switch (GetPlayerPlatform())
            {
                case PlayerPlatformType.VR:
                    Debug.Log("VR Mode (Oculus)");
                     return OculusPlayer;

                case PlayerPlatformType.WebVR:
                    Debug.Log("VR Mode (WebXR)");
                    return WebXRPlayer;

                case PlayerPlatformType.Desktop:
                default:
                    Debug.Log("PC Mode");
                    return DesktopPlayer;
            }
        }

        public static PlayerPlatformType GetPlayerPlatform()
        {
            if (Utils.InVR())
                return PlayerPlatformType.VR;

#if VOUR_WEBXR && UNITY_WEBGL
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return PlayerPlatformType.WebVR;
#endif
            return PlayerPlatformType.Desktop;
        }
    }
}