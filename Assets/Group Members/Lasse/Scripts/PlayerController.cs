using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private float maxCameraRotation = -40f;
    [SerializeField] private float minCameraRotation = 70f;
    [SerializeField] private float mouseSensitivity = 20f;
    private float xRotation;

    [SerializeField] private float zoomFOV = 60f;
    [SerializeField] private float normalFOV = 90f;
    [SerializeField] private float zoomSpeed = 5f;

    private float currentFOV;

    private CharacterController characterController;
    private InputManager inputManager;
    private Animator animator;
    private int xVelocityHash;
    private int yVelocityHash;
    private float animationBlendSpeed = 8f;

    private const float walkSpeed = 2f;
    private const float crouchSpeed = 1f;
    private Vector2 currentVelocity;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float gravity = -9.81f;
    private float groundedGravity = -0.05f;

    // Crouch settings
    private float normalHeight;
    [SerializeField] private float crouchHeight = 0.888f;
    [SerializeField] private float crouchCenterOffset = 0.444f;
    private bool isCrouching = false;

    // Crosshair and Interaction settings
    [SerializeField] private Image crosshairDot; // UI image for crosshair
    [SerializeField] private float interactionRange = 3f; // Range for detecting interactable objects
    private IInteractable currentInteractable; // Store the current interactable object
    private bool canInteract = true; // Control interaction trigger

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();

        xVelocityHash = Animator.StringToHash("X_Velocity");
        yVelocityHash = Animator.StringToHash("Y_Velocity");

        HideCursor();

        currentFOV = normalFOV;
        camera.fieldOfView = currentFOV;

        normalHeight = characterController.height;
        crosshairDot.enabled = false; // Hide crosshair at start
    }

    private void Update()
    {
        HandleCrouch();
        Move();
        CheckForInteractable(); // Check for interactable objects every frame

        // Check if the interact button is pressed and interact with the object only on the initial press
        if (inputManager.interact && canInteract && currentInteractable != null)
        {
            Interact();
            canInteract = false; // Disable interaction until button is released
        }

        // Reset canInteract when the interact button is released
        if (!inputManager.interact)
        {
            canInteract = true;
        }
    }

    private void LateUpdate()
    {
        CameraMovement();
        CameraZoom();
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Move()
    {
        isGrounded = characterController.isGrounded;

        float targetSpeed = isCrouching ? crouchSpeed : walkSpeed;

        if (inputManager.move == Vector2.zero)
        {
            targetSpeed = 0.1f;
        }

        Vector2 targetVelocity = inputManager.move.normalized * targetSpeed;
        currentVelocity.x = Mathf.Lerp(currentVelocity.x, targetVelocity.x, animationBlendSpeed * Time.deltaTime);
        currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetVelocity.y, animationBlendSpeed * Time.deltaTime);

        Vector3 move = transform.right * currentVelocity.x + transform.forward * currentVelocity.y;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = groundedGravity;
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }

        characterController.Move((move + playerVelocity) * Time.deltaTime);

        animator.SetFloat(xVelocityHash, currentVelocity.x);
        animator.SetFloat(yVelocityHash, currentVelocity.y);
    }

    private void HandleCrouch()
    {
        if (inputManager.crouch)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                characterController.height = crouchHeight;
                characterController.center = new Vector3(0, crouchCenterOffset, 0);
                animator.SetBool("IsCrouching", true);
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
                characterController.height = normalHeight;
                characterController.center = new Vector3(0, 0.888f, 0);
                animator.SetBool("IsCrouching", false);
            }
        }
    }

    private void CameraMovement()
    {
        var mouseX = inputManager.look.x;
        var mouseY = inputManager.look.y;

        camera.transform.position = cameraAnchor.position;

        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, maxCameraRotation, minCameraRotation);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up, mouseX * mouseSensitivity * Time.deltaTime);
    }

    private void CameraZoom()
    {
        float targetFOV = inputManager.zoom ? zoomFOV : normalFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, zoomSpeed * Time.deltaTime);
        camera.fieldOfView = currentFOV;
    }

    private void CheckForInteractable()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                crosshairDot.enabled = true; // Show crosshair if looking at interactable
                currentInteractable = interactable; // Set the interactable reference
                return;
            }
        }

        crosshairDot.enabled = false; // Hide crosshair if no interactable object
        currentInteractable = null;   // Clear interactable reference
    }

    private void Interact()
    {
        currentInteractable?.Interact();
    }
}
