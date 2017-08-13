namespace Hypermint.Base.Interfaces
{
    public interface IGameLaunch
    {
        void RocketLaunchGame(string RlPath, string systemName, string RomName, string HsPath);

        void RocketLaunchGameWithMode(string RlPath, string systemName, string RomName, string mode);
    }
}
