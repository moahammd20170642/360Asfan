using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace CrizGames.Vour
{
    public class CanvasPointer : MonoBehaviour
    {
        private EventSystem eventSystem;
        private StandaloneInputModule inputModule;
        private VourUIInput input;

        private Transform camT;
        private Quaternion beforeButtonDownRot;

        public void SetupEventSystem()
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null)
                eventSystem = new GameObject("Event System").AddComponent<EventSystem>();

            inputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (inputModule == null)
                inputModule = eventSystem.gameObject.AddComponent<StandaloneInputModule>();

            input = eventSystem.GetComponent<VourUIInput>();
            if (input == null)
            {
                input = eventSystem.gameObject.AddComponent<VourUIInput>();
                Debug.Log("Added VourUIInput to " + eventSystem.name + " for UI interaction support in VR");
            }
            input.Init();

            // Create a camera ignoring pointers rotation
            Camera cam = GetComponentInChildren<Camera>();
            cam.enabled = false;
            camT = cam.transform;
            input.eventCamera = cam;
        }

        private void Update()
        {
            // Stop moving when controller button is down (workaround for dragging UI)
            if (input.GetMouseButtonDown(0))
                beforeButtonDownRot = camT.rotation;

            if (input.GetMouseButton(0))
                camT.rotation = beforeButtonDownRot;

            if (input.GetMouseButtonUp(0))
                camT.localEulerAngles = Vector3.zero;
        }

        public (bool hit, GameObject hitObj, Vector3 hitPoint) GetRaycastHit()
        {
            // Get pointer screen pos
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = inputModule.inputOverride.mousePosition;

            RaycastResult result = input.PerformRaycast(eventData, eventSystem);
            return (result.distance != 0f, result.gameObject, result.worldPosition);
        }
    }
}
