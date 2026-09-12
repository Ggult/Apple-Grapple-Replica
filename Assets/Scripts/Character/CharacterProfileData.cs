namespace AppleGrapple
{
    [System.Serializable]
    public struct CharacterProfileData
    {
        public string Nickname;
        public Country Country;

        public CharacterProfileData(string nickname, Country country)
        {
            Nickname = nickname;
            Country = country;
        }

        // Matches the flag sprite file names (tr.png, us.png, cn.png, jp.png) for later UI wiring.
        public string FlagCode => Country.ToString().ToLowerInvariant();
    }
}
