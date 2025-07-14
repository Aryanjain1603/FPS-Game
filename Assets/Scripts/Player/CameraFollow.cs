using Player;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform cameraTransform; // Assign the camera here
    public float sensitivity = 2f;
    public float verticalClamp = 80f;

    private float yaw;
    private float pitch;
    
    void Start()
    {
        
    }

    public void HandleLook(Vector2 lookInput)
    {
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);

        // Rotate camera pitch (up/down)
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        // Rotate player body yaw (left/right)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}