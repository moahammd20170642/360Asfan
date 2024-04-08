using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VOUR_WEBXR
using WebXR;
#endif

[assembly: CrizGames.Vour.OptionalDependency("WebXR.WebXRSubsystem", "VOUR_WEBXR")]
[assembly: CrizGames.Vour.OptionalDependency("WebXRInputProfile.InputProfileLoader", "WEBXR_INPUT_PROFILES")]

namespace CrizGames.Vour
{
    public class WebXRPlayer : VRPlayer
    {
        [Header("Cameras")]
        [SerializeField] private Camera cameraMain;
        [SerializeField] private Camera cameraL;
        [SerializeField] private Camera cameraR;
        private Rect leftRect, rightRect;
        private int viewsCount = 1;

        private DesktopPlayer pcPlayer;

#if VOUR_WEBXR
        private WebXRController leftController;
        private WebXRController rightController;
        private WebXRController currentController;

        private WebXRState xrState = WebXRState.NORMAL;
        private WebXRState prevXRState = WebXRState.NORMAL;

        /// <summary>
        /// Init
        /// </summary>
        public override void Init()
        {
            base.Init();

            cam = cameraL;

            pcPlayer = GetComponent<DesktopPlayer>();
            pcPlayer.Init();

            if (WebXRManager.Instance.subsystem != null)
                xrState = WebXRManager.Instance.XRState;

#if UNITY_EDITOR
            if (Utils.InVR())
                xrState = WebXRState.VR;
#endif

            SwitchXRState();
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            WebXRManager.OnXRChange += OnXRChange;
            WebXRManager.OnHeadsetUpdate += OnHeadsetUpdate;
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            WebXRManager.OnXRChange -= OnXRChange;
            WebXRManager.OnHeadsetUpdate -= OnHeadsetUpdate;
        }

        /// <summary>
        /// SwitchXRState
        /// </summary>
        private void SwitchXRState()
        {
            switch (xrState)
            {
                case WebXRState.AR:
                    Debug.LogError("AR is not supported by Vour.");
                    break;

                case WebXRState.VR:
                    cameraMain.enabled = false;
#if UNITY_EDITOR
                    cameraL.enabled = true;
                    cameraR.enabled = true;
#else
                    // In WebGL, the viewports have to be adjusted manually
                    cameraL.enabled = viewsCount > 0;
                    cameraL.rect = leftRect;
                    cameraR.enabled = viewsCount > 1;
                    cameraR.rect = rightRect;
#endif
                    pointer.gameObject.SetActive(true);

                    // Reinitialize so that it is set as the inputOverride
                    VourUIInput.Instance.Init();
                    break;

                case WebXRState.NORMAL:
                    cameraMain.enabled = true;
                    cameraL.enabled = false;
                    cameraR.enabled = false;

                    pointer.gameObject.SetActive(false);

                    pcPlayer.SetCenterCam(pcPlayer.CenterCamera);

                    if (prevXRState == WebXRState.VR)
                        VourUIInput.Instance.ReturnToPCInput(pcPlayer.cam);
                    break;
            }
        }

        /// <summary>
        /// OnXRChange
        /// </summary>
        private void OnXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            Debug.Log($"XR state changed from {xrState} to {state}");
            //Debug.Log("Views Count: " + viewsCount);   // VR = 2; NORMAL = 1
            //Debug.Log("Left Eye Rect: " + leftRect);   // VR = (  0, 0, 0.5, 1); NORMAL = (0, 0, 0, 0)
            //Debug.Log("Right Eye Rect: " + rightRect); // VR = (0.5, 0, 0.5, 1); NORMAL = (0, 0, 0, 0)

            this.viewsCount = viewsCount;
            this.leftRect = leftRect;
            this.rightRect = rightRect;

            prevXRState = xrState;
            xrState = state;
            SwitchXRState();
        }

        /// <summary>
        /// OnHeadsetUpdate
        /// </summary>
        private void OnHeadsetUpdate(
            Matrix4x4 leftProjectionMatrix, Matrix4x4 rightProjectionMatrix,
            Quaternion leftRotation, Quaternion rightRotation,
            Vector3 leftPosition, Vector3 rightPosition)
        {
            if (xrState == WebXRState.VR)
            {
#if UNITY_EDITOR
                // Eye positions have a difference of ~0.04m
                Vector3 pos = (leftPosition + rightPosition) / 2f;

                cameraL.transform.localPosition = pos;
                cameraR.transform.localPosition = pos;
#else
                // In WebGL, the eye distance has to be set manually
                cameraL.transform.localPosition = leftPosition;
                cameraR.transform.localPosition = rightPosition;
#endif
                cameraL.transform.localRotation = leftRotation;
                cameraR.transform.localRotation = rightRotation;
                cameraL.projectionMatrix = leftProjectionMatrix;
                cameraR.projectionMatrix = rightProjectionMatrix;
            }
        }

        /// <summary>
        /// InitCurrentController
        /// </summary>
        protected override void InitCurrentController()
        {
            leftController = leftControllerAnchor.GetComponent<WebXRController>();
            rightController = rightControllerAnchor.GetComponent<WebXRController>();

            currentController = rightController;
            currentControllerT = rightControllerAnchor;

            VourUIInput.Instance.webController = currentController;
        }

        /// <summary>
        /// InteractButtonDown
        /// </summary>
        private bool InteractButtonDown(WebXRController controller)
        {
            return controller.GetAnyButtonDown(WebControllerButtons.Trigger | WebControllerButtons.Grip | WebControllerButtons.ButtonA);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (xrState == WebXRState.NORMAL)
                return;

            base.Update();

            if (InteractButtonDown(currentController) && !canvasInteractable)
                CurrentInteractable?.Interact();

            // Rotate via thumbstick
            RotatePlayer(currentController.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick).x);
        }

        /// <summary>
        /// UpdateCurrentController
        /// </summary>
        protected override void UpdateCurrentController()
        {
            if (InteractButtonDown(rightController))
            {
                currentController = rightController;
                currentControllerT = rightControllerAnchor;
            }
            else if (InteractButtonDown(leftController))
            {
                currentController = leftController;
                currentControllerT = leftControllerAnchor;
            }
            VourUIInput.Instance.webController = currentController;
        }

        /// <summary>
        /// VibrateController
        /// </summary>
        protected override void VibrateController(float intensity, float time)
        {
            currentController.Pulse(intensity, time);
        }

        /// <summary>
        /// GetCamera
        /// </summary>
        public override Camera GetCamera()
        {
            return cameraMain;
        }
#else
        protected override void InitCurrentController() { }
        protected override void UpdateCurrentController() { }
        protected override void VibrateController(float intensity, float time) { }
#endif
    }
}
