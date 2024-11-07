using UnityEngine;

public class DragAndDrop3D : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 originalPosition;
    private Photograph photograph; // Reference to the Photograph component

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
        photograph = GetComponent<Photograph>(); // Get the Photograph component
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPos() + offset;
            newPosition.z = transform.position.z; // Maintain the original Z position
            transform.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        bool snapped = false;

        foreach (var hitCollider in hitColliders)
        {
            DropArea dropArea = hitCollider.GetComponent<DropArea>();
            if (dropArea != null && dropArea.expectedType == photograph.type) // Check for matching types
            {
                dropArea.SnapPhotographToDropArea(transform);
                snapped = true;
                break;
            }
        }

        if (!snapped)
        {
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        // Set the Z position to the distance from the camera to the object being dragged
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
}