namespace Hypermint.Base.Constants
{
    public static class Root
    {
        public const string Databases = "Databases";
        public const string Media = "Media";
        public const string Settings = "Settings";
        public const string Sound = "Sound";
        public const string Themes = "Themes";
        public const string Video = "Video";
    }

    public static class Images
    {
        public const string Artwork1 = @"Images\Artwork1";
        public const string Artwork2 = @"Images\Artwork2";
        public const string Artwork3 = @"Images\Artwork3";
        public const string Artwork4 = @"Images\Artwork4";
        public const string Backgrounds = @"Images\Backgrounds";
        public const string GenreWheel = @"Images\Genre\Wheel";
        public const string GenreBackgrounds = @"Images\Genre\Backgrounds";
        public const string Letters = @"Images\Letters";
        public const string Pointer = @"Images\Other";
        public const string Special = @"Images\Special";
        public const string Wheels = @"Images\Wheel";                
    }

    public static class Sound
    {
        public const string WheelSounds = @"Sound\Wheel Sounds";
        public const string BackgroundMusic = @"Sound\Background Music";
        public const string SystemStart = @"Sound\System Start";
        public const string SystemExit = @"Sound\System Exit";

    }

    public enum MediaType
    {

    }

    public enum MediaSection
    {
        Images,
        Sound,
        Themes,
        Video
    }
}
