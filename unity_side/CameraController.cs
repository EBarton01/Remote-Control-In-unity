using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 2.0f;

    private float rotationX = 0.0f;

    void Update()
    {
        // Movement controls (similar to what you already have)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput);
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime);

        // Rotate the camera only when the middle mouse button is clicked
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate the camera horizontally (yaw)
            transform.Rotate(Vector3.up * mouseX * rotationSpeed);

            // Rotate the camera vertically (pitch), clamping the angle to avoid flipping
            rotationX -= mouseY * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        }

        // Add ability to look left and right using 'Q' and 'E' keys
        float rotationY = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotationY = -1.0f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationY = 1.0f;
        }

        transform.Rotate(Vector3.up * rotationY * rotationSpeed);
    }
}
