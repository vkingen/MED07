using UnityEngine;
using TMPro;

public class Inspect : MonoBehaviour
{
    public float raycastDistance = 2f;  // Distance the player can inspect objects
    public Transform inspectionPoint;   // Point in front of the player for the inspected object
    private GameObject currentObject;   // Currently inspected object
    private bool isInspecting = false;  // Flag for inspection state
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    public PlayerController playerController;

    public TextMeshProUGUI interactionPrompt;

    void Update()
    {
    if (!isInspecting)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Inspectable"))
            {
                interactionPrompt.gameObject.SetActive(true);

                if (Input.GetMouseButtonDown(0)) // Left mouse button to toggle inspection
                {
                    currentObject = hit.collider.gameObject;
                    StartInspecting();
                }
            }
            else
            {
                interactionPrompt.gameObject.SetActive(false);
            }
            
        }
        else
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }
    else
    {
        interactionPrompt.gameObject.SetActive(false);

        if (Input.GetMouseButtonDown(0)) // Left mouse button to stop inspecting
        {
            StopInspecting();
        }
        else
        {
            RotateObject();
        }
    }
}

    void StartInspecting()
    {
        playerController.enabled = false; // Disable player controls
        isInspecting = true;
        originalPosition = currentObject.transform.position;
        currentObject.transform.position = inspectionPoint.position;
        currentObject.transform.SetParent(inspectionPoint);  // Parent to inspection point
        
        // Store the original rotation & position before aligning
        
        originalRotation = currentObject.transform.rotation;
        currentObject.transform.rotation = Camera.main.transform.rotation;
        
        var rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; // Disable physics while inspecting
    }

    void StopInspecting()
    {
        playerController.enabled = true; // Re-enable player controls
        interactionPrompt.gameObject.SetActive(false);
        isInspecting = false;
        currentObject.transform.SetParent(null);  // Unparent
        
        // Restore the original rotation
        currentObject.transform.position = originalPosition;
        currentObject.transform.rotation = originalRotation;

        var rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false; // Re-enable physics
        currentObject = null;
    }

    void RotateObject()
    {
        float rotationX = Input.GetAxis("Mouse ScrollWheel") * 100f;
        float rotationY = Input.GetAxis("Horizontal") * 100f * Time.deltaTime;
        currentObject.transform.Rotate(Vector3.up, -rotationX, Space.World);   // Horizontal rotation
        currentObject.transform.Rotate(Vector3.right, rotationY, Space.World); // Vertical rotation
    }
}
