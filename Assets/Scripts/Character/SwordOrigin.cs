using System.Collections.Generic;
using UnityEngine;
namespace AppleGrapple
{
    [RequireComponent(typeof(CharacterIdentity))]
    public class SwordOrigin : MonoBehaviour
    {
        [SerializeField] private float _radius = 1.5f;
        [SerializeField] private float _orbitSpeed = 90f;
        [SerializeField] private float _slotBlendSpeed = 360f;

        private readonly List<Sword> _weapons = new List<Sword>();
        private readonly List<float> _currentSlotAngles = new List<float>();
        private readonly List<float> _targetSlotAngles = new List<float>();
        private float _orbitAngle;
        private CharacterIdentity _identity;

        public bool IsPlayer => _identity is {IsPlayer: true};

        private void Awake()
        {
            _identity = GetComponent<CharacterIdentity>();
        }

        [ContextMenu("Add Weapon")]
        public void AddWeapon()
        {
            var sword = WeaponPooling.Get();
            sword.SetOwner(this);
            sword.transform.SetParent(transform);
            _weapons.Add(sword);
            _currentSlotAngles.Add(_targetSlotAngles.Count > 0 ? _targetSlotAngles[_targetSlotAngles.Count - 1] : 0f);
            _targetSlotAngles.Add(0f);
            RecalculateSlotTargets();
        }

        private void RecalculateSlotTargets()
        {
            for (int i = 0; i < _weapons.Count; i++)
            {
                _targetSlotAngles[i] = 360f / _weapons.Count * i;
            }
        }
        public void RemoveWeapon(Sword weapon = null)
        {
            if (weapon == null && _weapons.Count > 0)
            {
                weapon = _weapons[_weapons.Count - 1];
            }
            int index = _weapons.IndexOf(weapon);
            if (index >= 0)
            {
                _weapons.RemoveAt(index);
                _currentSlotAngles.RemoveAt(index);
                _targetSlotAngles.RemoveAt(index);
                WeaponPooling.Return(weapon);
                RecalculateSlotTargets();
            }
        }

        private void FixedUpdate()
        {
            if (_weapons.Count == 0) return;

            _orbitAngle = Mathf.Repeat(_orbitAngle + _orbitSpeed * Time.fixedDeltaTime, 360f);

            for (int i = 0; i < _weapons.Count; i++)
            {
                _currentSlotAngles[i] = Mathf.MoveTowardsAngle(_currentSlotAngles[i], _targetSlotAngles[i], _slotBlendSpeed * Time.fixedDeltaTime);

                var angle = _orbitAngle + _currentSlotAngles[i];
                var radians = angle * Mathf.Deg2Rad;
                var offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * _radius;

                _weapons[i].transform.localPosition = offset;
                _weapons[i].transform.localRotation = Quaternion.Euler(0, 0, angle);
                ScratchManager.Scratch(_weapons[i].ScratchPosition);
            }
        }
    }
}
