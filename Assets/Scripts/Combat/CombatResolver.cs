using System.Collections.Generic;
using UnityEngine;
namespace AppleGrapple
{
    // Central referee for all sword contacts: swords only report "I touched this", this decides what happens.
    // Priority: sword-vs-sword clash always wins over sword-vs-health damage. Also suppresses duplicate
    // damage ticks caused by knockback separating/re-overlapping colliders within the same swing.
    public static class CombatResolver
    {
        private static readonly Dictionary<(Sword, Health), float> _lastHitTime = new();

        public static void ReportContact(Sword sword, Collider2D other)
        {
            var otherSword = other.GetComponentInParent<Sword>();
            if (otherSword != null)
            {
                if (otherSword.Owner == sword.Owner) return; // same owner's swords never clash

                ResolveClash(sword, otherSword);
                return;
            }

            var health = other.GetComponentInParent<Health>();
            if (health == null || health.gameObject == sword.Owner.gameObject) return;

            if (HasEnemySwordNearby(sword, health)) return; // let the sword-vs-sword contact win instead
            if (IsOnCooldown(sword, health)) return;

            var hitPosition = sword.transform.position;
            var targetOrigin = health.GetComponent<SwordOrigin>();
            var involvesPlayer = sword.Owner.IsPlayer || (targetOrigin != null && targetOrigin.IsPlayer);
            var hitInfo = new HitInfo(sword.Damage, hitPosition, HitType.EnemyHit, involvesPlayer);
            health.TakeDamage(hitInfo);
            HitFeedback.Raise(hitInfo);
            _lastHitTime[(sword, health)] = Time.time;
        }

        // Call when a sword is returned to its pool so a future, unrelated owner doesn't inherit a stale cooldown.
        public static void ClearCooldowns(Sword sword)
        {
            var keysToRemove = new List<(Sword, Health)>();
            foreach (var key in _lastHitTime.Keys)
            {
                if (key.Item1 == sword) keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
            {
                _lastHitTime.Remove(key);
            }
        }

        private static void ResolveClash(Sword a, Sword b)
        {
            var hitPosition = (a.transform.position + b.transform.position) * 0.5f;
            var hitInfo = new HitInfo(0, hitPosition, HitType.SwordClash,
                a.Owner.IsPlayer || b.Owner.IsPlayer);
            HitFeedback.Raise(hitInfo);

            a.Owner.RemoveWeapon(a);
            b.Owner.RemoveWeapon(b);
        }

        private static bool HasEnemySwordNearby(Sword sword, Health health)
        {
            var enemyOrigin = health.GetComponent<SwordOrigin>();
            if (enemyOrigin == null) return false;

            var hits = Physics2D.OverlapCircleAll(sword.transform.position, sword.WeaponConfig.clashPriorityRadius);
            foreach (var hit in hits)
            {
                var enemySword = hit.GetComponentInParent<Sword>();
                if (enemySword != null && enemySword.Owner == enemyOrigin) return true;
            }
            return false;
        }

        private static bool IsOnCooldown(Sword sword, Health health)
        {
            return _lastHitTime.TryGetValue((sword, health), out var lastHit)
                && Time.time - lastHit < sword.WeaponConfig.rehitCooldown;
        }
    }
}
