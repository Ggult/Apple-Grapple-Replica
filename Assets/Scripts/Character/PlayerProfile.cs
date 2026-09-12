using System;
using UnityEngine;

namespace AppleGrapple
{
    public static class PlayerProfile
    {
        private const string NicknameKey = "PlayerIdentity.Nickname";
        private const string CountryKey = "PlayerIdentity.Country";
        private const string DefaultNickname = "Player";
        private const Country DefaultCountry = Country.TR;
        public static CharacterProfileData Data
        {
            get
            {
                var nickname = PlayerPrefs.GetString(NicknameKey, DefaultNickname);
                var countryName = PlayerPrefs.GetString(CountryKey, DefaultCountry.ToString());
                var country = Enum.TryParse<Country>(countryName, out var parsed) ? parsed : DefaultCountry;
                return new CharacterProfileData(nickname, country);
            }
            set
            {
                PlayerPrefs.SetString(NicknameKey, value.Nickname);
                PlayerPrefs.SetString(CountryKey, value.Country.ToString());
                PlayerPrefs.Save();
            }
        }
    }
}