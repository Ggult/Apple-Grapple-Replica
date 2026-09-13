using System.Collections.Generic;
using UnityEngine;

namespace AppleGrapple
{
    public sealed class CharacterRegistry : MonoBehaviour
    {
        public static CharacterRegistry Instance { get; private set; }

        private readonly List<CharacterRoot> _characters = new();
        public IReadOnlyList<CharacterRoot> Characters => _characters;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Register(CharacterRoot character)
        {
            if (character != null && !_characters.Contains(character))
                _characters.Add(character);
        }

        public void Unregister(CharacterRoot character)
        {
            _characters.Remove(character);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}