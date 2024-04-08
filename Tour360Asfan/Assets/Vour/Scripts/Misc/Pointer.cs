using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    [System.Serializable]
    public enum PointerType
    {
        Laser,
        Gaze
    }

    public class Pointer : MonoBehaviour
    {
        [SerializeField] private PointerType pointerType;

        [SerializeField] private LineRenderer laser;
        [SerializeField] private SpriteRenderer gaze;

        public Color laserUnavailableColor = new Color(0.6f, 0.6f, 0.6f);
        public Color laserAvailableColor = Color.white;

        private Transform currentControllerT;

        private Transform camT;

        [Header("Gaze Settings")]
        [HideInInspector] public bool startedClick = false;
        private float startTime;
        private float startGazeSize;
        public float clickTime = 2;
        public float clickAnimDelay = 2;

        private IInteractable interactable;


        private void Start()
        {
            camT = GetComponentInParent<PlayerBase>().GetCamera().transform;
            startGazeSize = gaze.transform.localScale.x;

            SetPointerType(pointerType);
        }

        private void Update()
        {
            if (currentControllerT != null && pointerType == PointerType.Laser)
            {
                transform.SetPositionAndRotation(currentControllerT.position, currentControllerT.rotation);
            }
            else if(pointerType == PointerType.Gaze)
            {
                transform.SetPositionAndRotation(camT.position, camT.rotation);
                gaze.transform.LookAt(camT);
            }

            if(startedClick)
            {
                float timeDiff = Time.time - startTime;
                if(timeDiff > 1f)
                {
                    float size = (clickTime - timeDiff + 1f) / clickTime * startGazeSize; 
                    gaze.transform.localScale = new Vector3(size, size, 1f);

                    if (timeDiff - 1f > clickTime)
                    {
                        interactable.Interact();
                        startedClick = false;
                    }
                }
            }
            else
            {
                gaze.transform.localScale = new Vector3(startGazeSize, startGazeSize, 1f);
            }
        }

        public void SetController(Transform controller)
        {
            currentControllerT = controller;
        }

        public void SetPointerType(PointerType type)
        {
            pointerType = type;

            laser.enabled = type == PointerType.Laser;
            gaze.enabled = type == PointerType.Gaze;
        }

        public void SetPointer(Vector3 start, Vector3 end, bool hit)
        {
            laser.SetPosition(0, start);
            laser.SetPosition(1, end);
            gaze.transform.position = end;

            if (Mathf.RoundToInt(gaze.transform.localPosition.z) == 100)
                gaze.transform.localPosition = Vector3.forward * 5;

            laser.endColor = hit ? laserAvailableColor : laserUnavailableColor;
            gaze.color = hit ? laserAvailableColor : laserUnavailableColor;
        }

        public void StartClick(IInteractable interactable)
        {
            startedClick = true;
            startTime = Time.time;
            this.interactable = interactable;
        }

        public void CancelClick()
        {
            startedClick = false;
        }
    }
}
