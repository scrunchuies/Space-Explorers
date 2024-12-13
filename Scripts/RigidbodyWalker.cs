using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.SoundManagerNamespace;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class RigidbodyWalker : MonoBehaviour
{
    public Transform planet;
    public float acceleration = 18f;
    public float maxSpeed = 8f;
    public bool canJump = true;
    public float jumpHeight = 3.0f;
    public Camera playerCamera;
    public float lookSpeed = 1.0f;
    public float lookXLimit = 60.0f;
    public float groundCheckRadius = 0.5f;
    public float slopeLimit = 75f;
    public float decelerationFactor = 0.3f;

    [Header("SFX")]
    [SerializeField] private AudioClip jumping;
    [SerializeField] private AudioClip walking;
    [SerializeField] private float walkingVolume = 0.15f;
    private AudioSource audioSource;
    private float graceTime = 0.3f;
    private float ungroundedTimer = 0f;

    private bool isPaused = false;
    private bool grounded = false;
    private bool isWalking = false;
    private Rigidbody r;
    private Vector2 rotation = Vector2.zero;
    private Vector3 groundNormal;
    private float maxVerticalVelocity = 5f;

    // Input actions
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        controls.Player.Jump.performed += ctx => jumpInput = true;
        controls.Player.Jump.canceled += ctx => jumpInput = false;

        r = GetComponent<Rigidbody>();
        r.freezeRotation = true;  // Prevent physics from altering the rotation
        r.useGravity = false;  // Gravity is handled by the planet's gravity
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        r.interpolation = RigidbodyInterpolation.Interpolate;
        rotation.y = transform.eulerAngles.y;

        // Ensure cursor is locked at start
        if (Application.isFocused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = walkingVolume;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!enabled) return;

        // Handle camera rotation based on mouse delta or right stick (gamepad)
        float mouseX = lookInput.x * lookSpeed;  // Horizontal rotation (yaw)
        float mouseY = -lookInput.y * lookSpeed; // Vertical rotation (pitch)

        // Apply vertical rotation to the camera (pitch)
        rotation.x += mouseY;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);  // Clamp vertical rotation
        playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0f, 0f);  // Apply pitch (up/down)

        // Apply horizontal rotation to the player using Quaternions (yaw)
        if (Mathf.Abs(mouseX) > 0.001f)
        {
            Quaternion yawRotation = Quaternion.Euler(0f, mouseX, 0f);
            transform.rotation *= yawRotation;
        }

        HandleWalkingSound();
    }

    void FixedUpdate()
    {
        if (!enabled) return;

        // Calculate movement direction relative to the planet's surface
        Vector3 planetToPlayer = transform.position - planet.position;  // Direction from planet center to player
        groundNormal = planetToPlayer.normalized;  // Surface normal, pointing outward from planet

        // Get input directions from the new input system
        float vertical = moveInput.y;
        float horizontal = moveInput.x;

        // Adjust movement to be relative to the planet surface
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Project movement direction onto the planet's surface (perpendicular to the surface normal)
        moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            // Gradual acceleration
            if (r.velocity.magnitude < maxSpeed)
            {
                Vector3 force = moveDirection * acceleration;
                r.AddForce(force, ForceMode.Force);
            }
        }
        else if (grounded) // Apply deceleration only when grounded
        {
            r.velocity *= decelerationFactor;
        }

        // Jump mechanic
        if (jumpInput && canJump && grounded)
        {
            Vector3 jumpForce = groundNormal * jumpHeight;  // Jumping in the direction of the surface normal
            r.AddForce(jumpForce, ForceMode.VelocityChange);
            grounded = false;  // Temporarily set grounded to false to prevent double-jumping
            SoundController.instance.PlaySound(jumping);
        }

        // Clamp vertical velocity for low-gravity feel
        Vector3 localVel = transform.InverseTransformDirection(r.velocity);
        localVel.y = Mathf.Clamp(localVel.y, -maxVerticalVelocity, maxVerticalVelocity);
        r.velocity = transform.TransformDirection(localVel);

        // Update ungrounded timer
        if (!grounded)
        {
            ungroundedTimer += Time.unscaledDeltaTime;
        }
        else
        {
            ungroundedTimer = 0f;
        }

        // Reset grounded status every frame
        grounded = false;
    }

    // Handle collisions
    void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            float angle = Vector3.Angle(contact.normal, transform.up);

            if (angle <= slopeLimit)
            {
                grounded = true;
                groundNormal = contact.normal;
                break;
            }
        }
    }

    void HandleWalkingSound()
    {
        // Makes sure walking sound dosen't overlap over eachother and plays smoothly
        if (!enabled) return;

        bool isMoving = (grounded || ungroundedTimer <= graceTime) &&
                        (Mathf.Abs(moveInput.x) > 0.1f || Mathf.Abs(moveInput.y) > 0.1f);

        if (isMoving && !isWalking)
        {
            audioSource.clip = walking;
            audioSource.Play();
            isWalking = true;
        }
        else if (!isMoving && isWalking)
        {
            audioSource.Stop();
            isWalking = false;
        }
    }

    public void SetPauseState(bool paused)
    {
        isPaused = paused;
    }
}
