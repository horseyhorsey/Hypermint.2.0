using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using Hypermint.Shell.Models;
using Hypermint.Base.Interfaces;
using Hs.Hypermint.Settings;
using System;
using Hypermint.Base.Services;

namespace Hypermint.Shell.ViewModels
{
    public class SettingsFlyoutViewModel : ViewModelBase
    {        

        private ISettingsRepo _hyperMintSettings;
        public Setting HyperMintSettings
        {            
            get { return _hyperMintSettings.HypermintSettings; }
            set { _hyperMintSettings.HypermintSettings = value; }
        }

        private IFindDirectoryService _findDirectoryService;

        public ObservableCollection<string> GuiThemes { get; set; }        

        #region Delegate Commands
        public DelegateCommand SaveSettings { get; private set; }             
        public DelegateCommand<string> FindPath { get; set; }
        public DelegateCommand<string> UpdateProperty { get; set; }
        #endregion

        #region Ctors
        public SettingsFlyoutViewModel(ISettingsRepo settings, IFindDirectoryService findDir)
        {
           // if (settings == null) throw new ArgumentNullException("settings");
            //_eventAggregator = eventAggregator;     
            _hyperMintSettings = settings;
            _findDirectoryService = findDir;
            HyperMintSettings = new Setting();

            //Loading this through other module. 
            //getSavedHypermintSettings();

            HyperMintSettings = _hyperMintSettings.HypermintSettings;

            CurrentThemeColor = Properties.Settings.Default.GuiColor;
            IsDarkTheme = Properties.Settings.Default.GuiTheme;

            //Setup themes for combobox binding            
            GuiThemes = new ObservableCollection<string>(MahAppTheme.AvailableThemes);
            
            //setup commands
            SaveSettings = new DelegateCommand(SaveGuiSettings);
            FindPath = new DelegateCommand<string>(LocatePath);            

        }

        /// <summary>
        /// Locate directory to set with Windows.Forms Folder Dialog
        /// </summary>
        /// <param name="pathName"></param>
        private void LocatePath(string pathName)
        {
            var userPath = "";

            _findDirectoryService.setFolderDialog();

            if (!string.IsNullOrEmpty(_findDirectoryService.SelectedFolder))
            {
                userPath = _findDirectoryService.SelectedFolder;

                UpdatePath(pathName, userPath);
            }

        }
        #endregion

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
                case "ImPath":
                    HyperMintSettings.ImageMagickPath = FolderName;
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

        #region Theme Properties            
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
