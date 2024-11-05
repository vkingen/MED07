using UnityEngine;

public class DropZone : MonoBehaviour
{
    // Define the type of item this drop zone accepts
    public ItemType zoneType;

    // Track if this DropZone is occupied
    private DraggableObject occupyingObject;

    public bool IsOccupied => occupyingObject != null;

    public void SetOccupyingObject(DraggableObject obj)
    {
        occupyingObject = obj;
    }

    public void ClearOccupyingObject()
    {
        occupyingObject = null;
    }

    // Check if a DraggableObject's type matches this DropZone's type
    public bool CanAcceptObject(DraggableObject obj)
    {
        return obj.objectType == zoneType;
    }
}
