using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private PlayerInputHandler input;
    private PlayerCamera playerCam;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
        playerCam = GetComponent<PlayerCamera>();
    }

    void Update()
    {
        HandleMovement();
        playerCam.HandleLook(input.LookInput);
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Movement
        Vector3 move = transform.right * input.MoveInput.x + transform.forward * input.MoveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (input.JumpTriggered && isGrounded)
            velocity.y = jumpForce;

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}