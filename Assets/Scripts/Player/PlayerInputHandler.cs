using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    
    public bool InteractTriggered { get; private set; }
    
    public bool SprintTriggered { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => JumpTriggered = true;
        
        inputActions.Player.Interact.performed += ctx => InteractTriggered = true;
        
        inputActions.Player.Sprint.performed += ctx => SprintTriggered = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void LateUpdate()
    {
        JumpTriggered = false; // Reset jump after read
        InteractTriggered = false; // Reset interact after read
        SprintTriggered = false; // Reset sprint after read
    }
}