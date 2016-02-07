using Hypermint.Base.Constants;
using MahApps.Metro.Controls;
using Prism.Regions;
using System.Windows;
using System.Windows.Input;

namespace Hypermint.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {                

        public Shell(IRegionManager regionManager)
        {
            InitializeComponent();
            if (regionManager != null)
            {
                SetRegionManager(regionManager, this.flyoutsControlRegion, RegionNames.FlyoutRegion);
            }
        }

        void SetRegionManager(IRegionManager regionManager, DependencyObject regionTarget, string regionName)
        {
            RegionManager.SetRegionName(regionTarget, regionName);
            RegionManager.SetRegionManager(regionTarget, regionManager);
        }

    }
}
