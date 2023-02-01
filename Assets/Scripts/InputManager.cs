using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    [ Header("Player Input") ]
    [ Tooltip("The PlayerInput Unity Component that contains all the input action maps for the player.") ]
    [ SerializeField ]
    private PlayerInput playerInput;
    [ Header("Switch input actions to toggle") ]
    [ SerializeField ]
    private bool toggleWalk;
    [ SerializeField ]
    private bool toggleSprint;
    [ SerializeField ]
    private bool toggleCrouch;
    [ SerializeField ]
    private bool toggleZoom;

    private InputActionMap currentActionMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction walkAction;
    private InputAction sprintAction;
    private InputAction dashAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    private InputAction zoomAction;
    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool walk { get; private set; }
    public bool sprint { get; private set; }
    public bool dash { get; private set; }
    public bool jump { get; private set; }

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentActionMap = playerInput.currentActionMap;
        moveAction = currentActionMap.FindAction("Move");
        lookAction = currentActionMap.FindAction("Look");
        walkAction = currentActionMap.FindAction("Walk");
        sprintAction = currentActionMap.FindAction("Sprint");
        jumpAction = currentActionMap.FindAction("Jump");
        crouchAction = currentActionMap.FindAction("Crouch");
        interactAction = currentActionMap.FindAction("Interact");
        zoomAction = currentActionMap.FindAction("Zoom");

        moveAction.performed += OnMove;
        lookAction.performed += OnLook;
        walkAction.performed += OnWalk;
        sprintAction.performed += OnSprint;
        jumpAction.performed += OnJump;
        crouchAction.performed += OnCrouch;
        interactAction.performed += OnInteract;
        zoomAction.performed += OnZoom;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        walkAction.canceled += OnWalk;
        sprintAction.canceled += OnSprint;
        jumpAction.canceled += OnJump;
        crouchAction.canceled += OnCrouch;
        interactAction.canceled += OnInteract;
        zoomAction.canceled += OnZoom;
    }

    public bool crouch { get; private set; }

    public bool interact { get; private set; }

    public bool zoom { get; private set; }

    private void OnEnable() {
        currentActionMap.Enable();
    }

    private void OnDisable() {
        currentActionMap.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx) {
        // set dash to true when the player presses the corresponding direction twice in quick succession
        move = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx) {
        look = ctx.ReadValue<Vector2>();
    }

    private void OnWalk(InputAction.CallbackContext ctx) {
        // if toggle is enabled, toggle walk
        if (toggleWalk && ctx.performed && ctx.ReadValueAsButton()) {
            walk = !walk;
        } else {
            walk = ctx.ReadValueAsButton();
        }
    }

    private void OnSprint(InputAction.CallbackContext ctx) {
        // if toggle is enabled, toggle sprint
        if (toggleSprint && ctx.performed && ctx.ReadValueAsButton()) {
            sprint = !sprint;
        } else {
            sprint = ctx.ReadValueAsButton();
        }
    }

    private void OnJump(InputAction.CallbackContext ctx) {
        jump = ctx.ReadValueAsButton();
    }

    private void OnCrouch(InputAction.CallbackContext ctx) {
        // if toggle is enabled, toggle crouch
        if (toggleCrouch && ctx.performed && ctx.ReadValueAsButton()) {
            crouch = !crouch;
        } else {
            crouch = ctx.ReadValueAsButton();
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx) {
        interact = ctx.ReadValueAsButton();
    }

    private void OnZoom(InputAction.CallbackContext ctx) {
        // if toggle is enabled, toggle zoom
        if (toggleZoom && ctx.performed && ctx.ReadValueAsButton()) {
            zoom = !zoom;
        } else {
            zoom = ctx.ReadValueAsButton();
        }
    }
}