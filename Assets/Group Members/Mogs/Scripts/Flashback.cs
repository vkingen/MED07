using UnityEngine;

[System.Serializable]
public class Flashback
{
    public string sceneName; // The name of the flashback scene
    public Vector3 targetPosition; // Position to teleport the player to in the flashback
    public float duration; // Duration of the flashback
}