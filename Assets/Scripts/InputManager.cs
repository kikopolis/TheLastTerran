using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    private const float MAX_TIME_BETWEEN_DASH_BUTTON_PRESSES = 0.2f;

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
    private float sinceDashPress;
    public Vector2 move { get; set; }
    public Vector2 dash { get; set; }
    private Vector2 previousMoveDirection;
    private int pressCountForDash;
    private bool dashConsumed;
    public Vector2 look { get; set; }
    public bool walk { get; set; }
    public bool sprint { get; set; }
    public bool jump { get; set; }
    public bool crouch { get; set; }
    public bool interact { get; set; }
    public bool zoom { get; set; }

    private void Awake() {
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

    private void OnEnable() {
        currentActionMap.Enable();
    }

    private void OnDisable() {
        currentActionMap.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx) {
        var mDir = ctx.ReadValue<Vector2>();
        if (mDir.magnitude > 0f) {
            // if previous move direction is different, reset dash counters
            if (mDir != previousMoveDirection) {
                Debug.Log("reset dash counters, move dir is different");
                previousMoveDirection = mDir;
                pressCountForDash = 0;
                sinceDashPress = 0f;
                dashConsumed = false;
            }
            // if move direction is the same as previous and within time limit, increment dash counter
            if (mDir == previousMoveDirection && sinceDashPress < MAX_TIME_BETWEEN_DASH_BUTTON_PRESSES) {
                Debug.Log("increment dash counter, move dir is the same");
                pressCountForDash++;
                sinceDashPress = Time.time;
            }
            // if move direction is the same as previous and outside time limit, reset dash counter
            if (mDir == previousMoveDirection && sinceDashPress > MAX_TIME_BETWEEN_DASH_BUTTON_PRESSES) {
                Debug.Log("reset dash counter, move dir is the same but outside time limit");
                previousMoveDirection = mDir;
                pressCountForDash = 0;
                sinceDashPress = 0f;
            }
            // if dash counter is 2 and within timer, set dash direction
            if (pressCountForDash == 2 && !dashConsumed && sinceDashPress < MAX_TIME_BETWEEN_DASH_BUTTON_PRESSES) {
                Debug.Log("set dash direction");
                dash = mDir;
                dashConsumed = true;
                previousMoveDirection = Vector2.zero;
                sinceDashPress = 0f;
            }
        }

        move = mDir;
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