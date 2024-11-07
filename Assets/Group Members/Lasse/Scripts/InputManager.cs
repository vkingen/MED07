using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool zoom { get; private set; }
    public bool crouch { get; private set; }
    public bool interact { get; private set; }

    private InputActionMap currentMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction zoomAction;
    private InputAction crouchAction;
    private InputAction interactAction;


    private void Awake()
    {
        currentMap = playerInput.currentActionMap;
        moveAction = currentMap.FindAction("Move");
        lookAction = currentMap.FindAction("Look");
        zoomAction = currentMap.FindAction("Zoom");
        crouchAction = currentMap.FindAction("Crouch");
        interactAction = currentMap.FindAction("Interact");

        moveAction.performed += onMove;
        lookAction.performed += onLook;
        zoomAction.performed += onZoom;
        crouchAction.performed += onCrouch;
        interactAction.performed += onInteract;

        moveAction.canceled += onMove;
        lookAction.canceled += onLook;
        zoomAction.canceled += onZoom;
        crouchAction.canceled += onCrouch;
        interactAction.canceled += onInteract;
    }

    private void onMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void onLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    private void onZoom(InputAction.CallbackContext context)
    {
        zoom = context.ReadValueAsButton();
    }

    private void onCrouch(InputAction.CallbackContext context)
    {
        crouch = context.ReadValueAsButton();
    }

    private void onInteract(InputAction.CallbackContext context)
    {
        interact = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        currentMap.Enable();
    }

    private void OnDisable()
    {
        currentMap.Disable();
    }
}
