using UnityEngine;

public class ObjectViewer : MonoBehaviour
{
    // Speed of rotation
    public float rotationSpeed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        // Get mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate the object based on mouse movement
        transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
        transform.Rotate(Vector3.right, mouseY * rotationSpeed, Space.World);
    }
}
