using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
#if VOUR_WEBXR
using WebXR;
#endif
using static CrizGames.Vour.Location;

[assembly: CrizGames.Vour.OptionalDependency("UnityEngine.XR.XRDisplaySubsystem", "VOUR_XR")]

namespace CrizGames.Vour
{
#if VOUR_WEBXR
    [Flags]
    public enum WebControllerButtons
    {
        ButtonA = WebXRController.ButtonTypes.ButtonA,
        ButtonB = WebXRController.ButtonTypes.ButtonB,
        Grip = WebXRController.ButtonTypes.Grip,
        Thumbstick = WebXRController.ButtonTypes.Thumbstick,
        Touchpad = WebXRController.ButtonTypes.Touchpad,
        Trigger = WebXRController.ButtonTypes.Trigger
    }
#endif

    public static class Utils
    {
        public static bool InVR()
        {
#if VOUR_XR
            List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(displaySubsystems);

            // If there are xr displays detected = VR is on
            return displaySubsystems.Count > 0;
#else
            return false;
#endif
        }


#if VOUR_WEBXR
        private static WebXRController.ButtonTypes[] GetButtonsFromFlags(WebControllerButtons flags)
        {
            return Enum.GetValues(flags.GetType()).Cast<Enum>().Where(flags.HasFlag).Cast<WebXRController.ButtonTypes>().ToArray();
        }

        /// <summary>
        /// Returns true if any one of the specified buttons are pressed.
        /// </summary>
        public static bool GetAnyButton(this WebXRController controller, WebControllerButtons flags)
        {
            foreach (var value in GetButtonsFromFlags(flags))
                if (controller.GetButton(value))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if any one of the specified buttons were pressed down this frame.
        /// </summary>
        public static bool GetAnyButtonDown(this WebXRController controller, WebControllerButtons flags)
        {
            foreach (var value in GetButtonsFromFlags(flags))
                if (controller.GetButtonDown(value))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if any one of the specified buttons were pressed up this frame.
        /// </summary>
        public static bool GetAnyButtonUp(this WebXRController controller, WebControllerButtons flags)
        {
            foreach (var value in GetButtonsFromFlags(flags))
                if (controller.GetButtonUp(value))
                    return true;
            return false;
        }
#endif

        /// <summary>
        /// Is it some kind of image location?
        /// </summary>
        public static bool IsImage(this LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Image:
                case LocationType.Image3D:
                case LocationType.Image360:
                case LocationType.Image3D360:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Is it some kind of video location?
        /// </summary>
        public static bool IsVideo(this LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Video:
                case LocationType.Video3D:
                case LocationType.Video360:
                case LocationType.Video3D360:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Is it some kind of 360 location?
        /// </summary>
        public static bool Is360(this LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Image360:
                case LocationType.Image3D360:
                case LocationType.Video360:
                case LocationType.Video3D360:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Is it some kind of 3D location?
        /// </summary>
        public static bool Is3D(this LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Image3D:
                case LocationType.Image3D360:
                case LocationType.Video3D:
                case LocationType.Video3D360:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Is it some kind of VR player?
        /// </summary>
        public static bool IsAnyVR(this Player.PlayerPlatformType platform)
        {
            switch (platform)
            {
                case Player.PlayerPlatformType.VR:
                case Player.PlayerPlatformType.WebVR:
                    return true;
                case Player.PlayerPlatformType.Desktop:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Find child of transform by tag
        /// </summary>
        public static Transform FindByTag(this Transform parent, string tag)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.CompareTag(tag))
                    return child;
            }
            return null;
        }
    }
}
