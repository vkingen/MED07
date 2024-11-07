using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FlashbackManager : MonoBehaviour
{
    public GameObject flashbackEffectPrefab; // Assign a flash effect prefab
    public Flashback[] flashbacks; // Array of flashbacks
    private bool isTransitioning = false;
    private Transform playerTransform; // Reference to the player's transform

    private void Start()
    {
        // Find the player transform (assuming the player has a tag "Player")
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // You can define which flashback to trigger here, for example based on the collider name
            TriggerFlashback(0); // Change the index based on the desired flashback
        }
    }

    public void TriggerFlashback(int flashbackIndex)
    {
        if (flashbackIndex < 0 || flashbackIndex >= flashbacks.Length)
        {
            Debug.LogWarning("Invalid flashback index.");
            return;
        }

        StartCoroutine(HandleFlashback(flashbacks[flashbackIndex]));
    }

    private IEnumerator HandleFlashback(Flashback flashback)
    {
        isTransitioning = true;

        // Trigger flash effect
        GameObject flashEffect = Instantiate(flashbackEffectPrefab);
        flashEffect.GetComponent<FlashEffect>().TriggerFlash(1f); // Trigger flash with duration
        Destroy(flashEffect, 1f); // Destroy after the flash duration

        // Wait for the flash effect to finish
        yield return new WaitForSeconds(0.5f);

        // Store the player's current position
        Vector3 originalPosition = playerTransform.position;

        // Load the flashback scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(flashback.sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Set the player's position in the flashback scene
        playerTransform.position = flashback.targetPosition;

        // Wait for the duration of the flashback
        yield return new WaitForSeconds(flashback.duration);

        // Optionally, you can add another flash effect here before returning
        // Trigger flash effect again if needed
        GameObject FlashEffect = Instantiate(flashbackEffectPrefab);
        flashEffect.GetComponent<FlashEffect>().TriggerFlash(1f); // Trigger flash with duration
        Destroy(FlashEffect, 1f); // Adjust the flash effect duration if needed

        // Wait for the flash effect to finish
        yield return new WaitForSeconds(0.5f);

        // Return to the original scene
        asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Restore the player's position
        playerTransform.position = originalPosition;

        isTransitioning = false;
    }
}