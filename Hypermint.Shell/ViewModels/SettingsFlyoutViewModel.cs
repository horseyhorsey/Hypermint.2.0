using Hypermint.Base;
using Prism.Commands;
using System.Collections.ObjectModel;
using Hypermint.Shell.Models;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Hypermint.Base.Model;

namespace Hypermint.Shell.ViewModels
{
    public class SettingsFlyoutViewModel : ViewModelBase
    {

        private IFileDialogHelper _fileFolderService;
        private ISettingsHypermint _hyperMintSettings;

        #region Delegate Commands
        public DelegateCommand SaveSettings { get; private set; }
        public DelegateCommand<string> FindPath { get; set; }
        public DelegateCommand<string> UpdateProperty { get; set; }
        #endregion

        #region Ctors
        public SettingsFlyoutViewModel(IFileDialogHelper findDir, ISettingsHypermint settings)
        {
            // if (settings == null) throw new ArgumentNullException("settings");
            //_eventAggregator = eventAggregator;     

            _hyperMintSettings = settings;
            _fileFolderService = findDir;

            //Load hypermint settings
            if (_hyperMintSettings.HypermintSettings == null)
            {
                _hyperMintSettings.LoadHypermintSettings();
            }
                
            HyperMintSettings = _hyperMintSettings.HypermintSettings;
            
            //set the ui theme
            CurrentThemeColor = Properties.Settings.Default.GuiColor;
            IsDarkTheme = Properties.Settings.Default.GuiTheme;

            //Setup themes for combobox binding            
            GuiThemes = new ObservableCollection<string>(MahAppTheme.AvailableThemes);

            //setup commands
            SaveSettings = new DelegateCommand(SaveGuiSettings);
            FindPath = new DelegateCommand<string>(LocatePath);

        }

        #endregion

        #region Properties            
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

        public ObservableCollection<string> GuiThemes { get; set; }

        public Setting HyperMintSettings
        {
            get { return _hyperMintSettings.HypermintSettings; }
            set { _hyperMintSettings.HypermintSettings = value; }
        }
        #endregion

        #region Support Methods

        /// <summary>
        /// Update the local model to reflect changes to UI
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="FolderName"></param>
        public void UpdatePath(string pathName, string FolderName)
        {
            switch (pathName)
            {
                case "HsPath":
                    HyperMintSettings.HsPath = FolderName;
                    break;
                case "RlPath":
                    HyperMintSettings.RlPath = FolderName;
                    break;
                case "RlMediaPath":
                    HyperMintSettings.RlMediaPath = FolderName;
                    break;
                case "GsPath":
                    HyperMintSettings.GhostscriptPath = FolderName;
                    break;
                default:
                    break;
            };

        }

        private void getSavedHypermintSettings()
        {
            _hyperMintSettings.LoadHypermintSettings();
        }

        private void saveHypermintSettings()
        {
            _hyperMintSettings.SaveHypermintSettings();

        }


        /// <summary>
        /// Locate directory to set with Windows.Forms Folder Dialog
        /// </summary>
        /// <param name="pathName"></param>
        private void LocatePath(string pathName)
        {
            var userPath = "";

            _fileFolderService.SetFolderDialog();

            if (!string.IsNullOrEmpty(_fileFolderService.SelectedFolder))
            {
                userPath = _fileFolderService.SelectedFolder;

                UpdatePath(pathName, userPath);
            }

        }

        private void changeGuiTheme()
        {
            string darkOrLight = string.Empty;
            if (IsDarkTheme)
                darkOrLight = "BaseDark";
            else
                darkOrLight = "BaseLight";

            // now set the theme
            try
            {
                MahApps.Metro.ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                            MahApps.Metro.ThemeManager.GetAccent(CurrentThemeColor),
                            MahApps.Metro.ThemeManager.GetAppTheme(darkOrLight));
            }
            catch (System.Exception)
            {


            }

        }

        public void SaveGuiSettings()
        {
            Properties.Settings.Default.GuiColor = CurrentThemeColor;
            Properties.Settings.Default.GuiTheme = IsDarkTheme;
            Properties.Settings.Default.Save();

            _hyperMintSettings.HypermintSettings = HyperMintSettings;
            saveHypermintSettings();
        }
        #endregion

    }
}
