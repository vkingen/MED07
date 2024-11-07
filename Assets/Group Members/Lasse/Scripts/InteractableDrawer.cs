using UnityEngine;

public class InteractableDrawer : MonoBehaviour, IInteractable
{
    private bool isOpen = false;                      // Tracks if the drawer is open or closed
    public Transform drawerTransform;                 // Transform of the drawer to slide
    public float slideDistance = 0.5f;                // Distance the drawer should slide
    public float slideSpeed = 2f;                     // Speed of the sliding motion

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Start()
    {
        // Store the initial (closed) position and calculate the open position
        closedPosition = drawerTransform.localPosition;
        openPosition = closedPosition + drawerTransform.forward * slideDistance;
    }

    public void Interact() // Required method by IInteractable interface
    {
        isOpen = !isOpen; // Toggle open/close state
        StopAllCoroutines();
        StartCoroutine(SlideDrawer());
    }

    private System.Collections.IEnumerator SlideDrawer()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        // Slide the drawer to the target position
        while (Vector3.Distance(drawerTransform.localPosition, targetPosition) > 0.01f)
        {
            drawerTransform.localPosition = Vector3.Lerp(drawerTransform.localPosition, targetPosition, Time.deltaTime * slideSpeed);
            yield return null;
        }

        // Ensure it reaches the exact target position
        drawerTransform.localPosition = targetPosition;
    }
}
