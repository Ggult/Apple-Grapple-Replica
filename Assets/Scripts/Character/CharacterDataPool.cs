using System.Collections.Generic;
using UnityEngine;
namespace AppleGrapple
{
    // Predefined AI identities. Get() hands out a random unused entry and removes it, so no two AI end up identical.
    public static class CharacterDataPool
    {
        private static readonly List<CharacterData> _allEntries = new()
        {
            new CharacterData("bigboss_99", Country.TR),
            new CharacterData("american_99", Country.US),
            new CharacterData("chinese_99", Country.CN),
            new CharacterData("japanese_99", Country.JP),
        };

        private static List<CharacterData> _available = new(_allEntries);

        // Call at match start so a previous match's picks don't carry over.
        public static void Reset()
        {
            _available = new List<CharacterData>(_allEntries);
        }

        public static CharacterData Get()
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
