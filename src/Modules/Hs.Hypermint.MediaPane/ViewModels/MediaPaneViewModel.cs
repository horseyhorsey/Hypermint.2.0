using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Events;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Constants;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class MediaPaneViewModel : ViewModelBase
    {
        private ImageSource wheelSource;
        private IEventAggregator _eventAggregator;
        private ISelectedService _selectedService;
        private ISettingsRepo _settingsRepo;

        public ImageSource WheelSource
        {
            get { return wheelSource; }
            set { SetProperty(ref wheelSource, value); }
        }

        public MediaPaneViewModel(IEventAggregator eventAggregator, ISelectedService selectedService,
            ISettingsRepo settingsRepo)
        {
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;
            _settingsRepo = settingsRepo;  

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(SetImage);
            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(SetImageGame);
            //
        }

        private void SetImageGame(string selectedRom)
        {
            var imagePath = Path.Combine(
                _settingsRepo.HypermintSettings.HsPath,                
                Root.Media, _selectedService.CurrentSystem, 
                Images.Wheels, selectedRom + ".png");

            if (!File.Exists(imagePath))
                WheelSource = _selectedService.SystemImage;
            else
            {                
                _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(imagePath));
                WheelSource = _selectedService.GameImage;
            }
                        
        }

        private void SetImage(string obj)
        {
            WheelSource = _selectedService.SystemImage;
        }

    }
}
