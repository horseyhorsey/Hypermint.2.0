using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;

namespace Hs.Hypermint.Settings
{
    public class SettingsFlyoutViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        private IEventAggregator _eventAggregator;

        public SettingsFlyoutViewModel()
        {

        }

        #region Delegate Commands
        public DelegateCommand SetTheme { get; private set; }
        #endregion

        #region Ctors
        public SettingsFlyoutViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            //if (settings == null) throw new ArgumentNullException("settings");
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            //_hyperMintSettings = settings;
            ToggleFlyout(0);

            //CurrentThemeColor = Properties.Settings.Default.GuiColor;
            //IsDarkTheme = Properties.Settings.Default.GuiTheme;

            //Setup themes for combobox binding
            //var mahAppTheme = new Models.MahAppTheme();
            //GuiThemes = new ObservableCollection<string>(mahAppTheme.AvailableThemes);

            //changeGuiTheme();

            SetTheme = new DelegateCommand(SaveGuiSettings);

        }
        #endregion

        private void ToggleFlyout(int index)
        {
            //var flyout = Flyouts.Items[index] as Flyout;
            //if (flyout == null)
            //{
            //    return;
            //}

            //flyout.IsOpen = !flyout.IsOpen;
        }

        #region Theme Properties
        public ObservableCollection<string> GuiThemes { get; set; }

        private string _currentThemeColor;
        public string CurrentThemeColor
        {
            get { return _currentThemeColor; }
            set
            {
                SetProperty(ref _currentThemeColor, value);
                changeGuiTheme();
            }
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                SetProperty(ref _isDarkTheme, value);
                changeGuiTheme();
            }
        }
        #endregion

        #region Theme Methods
        private void changeGuiTheme()
        {
            string darkOrLight = string.Empty;
            if (IsDarkTheme)
                darkOrLight = "BaseDark";
            else
                darkOrLight = "BaseLight";

            // now set the theme
            MahApps.Metro.ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                        MahApps.Metro.ThemeManager.GetAccent(CurrentThemeColor),
                                        MahApps.Metro.ThemeManager.GetAppTheme(darkOrLight));
        }

        public void SaveGuiSettings()
        {
            Properties.Settings.Default.GuiColor = CurrentThemeColor;
            Properties.Settings.Default.GuiTheme = IsDarkTheme;
            Properties.Settings.Default.Save();
        }
        #endregion
        
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("ContentRegion", uri);
        }
    }
}
