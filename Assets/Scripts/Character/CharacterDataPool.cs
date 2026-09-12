using System.Collections.Generic;
using UnityEngine;
namespace AppleGrapple
{
    // Predefined AI identities. Get() hands out a random unused entry and removes it, so no two AI end up identical.
    public static class CharacterDataPool
    {
        private static readonly List<CharacterProfileData> _allEntries = new()
        {
            new CharacterProfileData("AnatolianAce", Country.TR),
            new CharacterProfileData("BosporusBlade", Country.TR),
            new CharacterProfileData("CappadociaClash", Country.TR),
            new CharacterProfileData("IstanbulDash", Country.TR),
            new CharacterProfileData("RedCrescent", Country.TR),
            new CharacterProfileData("TurkishThunder", Country.TR),

            new CharacterProfileData("LibertyRush", Country.US),
            new CharacterProfileData("DesertRanger", Country.US),
            new CharacterProfileData("IronEagle", Country.US),
            new CharacterProfileData("LoneStriker", Country.US),
            new CharacterProfileData("NeonHustler", Country.US),
            new CharacterProfileData("WildFrontier", Country.US),

            new CharacterProfileData("JadeRunner", Country.CN),
            new CharacterProfileData("DragonPulse", Country.CN),
            new CharacterProfileData("SilkStorm", Country.CN),
            new CharacterProfileData("PandaFury", Country.CN),
            new CharacterProfileData("GreatWall", Country.CN),
            new CharacterProfileData("RedLantern", Country.CN),

            new CharacterProfileData("RisingSun", Country.JP),
            new CharacterProfileData("RoninRush", Country.JP),
            new CharacterProfileData("NeonSamurai", Country.JP),
            new CharacterProfileData("SakuraStrike", Country.JP),
            new CharacterProfileData("ShogunEdge", Country.JP),
            new CharacterProfileData("TokyoFlash", Country.JP),
        };

        private static List<CharacterProfileData> _available = new(_allEntries);

        // Call at match start so a previous match's picks don't carry over.
        public static void Reset()
        {
            _available = new List<CharacterProfileData>(_allEntries);
        }

        public static CharacterProfileData Get()
        {
            if (_available.Count == 0)
            {
                Reset(); // more requests than predefined entries: recycle rather than throw
            }

            var index = Random.Range(0, _available.Count);
            var data = _available[index];
            _available.RemoveAt(index);
            return data;
        }
    }
}
