using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    private bool isOpen = false;                   // Tracks if the door is open or closed
    public Transform doorHinge;                    // Hinge (or pivot point) for door rotation
    public float openAngle = 90f;                  // Default angle to open the door
    public float openSpeed = 2f;                   // Speed of door rotation
    public Vector3 openAxis = Vector3.up;          // Axis around which the door rotates (Y-axis for regular doors)

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        // Set up open and closed rotations based on door hinge, open angle, and open axis
        closedRotation = doorHinge.localRotation;
        openRotation = closedRotation * Quaternion.Euler(openAxis * openAngle);
    }

    public void Interact() // This is the method required by IInteractable
    {
        isOpen = !isOpen; // Toggle open/close state
        StopAllCoroutines();
        StartCoroutine(RotateDoor());
    }

    private System.Collections.IEnumerator RotateDoor()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        while (Quaternion.Angle(doorHinge.localRotation, targetRotation) > 0.1f)
        {
            doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        // Ensure precise final rotation alignment
        doorHinge.localRotation = targetRotation;
    }
}
