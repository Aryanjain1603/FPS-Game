using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviourPun
{
    public Animator animator;
    public PlayerController playerController;
    public PlayerInputHandler inputHandler;

    private Vector3 lastPosition;
    private float speed;
    public bool isAiming = false;
    public int aimLayerIndex = 1;
    

    private void Update()
    {
        if (!photonView.IsMine) return;

        UpdateMovementAnimations();
        UpdateStateAnimations();
        UpdateAimingAnimation();
    }
    private void UpdateAimingAnimation()
    {
        // Replace with your actual input logic
        isAiming = Input.GetMouseButton(1); // Right click to aim
        animator.SetBool("Aim", isAiming);
        
        // Set layer weight for aiming
        
        // animator.SetLayerWeight(aimLayerIndex, isAiming ? 1f : 0f);
        
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
    }
}