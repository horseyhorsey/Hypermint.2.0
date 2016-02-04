namespace Hypermint.Base.Interfaces
{
    public interface ISettingsRepo
    {
        string HyperSpinPath { get; set; }

        string RocketLauncherPath { get; set; }

        string GetHsPathFromSettings();

        string GetRlPathFromSettings();

        bool SaveHsPathToSettings();

        bool SaveRlPathToSettings();
    }
}
