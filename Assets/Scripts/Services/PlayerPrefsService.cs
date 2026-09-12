using System;
using UnityEngine;

namespace AppleGrapple
{
    public static class PlayerPrefsService
    {
        private const string NicknameKey = "PlayerIdentity.Nickname";
        private const string CountryKey = "PlayerIdentity.Country";
        private const string SelectedEnemyCountKey = "Game.SelectedEnemyCount";

        private const string DefaultNickname = "Player";
        private const Country DefaultCountry = Country.TR;
        private const int DefaultSelectedEnemyCount = 3;

        private static string Nickname
        {
            get => PlayerPrefs.GetString(NicknameKey, DefaultNickname);
            set
            {
                PlayerPrefs.SetString(NicknameKey, string.IsNullOrWhiteSpace(value) ? DefaultNickname : value);
                PlayerPrefs.Save();
            }
        }

        private static Country Country
        {
            get
            {
                var countryName = PlayerPrefs.GetString(CountryKey, DefaultCountry.ToString());
                return Enum.TryParse(countryName, out Country country) ? country : DefaultCountry;
            }
            set
            {
                PlayerPrefs.SetString(CountryKey, value.ToString());
                PlayerPrefs.Save();
            }
        }

        public static int SelectedEnemyCount
        {
            get => PlayerPrefs.GetInt(SelectedEnemyCountKey, DefaultSelectedEnemyCount);
            set
            {
                PlayerPrefs.SetInt(SelectedEnemyCountKey, Mathf.Max(0, value));
                PlayerPrefs.Save();
            }
        }

        public static CharacterProfileData PlayerProfile
        {
            get => new(Nickname, Country);
            set
            {
                Nickname = value.Nickname;
                Country = value.Country;
            }
        }
    }
}
