using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
   
    public float rotationSpeed = 5.0f;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationX = mouseDelta.y * rotationSpeed * Time.deltaTime;
            float rotationY = -mouseDelta.x * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, rotationY, Space.World);
            transform.Rotate(Vector3.right, rotationX, Space.World);
        }

        lastMousePosition = Input.mousePosition;
    }
}
