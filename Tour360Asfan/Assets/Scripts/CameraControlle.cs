using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControlle : MonoBehaviour
{

    public float zoomSpeedscroll = 15; // Adjust this to control the speed of zooming
    public float minFOV = 40;   // Minimum field of view
    public float maxFOV = 60;   // Maximum field of view


    private float zoomSpeed = 600f;
    private float zoomAmount = 0.0f;
    private float rotationspeed = 100;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Adjust the field of view based on the scroll input
        float newFOV = Camera.main.fieldOfView - (scroll * zoomSpeedscroll);

        // Clamp the new FOV to the specified range
        newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);

        // Apply the new FOV to the camera
        Camera.main.fieldOfView = newFOV;

        //if (Input.GetMouseButton(1) || Input.GetMouseButtonUp(2))
        //{


        //    zoomAmount = Mathf.Clamp(zoomAmount + Input.GetAxis("Mouse Y") * Time.deltaTime * zoomSpeed, -5.0f, 5.0f);
        //    Camera.main.transform.localPosition = new Vector3(0, 0, zoomAmount);
        //}

        if (Input.GetMouseButton(0))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + Input.GetAxis("Mouse Y") * Time.deltaTime * rotationspeed, transform.localEulerAngles.y + -1*Input.GetAxis("Mouse X") * Time.deltaTime * rotationspeed, 0);

        }
    }
}
