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

            _hyperMintSettings = settings;
            _fileFolderService = findDir;
                
            HyperMintSettings = _hyperMintSettings.HypermintSettings;
            
            //set the ui theme
            CurrentThemeColor = Properties.Settings.Default.GuiColor;
            IsDarkTheme = Properties.Settings.Default.GuiTheme;

            //Setup themes for combobox binding            
            GuiThemes = new ObservableCollection<string>(MahAppTheme.AvailableThemes);

            //setup commands
            SaveSettings = new DelegateCommand(SaveUiSettings);
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
                ChangeUiTheme();
            }
        }

        private bool _isDarkTheme;        
        public bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                SetProperty(ref _isDarkTheme, value);
                ChangeUiTheme();
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
        /// Changes the UI theme.
        /// </summary>
        private void ChangeUiTheme()
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
            catch { }

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

        public void SaveUiSettings()
        {
            Properties.Settings.Default.GuiColor = CurrentThemeColor;
            Properties.Settings.Default.GuiTheme = IsDarkTheme;
            Properties.Settings.Default.RocketlauncherMedia = _hyperMintSettings.HypermintSettings.RlMediaPath;
            Properties.Settings.Default.Hyperspin = _hyperMintSettings.HypermintSettings.HsPath;
            Properties.Settings.Default.Rocketlauncher = _hyperMintSettings.HypermintSettings.RlPath;
            Properties.Settings.Default.Icons = _hyperMintSettings.HypermintSettings.Icons;
            Properties.Settings.Default.GhostscriptPath = _hyperMintSettings.HypermintSettings.GhostscriptPath;
            Properties.Settings.Default.Ffmpeg = _hyperMintSettings.HypermintSettings.Ffmpeg;
            Properties.Settings.Default.Author = _hyperMintSettings.HypermintSettings.Author;
            Properties.Settings.Default.Save();            
        }

        /// <summary>
        /// Update the local model to reflect changes to UI
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="dirName"></param>
        public void UpdatePath(string pathName, string dirName)
        {
            switch (pathName)
            {
                case "HsPath":
                    HyperMintSettings.HsPath = dirName;
                    break;
                case "RlPath":
                    HyperMintSettings.RlPath = dirName;
                    break;
                case "RlMediaPath":
                    HyperMintSettings.RlMediaPath = dirName;
                    break;
                case "GsPath":
                    HyperMintSettings.GhostscriptPath = dirName;
                    break;
                case "Ffmpeg":
                    HyperMintSettings.Ffmpeg = dirName;
                    break;
                default:
                    break;
            };

        }

        #endregion

    }
}
