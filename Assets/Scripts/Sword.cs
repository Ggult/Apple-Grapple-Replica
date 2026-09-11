using UnityEngine;
using AppleGrapple;

public class Sword : MonoBehaviour
{
    [SerializeField] private WeaponConfig _weaponConfig;

    public int Damage => _weaponConfig.damage;
    public WeaponConfig WeaponConfig => _weaponConfig;
    public SwordOrigin Owner { get; private set; }

    public void SetOwner(SwordOrigin owner) => Owner = owner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatResolver.ReportContact(this, other);
    }
}


