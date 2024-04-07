using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[assembly: CrizGames.Vour.OptionalDependency("OVRInput", "VOUR_OCULUS")]

namespace CrizGames.Vour
{
    /// <summary>
    /// OculusPlayer
    /// </summary>
    public class OculusPlayer : VRPlayer
    {
#if VOUR_OCULUS
        private OVRInput.Controller currentController;

        /// <summary>
        /// InitCurrentController
        /// </summary>
        protected override void InitCurrentController()
        {
            currentController = OVRInput.Controller.RTouch;
            currentControllerT = rightControllerAnchor;
            VourUIInput.Instance.controller = currentController;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            base.Update();

            // Click and Interact
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, currentController) && !canvasInteractable)
                CurrentInteractable?.Interact();

            // Rotate via any available 2d axis control
            RotatePlayer(OVRInput.Get(OVRInput.Axis2D.Any).x);
        }

        /// <summary>
        /// UpdateCurrentController
        /// </summary>
        protected override void UpdateCurrentController()
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                currentController = OVRInput.Controller.RTouch;
                currentControllerT = rightControllerAnchor;
            }
            else if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                currentController = OVRInput.Controller.LTouch;
                currentControllerT = leftControllerAnchor;
            }
            VourUIInput.Instance.controller = currentController;
        }

        /// <summary>
        /// VibrateController
        /// </summary>
        protected override void VibrateController(float intensity, float time)
        {
            VibrateController(1f, intensity, time);
        }

        /// <summary>
        /// VibrateController
        /// </summary>
        protected void VibrateController(float frequency, float amplitude, float time)
        {
            ResetVibration();

            OVRInput.SetControllerVibration(frequency, amplitude, currentController);

            Invoke("ResetVibration", time);
        }

        /// <summary>
        /// ResetVibration
        /// </summary>
        private void ResetVibration()
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.All);
        }
#else
        protected override void InitCurrentController() { }
        protected override void UpdateCurrentController() { }
        protected override void VibrateController(float intensity, float time) { }
#endif
    }
}