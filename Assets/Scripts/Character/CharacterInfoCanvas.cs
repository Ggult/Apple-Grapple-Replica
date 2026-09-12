using UnityEngine;
namespace AppleGrapple
{
    // Single shared Canvas that every character's info view lives under — one batching root instead of one per character.
    public class CharacterInfoCanvas : MonoBehaviour
    {
        public static CharacterInfoCanvas Instance { get; private set; }

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Camera _worldCamera;

        public RectTransform RectTransform => _rectTransform;
        public Camera WorldCamera => _worldCamera;

        private void Awake()
        {
            Instance = this;
        }
    }
}
