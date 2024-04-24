using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CrizGames.Vour
{
    public abstract class VRPlayer : PlayerBase
    {
        [Header("Hands & Controllers")]
        public Transform leftControllerAnchor;
        public Transform rightControllerAnchor;
        public Transform leftHandAnchor;
        public Transform rightHandAnchor;

        [Header("Pointer Settings")]
        public Pointer pointer;
        protected CanvasPointer canvasPointer;
        protected bool canvasInteractable = false;
        protected GameObject lastCanvasHit;

        private Transform _currentControllerT;
        protected Transform currentControllerT { 
            get => _currentControllerT; 
            set
            {
                pointer.SetController(value);
                _currentControllerT = value;
            }
        }

        protected bool rotated = false;


        /// <summary>
        /// Init
        /// </summary>
        public override void Init()
        {
            base.Init();

            canvasPointer = pointer.GetComponent<CanvasPointer>();
            canvasPointer.SetupEventSystem();

            InitCurrentController();
        }

        /// <summary>
        /// InitCurrentController
        /// </summary>
        protected abstract void InitCurrentController();

        /// <summary>
        /// UpdateCurrentController
        /// </summary>
        protected abstract void UpdateCurrentController();

        /// <summary>
        /// VibrateController
        /// </summary>
        protected abstract void VibrateController(float intensity, float time);

        /// <summary>
        /// Update
        /// </summary>
        protected virtual void Update()
        {
            if (EventSystem.current != null)
                pointerOverUI = EventSystem.current.IsPointerOverGameObject();

            UpdateCurrentController();

            GetInteractable();

            if (centerCam)
                transform.position -= cam.transform.position;
        }

        /// <summary>
        /// GetInteractable
        /// </summary>
        private void GetInteractable()
        {
            IInteractable i = RaycastInteractable(pointer.transform.position, pointer.transform.forward);
            bool hitInteractable = i != null;

            (bool hitCanvas, GameObject hitObj, Vector3 canvasPointerPos) = canvasPointer.GetRaycastHit();
            bool uiObjInteractable = hitObj != null && hitObj.GetComponent<Selectable>() != null;

            bool canvasCloser = false;

            // Check if canvas or interactable is closer
            if (hitCanvas && hitInteractable)
            {
                float iDist = Vector3.Distance(pointer.transform.position, pointerPos);
                float canvasDist = Vector3.Distance(pointer.transform.position, canvasPointerPos);

                canvasCloser = canvasDist < iDist;
            }
            else if (hitCanvas && !hitInteractable)
                canvasCloser = true;

            if (canvasCloser)
                pointerPos = canvasPointerPos;

            if (canvasInteractable != (canvasCloser && uiObjInteractable) ||
                (!canvasCloser && i != CurrentInteractable) ||
                (canvasCloser && uiObjInteractable && lastCanvasHit != hitObj))
                VibrateController(0.2f, 0.1f);

            canvasInteractable = canvasCloser && uiObjInteractable;

            lastCanvasHit = hitObj;

            UpdateInteractable(pointerOverUI ? null : i);

            UpdatePointer();
        }

        protected override void UpdateInteractable(IInteractable i)
        {
            if (i != CurrentInteractable)
                pointer.CancelClick();
            else if (!pointer.startedClick && i != null)
                pointer.StartClick(i);

            base.UpdateInteractable(i);
        }

        /// <summary>
        /// UpdatePointer
        /// </summary>
        private void UpdatePointer()
        {
            if (currentControllerT)
                pointer.SetPointer(currentControllerT.position, pointerPos, CurrentInteractable != null || canvasInteractable);
            else
                pointer.SetPointer(Vector3.zero, pointerPos, CurrentInteractable != null || canvasInteractable);
        }

        /// <summary>
        /// RotatePlayer
        /// </summary>
        protected virtual void RotatePlayer(float thumbstickX)
        {
            if (Mathf.Abs(thumbstickX) > 0.7f && !rotated)
            {
                transform.eulerAngles += Vector3.up * 45f * (thumbstickX > 0 ? 1f : -1f);
                rotated = true;
            }
            else if (Mathf.Abs(thumbstickX) < 0.4f && rotated)
                rotated = false;
        }

        /// <summary>
        /// ResetRotation
        /// </summary>
        public override void ResetRotation()
        {
            transform.eulerAngles = Vector3.up * -GetCamera().transform.localEulerAngles.y;
        }
    }
}