using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    private Image flashImage;

    private void Awake()
    {
        flashImage = GetComponent<Image>();
        flashImage.color = new Color(1, 1, 1, 0); // Ensure it's initially transparent
    }

    public void TriggerFlash(float duration)
    {
        StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        // Fade in
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / (duration / 2));
            flashImage.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fade out
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(1 - (elapsedTime / (duration / 2)));
            flashImage.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully transparent at the end
        flashImage.color = new Color(1, 1, 1, 0);
    }
}