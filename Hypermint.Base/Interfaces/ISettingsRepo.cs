using Hs.Hypermint.Settings;

namespace Hypermint.Base.Interfaces
{
    public interface ISettingsRepo
    {
        Setting HypermintSettings { get; set; }

        void LoadHypermintSettings();

        void SaveHypermintSettings();          
    }
}
