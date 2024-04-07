using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    /// <summary>
    /// Player Base
    /// </summary>
    public abstract class PlayerBase : MonoBehaviour
    {
        public static PlayerBase Instance;

        protected IInteractable CurrentInteractable;

        [HideInInspector] public Vector3 pointerPos;

        [HideInInspector] public Camera cam;

        [Header("Player Settings")]
        [SerializeField] private protected bool centerCam = true;
        public bool CenterCamera { get { return centerCam; } }

        private protected float startYPos;
        public float yOffset;

        protected bool initialized = false;

        protected bool pointerOverUI = false;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// Init
        /// </summary>
        public virtual void Init()
        {
            if (initialized)
                return;

            cam = GetComponentInChildren<Camera>();

            startYPos = transform.position.y;

            Instance = this;
            initialized = true;
        }

        /// <summary>
        /// RaycastInteractable
        /// </summary>
        public virtual IInteractable RaycastInteractable(Vector3 pos, Vector3 dir)
        {
            if (pointerOverUI)
                return null;

            if (Physics.Raycast(pos, dir, out RaycastHit hit))
            {
                pointerPos = hit.point;
                return hit.collider.GetComponentInParent<IInteractable>();
            }

            pointerPos = pos + dir * 100f;
            return null;
        }


        /// <summary>
        /// UpdateInteractable
        /// </summary>
        protected virtual void UpdateInteractable(IInteractable i)
        {
            if (i != null)
            {
                if (i != null && i != CurrentInteractable)
                    i.OnPointerOverObjEnter();

                if (CurrentInteractable != null && i != CurrentInteractable)
                    CurrentInteractable?.OnPointerOverObjExit();
            }
            else
            {
                CurrentInteractable?.OnPointerOverObjExit();
            }
            CurrentInteractable = i;
        }

        /// <summary>
        /// ResetRotation
        /// </summary>
        public abstract void ResetRotation();

        /// <summary>
        /// SetCenterCam
        /// </summary>
        public virtual void SetCenterCam(bool center)
        {
            centerCam = center;

            if (center)
                cam.transform.position = Vector3.zero;
            else
                transform.position = new Vector3(transform.position.x, startYPos + yOffset, transform.position.z);
        }

        /// <summary>
        /// GetCamera
        /// </summary>
        public virtual Camera GetCamera()
        {
            return GetComponentInChildren<Camera>();
        }

        protected virtual void OnDestroy()
        {
            if(Instance == this)
                Instance = null;
        }
    }
}