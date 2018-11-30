using System;
using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Prism.Logging;
using Hypermint.Base.Constants;

namespace Hypermint.Shell.ViewModels
{
    public class ShellViewModel : HypermintViewModelBase, IDropTarget
    {
        public DelegateCommand<string> NavigateCommand { get; set; }

        #region Fields
        private readonly IRegionManager _regionManager;
        private IHyperspinManager _hsManager;
        private ISelectedService _service;
        #endregion

        #region Constructor
        public ShellViewModel(ILoggerFacade loggerFacade, IRegionManager regionManager, IEventAggregator eventAggregator,
            ISelectedService service, IHyperspinManager hsManager, ISettingsHypermint settings) : base (loggerFacade)
        {
            _regionManager = regionManager;
            _hsManager = hsManager;
            _service = service;

            LoadSettings(settings);

            NavigateCommand = new DelegateCommand<string>(Navigate);

            //TODO: Remove this and all subscribers and replace with logging.
            eventAggregator.GetEvent<ErrorMessageEvent>().Subscribe(DisplayError);
        }

        #endregion

        #region Properties
        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }
        #endregion

        #region Support Methods
        private void DisplayError(string error)
        {
            ErrorMessage = error;
            Log(error, Category.Warn, Priority.None);
        }
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the settings from config into the settings service
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void LoadSettings(ISettingsHypermint settings)
        {
            Log("", Category.Debug, Priority.None);
            //Create settings
            settings.HypermintSettings =
                new Setting
                {                    
                    Author = Properties.Settings.Default.Author,
                    Ffmpeg = Properties.Settings.Default.Ffmpeg,
                    GhostscriptPath = Properties.Settings.Default.GhostscriptPath,
                    HsPath = Properties.Settings.Default.Hyperspin,
                    Icons = Properties.Settings.Default.Icons,
                    RlPath = Properties.Settings.Default.Rocketlauncher,
                    RlMediaPath = Properties.Settings.Default.RocketlauncherMedia,
                };
        }

        private void Navigate(string uri)
        {
            Log("", Category.Debug, Priority.None);
            _regionManager.RequestNavigate(RegionNames.ContentRegion, uri);
        }

        #endregion
    }
}
