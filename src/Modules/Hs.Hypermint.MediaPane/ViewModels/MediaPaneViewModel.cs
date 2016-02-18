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

        private void SetImageGame(string[] selectedOptions)
        {
            var hsPath = _settingsRepo.HypermintSettings.HsPath;
            var romName = selectedOptions[0];
            var mediaTypePath = Images.Wheels;

            if (!string.IsNullOrEmpty(selectedOptions[1]))
            {
                mediaTypePath = getImagePath(selectedOptions[1]);
            }

            var imagePath = Path.Combine(
                hsPath, Root.Media, _selectedService.CurrentSystem,
                mediaTypePath, romName + ".png");

            if (!File.Exists(imagePath))
                WheelSource = _selectedService.SystemImage;
            else
            {                
                _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(imagePath));
                WheelSource = _selectedService.GameImage;
            }
                        
        }

        private string getImagePath (string mediaType)
        {
            var imagePath = "";

            switch (mediaType)
            {
                case "Wheel":
                    imagePath = Images.Wheels;
                    break;
                case "Artwork1":
                    imagePath = Images.Artwork1;
                    break;
                case "Artwork2":
                    imagePath = Images.Artwork2;
                    break;
                case "Artwork3":
                    imagePath = Images.Artwork3;
                    break;
                case "Artwork4":
                    imagePath = Images.Artwork4;
                    break;                    
                default:
                    break;
            }

            return imagePath;
        }

        private void SetImage(string obj)
        {
            WheelSource = _selectedService.SystemImage;
        }

    }
}
