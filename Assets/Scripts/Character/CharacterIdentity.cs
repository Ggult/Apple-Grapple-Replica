using UnityEngine;

namespace AppleGrapple
{
    public class CharacterIdentity : MonoBehaviour
    {
        [SerializeField] private bool _isPlayer;
        public bool IsPlayer => _isPlayer;
        public CharacterProfileData Data => _isPlayer ? PlayerProfile.Data : CharacterDataPool.Get();
    }
}
