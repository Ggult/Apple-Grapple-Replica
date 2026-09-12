using UnityEngine;
using AppleGrapple;

public class Sword : MonoBehaviour
{
    [SerializeField] private WeaponConfig _weaponConfig;
    [SerializeField] private Transform _scratchPoint;

    public int Damage => _weaponConfig.damage;
    public WeaponConfig WeaponConfig => _weaponConfig;
    public SwordOrigin Owner { get; private set; }
    public Vector3 ScratchPosition => _scratchPoint != null ? _scratchPoint.position : transform.position;

    public void SetOwner(SwordOrigin owner) => Owner = owner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatResolver.ReportContact(this, other);
    }
}


