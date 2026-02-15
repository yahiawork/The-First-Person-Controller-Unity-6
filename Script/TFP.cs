using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float pitchClamp = 89f;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float acceleration = 18f;   // how fast we reach target speed
    [SerializeField] private float airControl = 0.35f;    // 0..1

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -19.62f;     // slightly stronger than default for snappier FPS feel
    [SerializeField] private float groundStickForce = -2.0f;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 1.1f;
    [SerializeField] private float crouchLerpSpeed = 12f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.15f;

    // Components
    private CharacterController controller;

    // Look state
    private float yaw;
    private float pitch;

    // Move state
    private Vector3 velocity;         // vertical velocity in y, plus some smoothing
    private Vector3 moveVelocity;     // smoothed horizontal velocity
    private bool isCrouching;

    private void Reset()
    {
        // Try to auto-assign a camera if possible
        Camera cam = GetComponentInChildren<Camera>();
        if (cam) cameraTransform = cam.transform;

        standHeight = 1.8f;
        crouchHeight = 1.1f;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam) cameraTransform = cam.transform;
        }

        // Initialize look angles from current rotation
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform ? cameraTransform.localEulerAngles.x : 0f;

        // Set controller height
        controller.height = standHeight;
        controller.center = new Vector3(0f, standHeight * 0.5f, 0f);
    }

    private void OnEnable()
    {
        LockCursor(true);
    }

    private void OnDisable()
    {
        LockCursor(false);
    }

    private void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("FirstPersonController: Assign Camera Transform.");
            return;
        }

        HandleCursorToggle();
        HandleLook();
        HandleCrouch();
        HandleMovement();
    }

    private void HandleCursorToggle()
    {
        // Press Escape to unlock, click to lock again (optional nice behavior)
        if (Input.GetKeyDown(KeyCode.Escape))
            LockCursor(false);

        if (Input.GetMouseButtonDown(0))
            LockCursor(true);
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void HandleLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mx = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float my = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        yaw += mx;

        float yDir = invertY ? 1f : -1f;
        pitch += my * yDir;

        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        // Apply rotations: yaw on player body, pitch on camera
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleCrouch()
    {
        // Hold Ctrl (or C) to crouch
        bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        // If you want toggle crouch, replace above with:
        // if (Input.GetKeyDown(KeyCode.C)) isCrouching = !isCrouching;

        isCrouching = crouchInput;

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        float newHeight = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchLerpSpeed);
        controller.height = newHeight;

        // Keep the bottom "feet" in place by adjusting center
        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);
    }

    private void HandleMovement()
    {
        // Ground check: CharacterController has isGrounded, but it's sometimes unreliable on slopes.
        bool grounded = IsGrounded();

        // Input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        // Speed
        bool sprint = Input.GetKey(KeyCode.LeftShift) && !isCrouching && z > 0.1f;
        float targetSpeed = isCrouching ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);

        // Convert to world direction (relative to yaw)
        Vector3 desired = (transform.right * input.x + transform.forward * input.z) * targetSpeed;

        // Smooth horizontal movement
        float control = grounded ? 1f : airControl;
        moveVelocity = Vector3.MoveTowards(moveVelocity, desired, acceleration * control * Time.deltaTime);

        // Jump
        if (grounded)
        {
            // small downward force to keep us grounded on slopes
            if (velocity.y < 0f) velocity.y = groundStickForce;

            if (Input.GetButtonDown("Jump") && !isCrouching)
            {
                // v = sqrt(h * -2g)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Move final
        Vector3 finalMove = moveVelocity;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        // Use CharacterController.isGrounded + a short ray to be more stable
        if (controller.isGrounded) return true;

        Vector3 origin = transform.position + Vector3.up * 0.05f;
        return Physics.SphereCast(origin, controller.radius * 0.9f, Vector3.down,
            out _, groundCheckDistance, ~0, QueryTriggerInteraction.Ignore);
    }
}
