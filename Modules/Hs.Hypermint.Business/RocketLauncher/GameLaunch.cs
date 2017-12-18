using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.Business.RocketLauncher
{
    public class GameLaunch : IGameLaunch
    {
        public void RocketLaunchGame(string RlPath, string systemName, string RomName, string HsPath)
        {
            var launcher = new Horsesoft.Frontends.Helper.Tools.GameLaunch();
            launcher.RocketLaunchGame(RlPath, systemName, RomName, HsPath);
        }

        public void RocketLaunchGameWithMode(string RlPath, string systemName, string RomName, string mode)
        {
            var launcher = new Horsesoft.Frontends.Helper.Tools.GameLaunch();
            launcher.RocketLaunchGameWithMode(RlPath, systemName, RomName, mode);
        }
    }
}
