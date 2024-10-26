using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private Transform camera;
    [SerializeField] private float maxCameraRotation = -40f;
    [SerializeField] private float minCameraRotation = 70f;
    [SerializeField] private float mouseSensitivity = 20f;
    private float xRotation;

    [SerializeField] private float cameraSmoothSpeed = 0.1f;  // Smoothing speed for camera movement
    private Vector3 cameraVelocity = Vector3.zero;

    private CharacterController characterController;
    private InputManager inputManager;
    private Animator animator;
    private int xVelocityHash;
    private int yVelocityHash;
    private float animationBlendSpeed = 9f;

    private const float walkSpeed = 2f;
    private Vector2 currentVelocity;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float gravity = -9.81f;
    private float groundedGravity = -0.05f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();

        xVelocityHash = Animator.StringToHash("X_Velocity");
        yVelocityHash = Animator.StringToHash("Y_Velocity");

        HideCursor();
    }

    private void Update()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraMovement();
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Move()
    {
        // Check if the character is on the ground
        isGrounded = characterController.isGrounded;

        // Handle input-based movement
        float targetSpeed = walkSpeed;
        if (inputManager.move == Vector2.zero)
        {
            targetSpeed = 0.1f;
        }

        Vector2 targetVelocity = inputManager.move.normalized * targetSpeed;
        currentVelocity.x = Mathf.Lerp(currentVelocity.x, targetVelocity.x, animationBlendSpeed * Time.deltaTime);
        currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetVelocity.y, animationBlendSpeed * Time.deltaTime);

        Vector3 move = transform.right * currentVelocity.x + transform.forward * currentVelocity.y;

        // Apply gravity
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = groundedGravity;  // Small constant to keep the player grounded
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }

        // Apply movement and gravity to the character controller
        characterController.Move((move + playerVelocity) * Time.deltaTime);

        // Set animation parameters
        animator.SetFloat(xVelocityHash, currentVelocity.x);
        animator.SetFloat(yVelocityHash, currentVelocity.y);
    }

    private void CameraMovement()
    {
        var mouseX = inputManager.look.x;
        var mouseY = inputManager.look.y;

        // Smoothly move the camera towards the anchor position
        camera.position = Vector3.SmoothDamp(camera.position, cameraAnchor.position, ref cameraVelocity, cameraSmoothSpeed);

        // Handle camera rotation
        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, maxCameraRotation, minCameraRotation);

        camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up, mouseX * mouseSensitivity * Time.deltaTime);
    }
}
