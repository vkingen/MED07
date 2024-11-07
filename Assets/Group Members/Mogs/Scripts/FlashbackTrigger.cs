using UnityEngine;

public class FlashbackTrigger : MonoBehaviour
{
    public int flashbackIndex; // Index of the flashback to trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FlashbackManager flashbackManager = FindObjectOfType<FlashbackManager>();
            if (flashbackManager != null)
            {
                flashbackManager.TriggerFlashback(flashbackIndex);
            }
        }
    }
}