using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Regions;

namespace Hypermint.Shell.Views
{
    /// <summary>
    /// Interaction logic for SettingsFlyout.xaml
    /// </summary>
    public partial class SettingsFlyout :IFlyoutView
    {
        public SettingsFlyout()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// The flyout name
        /// </summary>
        public string FlyoutName
        {
            get { return FlyoutNames.SettingsFlyout; }
        }

    }
}
