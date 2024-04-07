using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if VOUR_WEBXR
using WebXR;
#endif

namespace CrizGames.Vour
{
    public class VourUIInput : BaseInput
    {
        public static VourUIInput Instance;

        private Camera cam;
        public Camera eventCamera
        {
            get { return cam; }
            set
            {
                cam = value;

                foreach (var c in canvases)
                    c.worldCamera = cam;
            }
        }

        private Canvas[] canvases;

#if VOUR_OCULUS
        public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
        public OVRInput.Controller controller = OVRInput.Controller.All;
#endif
#if VOUR_WEBXR
        public WebControllerButtons webClickButton = WebControllerButtons.Trigger | WebControllerButtons.Grip | WebControllerButtons.ButtonA;
        public WebXRController webController;
#endif

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

            // Don't initialize when not in VR
            // Also don't destroy component for later use when switching to VR
            if (!Utils.InVR())
                return;

            Init();
        }

        public void Init()
        {
            BaseInputModule module = GetComponent<BaseInputModule>();
            if (module != null)
                module.inputOverride = this;
            else
                gameObject.AddComponent<BaseInputModule>().inputOverride = this;

            if (canvases == null || canvases.Length == 0)
                canvases = FindObjectsOfType<Canvas>(true);
        }

        public void ReturnToPCInput(Camera pcCam)
        {
            foreach (var c in canvases)
                c.worldCamera = pcCam;

            GetComponent<BaseInputModule>().inputOverride = null;
        }

        public RaycastResult PerformRaycast(PointerEventData eventData, EventSystem eventSystem)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(eventData, results);
            return FindFirstRaycast(results);
        }

        // From UnityEngine.EventSystems.BaseInputModule
        private RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (int i = 0; i < candidates.Count; i++)
                if (candidates[i].gameObject != null)
                    return candidates[i];

            return default;
        }

        public override bool GetMouseButton(int button)
        {
#if VOUR_OCULUS
            return OVRInput.Get(clickButton, controller);
#elif VOUR_WEBXR
        return webController.GetAnyButton(webClickButton);
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return false;
        }

        public override bool GetMouseButtonDown(int button)
        {
#if VOUR_OCULUS
            return OVRInput.GetDown(clickButton, controller);
#elif VOUR_WEBXR
        return webController.GetAnyButtonDown(webClickButton);
#endif
            return false;
        }

        public override bool GetMouseButtonUp(int button)
        {
#if VOUR_OCULUS
            return OVRInput.GetUp(clickButton, controller);
#elif VOUR_WEBXR
        return webController.GetAnyButtonUp(webClickButton);
#endif
            return false;
#pragma warning restore CS0162 // Unreachable code detected
        }

        public override Vector2 mousePosition
        {
            get
            {
                if (cam != null)
                    return cam.WorldToScreenPoint(cam.transform.position + cam.transform.parent.forward);
                return Vector2.zero;
            }
        }

#if VOUR_WEBXR && UNITY_WEBGL
        public override bool mousePresent => cam != null && webController != null;
#else
        public override bool mousePresent => cam != null;
#endif
    }
}