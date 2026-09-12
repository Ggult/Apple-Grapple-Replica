using System;
using UnityEngine;

namespace AppleGrapple
{
    public enum HitType
    {
        SwordClash,
        EnemyHit
    }

    public readonly struct HitInfo
    {
        public readonly int DamageAmount;
        public readonly Vector3 Position;
        public readonly HitType HitType;
        public readonly bool InvolvesPlayer;

        public HitInfo(int damageAmount, Vector3 position, HitType hitType, bool involvesPlayer)
        {
            DamageAmount = damageAmount;
            Position = position;
            HitType = hitType;
            InvolvesPlayer = involvesPlayer;
        }
    }

    public static class HitFeedback
    {
        public static event Action<HitInfo> Hit;

        public static void Raise(HitInfo hitInfo)
        {
            Hit?.Invoke(hitInfo);
        }
    }
}
