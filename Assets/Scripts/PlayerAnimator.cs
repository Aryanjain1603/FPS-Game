using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviourPun
{
    private Animator animator;
    private PlayerController playerController;
    private PlayerInputHandler inputHandler;

    private Vector3 lastPosition;
    private float speed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        UpdateMovementAnimations();
        UpdateStateAnimations();
    }

    private void UpdateMovementAnimations()
    {
        // Calculate speed (used for "Forward" parameter)
        Vector3 flatMovement = transform.position - lastPosition;
        flatMovement.y = 0;
        speed = flatMovement.magnitude / Time.deltaTime;
        lastPosition = transform.position;

        animator.SetFloat("Forward", speed);

        // Optional: Turn could be driven by input or camera look
        animator.SetFloat("Turn", inputHandler.MoveInput.x);
    }

    private void UpdateStateAnimations()
    {
        // Crouch logic (extend this with actual crouch input handling if added)
        animator.SetBool("Crouch", Input.GetKey(KeyCode.LeftControl));

        // Grounded check (from PlayerController)
        animator.SetBool("OnGround", IsGrounded());

        // Jump parameter (1 when jumping, 0 otherwise)
        animator.SetFloat("Jump", playerController.input.JumpTriggered && IsGrounded() ? 1f : 0f);
    }

    private bool IsGrounded()
    {
        // Use the same logic as PlayerController
        return playerController.isGrounded;
        return Physics.CheckSphere(playerController.groundCheck.position, playerController.groundDistance, playerController.groundMask);
    }
}