namespace AppleGrapple
{
    public static class PlayerProfile
    {
        public static CharacterProfileData Data
        {
            get => PlayerPrefsService.PlayerProfile;
            set
            {
                PlayerPrefsService.PlayerProfile = value;
            }
        }
    }
}