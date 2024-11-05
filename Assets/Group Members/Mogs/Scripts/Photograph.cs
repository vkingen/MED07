using UnityEngine;

public enum PhotographType
{
    Murderer,
    Victim,
    Weapon,
    Course
}

public class Photograph : MonoBehaviour
{
    public PhotographType type; // Set this in the Inspector
}