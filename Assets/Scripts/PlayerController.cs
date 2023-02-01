using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /// <summary>
    /// Disable this to prevent input actions during certain actions like vaulting.
    /// </summary>
    private bool canAcceptInput = true;

    [ Header("Camera") ]
    [ SerializeField ]
    private Camera playerCamera;
    [ Tooltip("This is where the camera will move to on each frame. Apply effects to this transform and not directly to the camera.") ]
    [ SerializeField ]
    private Transform cameraRoot;

    [ Header("Layers") ]
    [ SerializeField ]
    private LayerMask groundLayer;
    [ SerializeField ]
    private LayerMask ledgeLayer;

    [ Header("Physics") ]
    [ SerializeField ]
    private Rigidbody rb;
    [ SerializeField ]
    private float mass = 54f;
    [ SerializeField ]
    private float groundTolerance = 0.1f;
    [ SerializeField ]
    private float gravity = -9.81f;
    [ SerializeField ]
    private float airDrag = 0.8f;
    [ SerializeField ]
    private float standingHeight = 1f;
    [ SerializeField ]
    private float crouchHeight = 0.5f;
    [ SerializeField ]
    private float playerRadius = 0.5f;
    [ SerializeField ]
    private float acceleration = 9f;
    [ SerializeField ]
    private bool isGrounded;
    private float doubleGravity;
    private float gravityTimesMass;
    private float doubleGravityTimesMass;

    [ Header("Features") ]
    [ SerializeField ]
    private bool gravityEnabled = true;
    [ SerializeField ]
    private bool moveEnabled = true;
    [ SerializeField ]
    private bool mouseLookEnabled = true;
    [ SerializeField ]
    private bool walkEnabled = true;
    [ SerializeField ]
    private bool sprintEnabled = true;
    [ SerializeField ]
    private bool dashEnabled = true;
    [ SerializeField ]
    private bool vaultingToLedgesEnabled = true;
    [ SerializeField ]
    private bool jumpEnabled = true;
    [ SerializeField ]
    private bool crouchEnabled = true;
    [ SerializeField ]
    private bool interactEnabled = true;
    [ SerializeField ]
    private bool zoomEnabled = true;
    [ SerializeField ]
    private bool healthEnabled = true;
    [ SerializeField ]
    private bool staminaEnabled = true;
    [ SerializeField ]
    private bool manaEnabled = true;
    [ SerializeField ]
    private bool hungerEnabled = true;
    [ SerializeField ]
    private bool thirstEnabled = true;
    [ SerializeField ]
    private bool temperatureEnabled = true;
    [ SerializeField ]
    private bool oxygenEnabled = true;
    [ SerializeField ]
    private bool radiationEnabled = true;
    [ SerializeField ]
    private bool inventoryEnabled = true;

    [ Header("Controls") ]
    [ Tooltip("The InputManager class that handles all input processing.") ]
    [ SerializeField ]
    private InputManager inputManager;

    [ Header("Movement") ]
    [ SerializeField ]
    private float runSpeed = 5f;
    [ SerializeField ]
    private float dashSpeed = 12f;
    [ SerializeField ]
    private float dashDistance = 3f;
    [ SerializeField ]
    private float walkSpeed = 2f;
    [ SerializeField ]
    private float sprintSpeed = 9f;
    [ SerializeField ]
    private float crouchSpeed = 1f;
    [ SerializeField ]
    private float jumpHeight = 3f;
    [ SerializeField ]
    private float currentSpeed;
    [ SerializeField ]
    private Vector3 currentVelocity;
    [ SerializeField ]
    private float vaultHeight = 0.6f;
    private float crouchTimer;
    private float shouldBeCrouching;
    private bool isCrouching;
    private bool isDashing;
    private bool isJumping;
    private bool isVaultingToLedge;
    private bool isSliding;
    private Coroutine dashRoutine;
    private Coroutine vaultToLedgeRoutine;
    [ SerializeField ]
    private bool allowJumpWhileSliding = true;
    [ SerializeField ]
    private float jumpCooldown = 0.25f;
    private float timeSinceLastJump;
    private float timeSinceLastLanding;
    private bool jumpRequested;
    private bool jumpConsumed;
    private bool jumpedThisFrame;
    private float timeSinceJumpRequested;

    [ Header("Mouse Look") ]
    [ SerializeField ]
    private float lookDownLimit = -40f;
    [ SerializeField ]
    private float lookUpLimit = 70f;
    [ SerializeField ]
    private float mouseSensitivity = 30f;
    private float xRotation;

    [ Header("Zoom and Aim") ]
    [ SerializeField ]
    private float timeToZoom = 0.15f;
    [ SerializeField ]
    private float zoomFOV = 20f;
    [ SerializeField ]
    private float defaultFov = 60f;
    private Coroutine zoomRoutine;

    private void Awake() {
        defaultFov = playerCamera.fieldOfView;
        doubleGravity = gravity * 2;
        gravityTimesMass = gravity * mass;
        doubleGravityTimesMass = doubleGravity * mass;
    }

    private void FixedUpdate() {
        HandleInputs();
        CheckGround();
        if (FeelsGravity()) {
            HandleGravity();
        }
        // if (CanInteract()) {
        //     HandleInteractionCheck();
        //     HandleInteractionInput();
        // }
        // From here, only movement actions should be handled.
        if (!canAcceptInput) {
            return;
        }
        if (CanMove()) {
            HandleMovement();
        }
        if (CanJump()) {
            HandleJump();
        }
        if (CanVaultToLedge()) {
            HandleVaultToLedge();
        }
        // if (CanDash()) {
        //     HandleDash();
        // }
        if (CanCrouch()) {
            HandleCrouch();
        }
        // if (CanZoom()) {
        //     HandleZoom();
        // }
        // Debug.Log(rb.velocity.y);
    }

    private void LateUpdate() {
        if (CanMouseLook()) {
            HandleMouseLook();
        }
    }

    private void HandleInputs() {
        if (inputManager.jump) {
            timeSinceJumpRequested = 0f;
            jumpRequested = true;
        }
    }

    private bool FeelsGravity() {
        return gravityEnabled;
    }

    private bool CanMove() {
        return moveEnabled;
    }

    private bool CanWalk() {
        return walkEnabled;
    }

    private bool CanSprint() {
        return sprintEnabled;
    }

    private bool CanDash() {
        return dashEnabled && !isCrouching;
    }

    private bool CanVaultToLedge() {
        return vaultingToLedgesEnabled && !isCrouching && !isDashing && !isJumping;
    }

    private bool CanJump() {
        return jumpEnabled;
    }

    private bool CanCrouch() {
        return crouchEnabled && !isDashing && !isJumping && !isVaultingToLedge;
    }

    private bool CanZoom() {
        return zoomEnabled;
    }

    private bool CanInteract() {
        return interactEnabled;
    }

    private bool CanMouseLook() {
        return mouseLookEnabled;
    }

    private void HandleGravity() {
        // apply gravity downwards and consider player mass as well
        var rbVelocity = rb.velocity;
        rbVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && rb.velocity.y < 0) {
            rbVelocity.y = -2f;
        }
        rb.velocity = rbVelocity;
    }

    private void CheckGround() {
        IsGrounded();
    }

    /**
     * Project a sphere under the player to check for collision to the ground layer.
     */
    private void IsGrounded() {
        var src = rb.worldCenterOfMass;
        var dist = transform.localScale.y;
        // cast multiple rays at angles to check for ground hits
        Physics.Raycast(src, Vector3.down, out var hit1, dist + groundTolerance, groundLayer);
        var axis1 = Quaternion.AngleAxis(20, Vector3.right) * Vector3.down;
        Physics.Raycast(src, axis1, out var hit2, dist, groundLayer);
        var axis2 = Quaternion.AngleAxis(-20, Vector3.right) * Vector3.down;
        Physics.Raycast(src, axis2, out var hit3, dist, groundLayer);
        var axis3 = Quaternion.AngleAxis(20, Vector3.forward) * Vector3.down;
        Physics.Raycast(src, axis3, out var hit4, dist, groundLayer);
        var axis4 = Quaternion.AngleAxis(-20, Vector3.forward) * Vector3.down;
        Physics.Raycast(src, axis4, out var hit5, dist, groundLayer);
        isGrounded = hit1.collider != null
                  || hit2.collider != null
                  || hit3.collider != null
                  || hit4.collider != null
                  || hit5.collider != null;
        // If isGrounded, set the player to the ground and reset the velocity
        if (isGrounded) {
            var groundPoint = hit1.point;
            if (hit2.collider != null) {
                groundPoint = hit2.point;
            }
            if (hit3.collider != null) {
                groundPoint = hit3.point;
            }
            if (hit4.collider != null) {
                groundPoint = hit4.point;
            }
            if (hit5.collider != null) {
                groundPoint = hit5.point;
            }
            var groundPosition = new Vector3(transform.position.x,
                                             groundPoint.y + transform.localScale.y,
                                             transform.position.z);
            transform.position = groundPosition;
            currentVelocity.y = 0;
            timeSinceLastLanding = 0f;
        }
    }

    /**
     * Handle horizontal movement on the ground and in the air.
     * Default WASD.
     */
    private void HandleMovement() {
        switch (isGrounded) {
            case true when isDashing :
                CalculateTargetSpeed();
                // todo implement dashing
                break;
            case true : {
                CalculateTargetSpeed();
                var velocityChange = AddInputToCurrentPlanarVelocityAndGetDifference();
                AddForce(velocityChange, ForceMode.VelocityChange);
                break;
            }
            default : {
                // consider in air and falling, add air drag and disable control
                var drag = new Vector3(currentVelocity.x * airDrag, 0, currentVelocity.z * airDrag);
                AddForce(transform.TransformVector(drag), ForceMode.VelocityChange);
                break;
            }
        }
    }

    private void CalculateTargetSpeed() {
        var ts = runSpeed;
        if (inputManager.crouch) {
            ts = crouchSpeed;
        } else if (inputManager.sprint) {
            ts = sprintSpeed;
        } else if (inputManager.walk) {
            ts = walkSpeed;
        } else if (inputManager.dash) {
            ts = dashSpeed;
        }
        // if cannot stand up yet but crouch has been canceled, set back to crouch speed
        if (!IsStandingUp() && !inputManager.crouch) {
            ts = crouchSpeed;
        }
        // If no movement input, set speed to 0
        if (Mathf.Approximately(inputManager.move.x, 0f) && Mathf.Approximately(inputManager.move.y, 0f)) {
            ts = 0f;
        }
        // Smoothly interpolate between speed transitions
        if (ts > currentSpeed) {
            currentSpeed = Mathf.Min(ts, currentSpeed + acceleration * Time.deltaTime);
        } else if (ts < currentSpeed) {
            currentSpeed = Mathf.Max(ts, currentSpeed - acceleration * Time.deltaTime);
        }
    }

    private Vector3 AddInputToCurrentPlanarVelocityAndGetDifference() {
        var vec = new Vector3(inputManager.move.x, 0, inputManager.move.y);
        var targetVelocity = transform.TransformVector(vec) * currentSpeed;
        var velocityChange = targetVelocity - rb.velocity;
        // Remove vertical component for planar movement
        velocityChange.y = 0;
        return velocityChange;
    }

    private void Dash() {
        if (dashRoutine != null) {
            StopCoroutine(dashRoutine);
        }
        dashRoutine = StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine() {
        // Disable input during dash
        DisableInput();
        var dashDirection = transform.forward;
        var dashDistRemaining = 0f;
        while (dashDistRemaining < dashDistance) {
            AddForce(dashDirection * dashSpeed, ForceMode.Acceleration);
            dashDistRemaining += dashSpeed * Time.deltaTime;
            yield return null;
        }
        EnableInput();
    }

    private void HandleJump() {
        // todo handle landing
        // todo handle landing with different grace for different speeds or fall distances
        if (isGrounded && inputManager.jump) {
            AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -3f * doubleGravityTimesMass), ForceMode.Impulse);
        }
    }

    private void HandleDash() {
        // todo
    }

    private void HandleVaultToLedge() {
        var camTransform = playerCamera.transform;
        if (!Physics.Raycast(camTransform.position, camTransform.forward, out var hit, 3f, ledgeLayer)) {
            return;
        }
        // Debug.Log("vault target in sights");
        var rayOrigin = hit.point + (camTransform.forward * playerRadius) + (Vector3.up * vaultHeight * standingHeight);
        if (!Physics.Raycast(rayOrigin, Vector3.down, out var landPoint, standingHeight)) {
            return;
        }
        // Debug.Log("vault landing point found");
        if (inputManager.interact) {
            // Debug.Log("vaulting");
            StartCoroutine(LerpVault(landPoint.point, 0.5f));
        }
    }

    private IEnumerator LerpVault(Vector3 point, float time) {
        DisableInput();
        float timer = 0;
        var startPos = transform.position;
        var startRot = transform.rotation;
        var endRot = Quaternion.LookRotation(point - transform.position);
        while (timer < time) {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, point, timer / time);
            transform.rotation = Quaternion.Lerp(startRot, endRot, timer / time);
            yield return null;
        }
        // lerp back to start rotation
        timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(endRot, startRot, timer / time);
            yield return null;
        }
        EnableInput();
        // Debug.Log("vault complete, input restored");
    }

    private void HandleCrouch() {
        crouchTimer += Time.deltaTime;
        var percentComplete = crouchTimer / 1f;
        percentComplete *= percentComplete;
        if (inputManager.crouch) {
            // crouch the player down to crouching height
            transform.localScale = new Vector3(transform.localScale.x,
                                               Mathf.Lerp(transform.localScale.y, crouchHeight, percentComplete),
                                               transform.localScale.z);
        } else {
            // prevent standing up if there is an object above the player
            if (CanStandUp()) {
                transform.localScale = new Vector3(transform.localScale.x,
                                                   Mathf.Lerp(transform.localScale.y, standingHeight, percentComplete),
                                                   transform.localScale.z);
            }
        }
        if (percentComplete > 1) {
            crouchTimer = 0f;
        }
        // Debug.Log(transform.localScale.y);
    }

    private bool CanStandUp() {
        var tr = transform;
        var ray = new Ray(tr.position, tr.up);
        var h = tr.localScale.y;
        return !Physics.SphereCast(ray, h, h - playerRadius * 0.5f, groundLayer);
    }

    private bool IsStandingUp() {
        return Mathf.Approximately(transform.localScale.y, standingHeight);
    }

    private void HandleZoom() {
        if (inputManager.zoom) {
            if (zoomRoutine != null) {
                StopCoroutine(zoomRoutine);
            }
            zoomRoutine = StartCoroutine(ZoomRoutine());
        } else {
            if (zoomRoutine != null) {
                StopCoroutine(zoomRoutine);
            }
            zoomRoutine = StartCoroutine(UnZoomRoutine());
        }
    }

    private IEnumerator ZoomRoutine() {
        var p = 0f;
        while (p < 1) {
            p += Time.deltaTime / timeToZoom;
            playerCamera.fieldOfView = Mathf.Lerp(defaultFov, zoomFOV, p);
            yield return null;
        }
    }

    private IEnumerator UnZoomRoutine() {
        var p = 0f;
        while (p < 1) {
            p += Time.deltaTime / timeToZoom;
            playerCamera.fieldOfView = Mathf.Lerp(zoomFOV, defaultFov, p);
            yield return null;
        }
    }

    private void HandleInteractionCheck() { }

    private void HandleInteractionInput() { }

    private void HandleMouseLook() {
        var mouseX = inputManager.look.x;
        var mouseY = inputManager.look.y;
        playerCamera.transform.position = cameraRoot.position;
        xRotation -= mouseY * mouseSensitivity * Time.smoothDeltaTime;
        xRotation = Mathf.Clamp(xRotation, lookDownLimit, lookUpLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX * mouseSensitivity * Time.smoothDeltaTime, 0));
    }

    private void AddForce(Vector3 force, ForceMode mode) {
        // Debug.Log("Adding force: " + force + " with mode: " + mode);
        rb.AddForce(force, mode);
    }

    private void DisableInput() {
        canAcceptInput = false;
    }

    private void EnableInput() {
        canAcceptInput = true;
    }
}