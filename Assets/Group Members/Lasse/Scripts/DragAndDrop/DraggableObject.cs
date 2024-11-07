using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging;
    private DropZone currentDropZone;
    private Vector3 originalPosition;

    // Define the type of this draggable object
    public ItemType objectType;

    // Boundary within which the object can be dragged
    public BoxCollider dragBoundary;

    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position; // Save original position in case we need to reset
    }

    private void OnMouseDown()
    {
        // Calculate the offset between the object position and the mouse position
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        // Clear the current drop zone's occupying object if we're picking it up
        if (currentDropZone != null)
        {
            currentDropZone.ClearOccupyingObject();
            currentDropZone = null; // Clear the current reference
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            // Calculate the new position based on the mouse position plus the offset
            Vector3 newPosition = GetMouseWorldPosition() + offset;

            // Clamp the position to stay within the boundary
            if (dragBoundary != null)
            {
                newPosition = ClampPositionToBoundary(newPosition, dragBoundary);
            }

            // Update the object's position
            transform.position = newPosition;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // Check if the object is over a valid drop zone and if it is unoccupied
        if (currentDropZone != null && !currentDropZone.IsOccupied && currentDropZone.CanAcceptObject(this))
        {
            // Snap the object to the center of the drop zone
            transform.position = currentDropZone.transform.position;
            transform.rotation = currentDropZone.transform.rotation; // Optionally reset rotation

            // Set this object as the occupying object for the drop zone
            currentDropZone.SetOccupyingObject(this);
        }
        else
        {
            // Return to the original position if not over an unoccupied drop zone
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in world space
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 ClampPositionToBoundary(Vector3 position, BoxCollider boundary)
    {
        // Get the min and max bounds of the BoxCollider
        Vector3 minBounds = boundary.bounds.min;
        Vector3 maxBounds = boundary.bounds.max;

        // Clamp the position to within the bounds
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        position.z = Mathf.Clamp(position.z, minBounds.z, maxBounds.z);

        return position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider we entered is a drop zone
        if (other.CompareTag("DropZone"))
        {
            DropZone dropZone = other.GetComponent<DropZone>();

            // Only set as the current drop zone if it's unoccupied and matches object type
            if (dropZone != null && !dropZone.IsOccupied && dropZone.CanAcceptObject(this))
            {
                currentDropZone = dropZone;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider we exited is the current drop zone
        if (other.CompareTag("DropZone") && currentDropZone == other.GetComponent<DropZone>())
        {
            // Clear the reference to the drop zone and remove this object as occupant
            currentDropZone = null;
        }
    }

    private void OnDisable()
    {
        // Clear the drop zone’s reference if this object is destroyed or disabled
        if (currentDropZone != null)
        {
            currentDropZone.ClearOccupyingObject();
        }
    }
}
