using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "AppleGrapple/Character Config")]
public class CharacterConfig : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed = 5f;

    [Header("Stats")]
    public int maxHealth = 3;

    [Header("Hit Reaction")]
    public float knockbackForce = 2f;
    public float knockbackDuration = 0.12f;
}
