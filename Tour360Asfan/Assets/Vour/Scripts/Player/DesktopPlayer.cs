using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if VOUR_WEBXR
using WebXR;
#endif

namespace CrizGames.Vour
{
    /// <summary>
    /// DesktopPlayer
    /// </summary>
    public class DesktopPlayer : PlayerBase
    {
        public float MouseSensitivity = 50;
        public bool canMoveCam = true;

        private Vector2 startPos;

        private Vector3 camRot = Vector3.zero;


        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
#if VOUR_WEBXR
            if (WebXRManager.Instance != null && WebXRManager.Instance.subsystem != null && WebXRManager.Instance.XRState != WebXRState.NORMAL)
                return;
#endif
            if (EventSystem.current != null)
                pointerOverUI = EventSystem.current.IsPointerOverGameObject();

            // Get Interactable
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            IInteractable i = RaycastInteractable(ray.origin, ray.direction);
            UpdateInteractable(i);

            // Click and Interact
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                startPos = cam.ScreenToViewportPoint(Input.mousePosition);

            if (IsClick())
            {
                if (Input.GetMouseButtonUp(0))
                    i?.Interact();
            }
            // Look
            else
            {
                Look();
            }
        }

        /// <summary>
        /// IsClick
        /// </summary>
        private bool IsClick()
        {
            return Vector2.Distance(startPos, cam.ScreenToViewportPoint(Input.mousePosition)) < 0.02f;
        }

        /// <summary>
        /// Look
        /// </summary>
        private void Look()
        {
            if (!canMoveCam)
                return;

#if UNITY_ANDROID || UNITY_IOS
            if(Input.GetMouseButton(0) || Input.GetMouseButton(2) || Input.GetMouseButton(1))
#else
            if(Input.GetMouseButton(2) || Input.GetMouseButton(1))
#endif
            {
                Vector3 lookVec = new Vector3(Input.GetAxisRaw("Mouse Y"), -Input.GetAxisRaw("Mouse X")) / Screen.width * 100f;
                camRot += lookVec * MouseSensitivity;
                camRot.x = Mathf.Clamp(camRot.x, -90f, 90f);
                cam.transform.eulerAngles = camRot;
            }
        }

        /// <summary>
        /// ResetRotation
        /// </summary>
        public override void ResetRotation()
        {
            cam.transform.eulerAngles = camRot = Vector3.zero;
        }
    }
}