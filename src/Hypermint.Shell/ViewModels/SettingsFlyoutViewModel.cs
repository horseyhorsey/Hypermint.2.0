using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace Hypermint.Shell.ViewModels
{
    public class SettingsFlyoutViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand SetTheme { get; private set; }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        private IEventAggregator _eventAggregator;
        
        public SettingsFlyoutViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            SetTheme = new DelegateCommand(SetGuiTheme);
            _eventAggregator.GetEvent<SaveSettingsEvent>().Subscribe(SetMessage);
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("ContentRegion", uri);
        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }

        public void SetGuiTheme()
        {
            System.Windows.MessageBox.Show("");
        }

        #region GUI Colors & Settings
        //public void changeguiColor(string color, bool dark)
        //{
        //    string darkOrLight = string.Empty;
        //    if (dark)
        //        darkOrLight = "BaseDark";
        //    else
        //        darkOrLight = "BaseLight";
        //    // get the theme from the current application
        //    var theme = MahApps.Metro.ThemeManager.DetectAppStyle(System.Windows.Application.Current);

        //    // now set the Green accent and dark theme
        //    MahApps.Metro.ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
        //                                MahApps.Metro.ThemeManager.GetAccent(color),
        //                                MahApps.Metro.ThemeManager.GetAppTheme(darkOrLight));

        //    Properties.Settings.Default.guiColor = color;
        //    Properties.Settings.Default.guiTheme = dark;
        //    Properties.Settings.Default.Save();
        //}
        //private void comboStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    changeguiColor(getColor(), getTheme());
        //}
        //private void guiThemeCheckbox_Checked(object sender, RoutedEventArgs e)
        //{
        //    changeguiColor(getColor(), getTheme());
        //}
        //private void guiThemeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    changeguiColor(getColor(), getTheme());
        //}
        //private string getColor()
        //{
        //    if (comboStyles.SelectedItem != null)
        //        return comboStyles.SelectedItem.ToString();
        //    else if (Properties.Settings.Default.guiColor != string.Empty)
        //        return Properties.Settings.Default.guiColor;
        //    else
        //        return "Red";
        //}
        //private bool getTheme()
        //{
        //    bool dark = false;
        //    if (guiThemeCheckbox.IsChecked.Value == false)
        //        dark = false;
        //    else
        //        dark = true;
        //    return dark;
        //}
        #endregion
    }
}
