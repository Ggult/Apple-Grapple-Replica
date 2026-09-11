using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "AppleGrapple/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public Sword weaponPrefab;
    public int damage = 1;
    public float rehitCooldown = 0.25f;
    public float clashPriorityRadius = 0.6f;
}
