using UnityEngine;

public class DropArea : MonoBehaviour
{
    public PhotographType expectedType; // Set this in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Photograph"))
        {
            Photograph photograph = other.GetComponent<Photograph>();
            if (photograph != null && photograph.type == expectedType)
            {
                // Snap the photograph to the drop area
                SnapPhotographToDropArea(other.transform);
            }
        }
    }

    public void SnapPhotographToDropArea(Transform photographTransform)
    {
        // Set the position of the photograph to the center of the drop area
        photographTransform.position = transform.position;

        // Optionally, you can adjust the rotation to match the drop area
        photographTransform.rotation = transform.rotation;

        // Disable further interactions
        photographTransform.GetComponent<Collider>().enabled = false;

        // Make it a child of the drop area if needed
        photographTransform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Photograph"))
        {
            // Optional: Enable further interactions if you want to allow dragging again
            other.GetComponent<Collider>().enabled = true;
        }
    }
}