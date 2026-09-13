using UnityEngine;

namespace AppleGrapple
{
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

        [Header("AI")]
        [Range(0f, 1f)] public float aiSmartValue = 0.5f;
    }
}
