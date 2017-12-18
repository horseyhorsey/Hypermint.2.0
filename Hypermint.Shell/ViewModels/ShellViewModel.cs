using System;
using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;

namespace Hypermint.Shell.ViewModels
{
    public class ShellViewModel : ViewModelBase, IDropTarget
    {
        public DelegateCommand<string> NavigateCommand { get; set; }

        #region Fields
        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IHyperspinManager _hsManager;
        private ISelectedService _service;
        #endregion

        #region Constructor
        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            ISelectedService service, IHyperspinManager hsManager, ISettingsHypermint settings)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _hsManager = hsManager;
            _service = service;

            LoadSettings(settings);

            NavigateCommand = new DelegateCommand<string>(Navigate);

            _eventAggregator.GetEvent<ErrorMessageEvent>().Subscribe(DisplayError);
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
            //System.Windows.MessageBox.Show(error);
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
            _eventAggregator.GetEvent<NavigateRequestEvent>().Publish(uri);
        }

        #endregion
    }
}
