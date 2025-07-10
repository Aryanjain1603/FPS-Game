using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun, I_Damageable
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Health")]
    public int health = 100;

    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;

    public PlayerInputHandler input;
    private PlayerCamera playerCam;
    
    //Events ->
    public delegate void OnDamageEvent(int damage);
    public static event OnDamageEvent OnDamage;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
        playerCam = GetComponent<PlayerCamera>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            if (input != null) input.enabled = false;
            if (playerCam != null) playerCam.enabled = false;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        HandleMovement();
        playerCam.HandleLook(input.LookInput);
    }

    private void HandleMovement()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Move
        Vector2 moveInput = input.MoveInput;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (input.JumpTriggered && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage, int attackerActonNumber)
    {
        if (!photonView.IsMine) return;
        health -= damage;
        OnDamage?.Invoke(health);

        
        if (health <= 0)
        {
            Die(attackerActonNumber);
            // PhotonNetwork.Destroy(gameObject);
        }
    }

    private void Die(int attackerActonNumber)
    {
        ScoreManager.Instance.AddKill(attackerActonNumber);
        ReSpawn();
    }

    private void ReSpawn()
    {
        transform.position = Vector3.zero;
        health = 100;
        OnDamage?.Invoke(health);

    }
}
