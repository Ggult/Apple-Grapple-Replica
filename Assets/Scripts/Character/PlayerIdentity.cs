using System;
using UnityEngine;
namespace AppleGrapple
{
    // Shared PlayerPrefs keys so the (future) identity-selection UI and CharacterView agree on where data lives.
    public static class PlayerIdentity
    {
        private const string NicknameKey = "PlayerIdentity.Nickname";
        private const string CountryKey = "PlayerIdentity.Country";
        private const string DefaultNickname = "Player";
        private const Country DefaultCountry = Country.TR;

        public static CharacterData Load()
        {
            var nickname = PlayerPrefs.GetString(NicknameKey, DefaultNickname);
            var countryName = PlayerPrefs.GetString(CountryKey, DefaultCountry.ToString());
            var country = Enum.TryParse<Country>(countryName, out var parsed) ? parsed : DefaultCountry;
            return new CharacterData(nickname, country);
        }

        public static void Save(CharacterData data)
        {
            PlayerPrefs.SetString(NicknameKey, data.Nickname);
            PlayerPrefs.SetString(CountryKey, data.Country.ToString());
            PlayerPrefs.Save();
        }
    }
}
