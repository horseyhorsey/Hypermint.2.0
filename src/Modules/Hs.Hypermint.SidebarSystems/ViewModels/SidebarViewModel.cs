using Hypermint.Base.Base;
using System.Collections.ObjectModel;
using Hypermint.Base.Interfaces;
using System.IO;
using Hypermint.Base.Constants;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;
using Prism.Events;
using Hypermint.Base;
using System;
using System.Runtime.CompilerServices;
using Hypermint.Base.Services;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        #region Properties

        private string _systemWheelImage;
        public string SystemWheelImage
        {
            get { return _systemWheelImage; }
            set { SetProperty(ref _systemWheelImage, value); }
        }

        private string mainMenuDataBaseCount = "Main Menus: ";
        public string MainMenuDataBaseCount
        {
            get { return mainMenuDataBaseCount; }
            set { SetProperty(ref mainMenuDataBaseCount, value); }
        }

        private string systemTextFilter;
        public string SystemTextFilter
        {
            get { return systemTextFilter; }
            set
            {
                SetProperty(ref systemTextFilter, value);
                OnPropertyChanged(() => SystemTextFilter);
            }
        }

        public string SelectedMainMenu { get; set; }        
              
        #endregion

        #region Service Repos
        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;
        #endregion

        private IEventAggregator _eventAggregator;
        private ISelectedService _selectedService;

        protected override void OnPropertyChanged(string propertyName)
        {
            //base.OnPropertyChanged(propertyName);
            if (propertyName == "SystemTextFilter")
            {
                //Publish to SystemsViewModel with SystemFilteredEvent
                _eventAggregator.GetEvent<SystemFilteredEvent>().Publish(SystemTextFilter);
            }
        }

        public SidebarViewModel(IEventAggregator eventAggregator, IMainMenuRepo main, ISettingsRepo settings,
            ISelectedService selectedService)
        {
            _mainMenuRepo = main;            
            _settingsRepo = settings;
            _settingsRepo.LoadHypermintSettings();
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;

            //SelectedMainMenu            

        }

    }
}
