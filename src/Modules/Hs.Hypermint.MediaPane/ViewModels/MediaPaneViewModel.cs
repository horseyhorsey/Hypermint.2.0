using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Events;
using System.IO;
using System.Windows.Media;
using System;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class MediaPaneViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        #region Properties
        private ImageSource wheelSource;
        public ImageSource WheelSource
        {
            get { return wheelSource; }
            set { SetProperty(ref wheelSource, value); }
        }

        private Uri videoSource;
        public Uri VideoSource
        {
            get { return videoSource; }
            set { SetProperty(ref videoSource, value); }
        }

        #endregion

        #region Constructors
        public MediaPaneViewModel(IEventAggregator eventAggregator, ISelectedService selectedService,
            ISettingsRepo settingsRepo)
        {
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;
            _settingsRepo = settingsRepo;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(SetImage);
            _eventAggregator.GetEvent<SetMediaFileRlEvent>().Subscribe(SetMediaForRlAudit);
            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(SetImageGame);
            _eventAggregator.GetEvent<PreviewGeneratedEvent>().Subscribe(SetImageWheelPreview);
            //
        }

        #endregion

        #region Services
        private ISelectedService _selectedService;
        private ISettingsRepo _settingsRepo;
        #endregion        

        #region Methods
        private void SetImageGame(string[] selectedOptions)
        {
            WheelSource = null;
            VideoSource = null;

            var hsPath = _settingsRepo.HypermintSettings.HsPath;
            var romName = selectedOptions[0];
            var mediaTypePath = Images.Wheels;

            if (!string.IsNullOrEmpty(selectedOptions[1]))
            {
                mediaTypePath = getMediaPath(selectedOptions[1]);
            }

            var imagePath = Path.Combine(
                hsPath, Root.Media, _selectedService.CurrentSystem,
                mediaTypePath, romName + ".png");

            if (selectedOptions[1] == "Videos")
            {
                SetVideo(imagePath);
                return;
            }

            if (selectedOptions[1].Contains("Sound") || selectedOptions[1] == "MusicBg")
            {
                SetSound(imagePath);
                return;
            }

            if (!File.Exists(imagePath))
                WheelSource = _selectedService.SystemImage;
            else
            {
                _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(imagePath));
                WheelSource = _selectedService.GameImage;
            }

        }

        private void SetImageWheelPreview(string imagePath)
        {
            WheelSource = null;
            VideoSource = null;

            if (File.Exists(imagePath))
            {
                var fullpath = Path.GetFullPath(imagePath);
                _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(fullpath));
                WheelSource = _selectedService.GameImage;
            }
        }

        private void SetSound(string soundPath)
        {
            var soundsPath = soundPath.Replace(".png", ".mp3");

            if (File.Exists(soundsPath))
            {
                VideoSource = new Uri(soundsPath);                
            }
        }

        private void SetVideo(string pathToImage)
        {
            // Check if there is an mp4 or Flv.
            // If none exists then use the original image path instead.
            var videoPath = pathToImage.Replace(".png", ".mp4");
            if (!File.Exists(videoPath))
            {
                videoPath = videoPath.Replace(".mp4", ".flv");

                if (!File.Exists(videoPath))
                {
                    _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(pathToImage));                    
                    WheelSource = _selectedService.GameImage;
                }

            }
            else
            {                
                VideoSource = new Uri(videoPath);
            }
                        
        }

        private string getMediaPath(string mediaType)
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
                case "Backgrounds":
                    imagePath = Images.Backgrounds;
                    break;
                case "GenreBG":
                    imagePath = Images.GenreBackgrounds;
                    break;
                case "GenreWheel":
                    imagePath = Images.GenreWheel;
                    break;
                case "Letters":
                    imagePath = Images.Letters;
                    break;
                case "Pointer":
                    imagePath = Images.Pointer;
                    break;
                case "Special":
                    imagePath = Images.Special;
                    break;
                case "Videos":
                    imagePath = Root.Video;
                    break;
                case "MusicBg":
                    imagePath = Sound.BackgroundMusic;
                    break;
                case "Sound Start":
                    imagePath = Sound.SystemStart;
                    break;
                case "Sound End":
                    imagePath = Sound.SystemExit;
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

        private void SetMediaForRlAudit(string file)
        {
            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case ".png":
                case ".jpg":
                    SetImageWheelPreview(file);
                    break;
                case ".avi":
                case ".flv":
                case ".mp4":
                    SetVideo(file);
                    break;
                case ".mp3":
                case ".wav":
                    SetSound(file);
                    break;
                default:
                    SetImageWheelPreview("");
                    break;
            }

        }

        #endregion

    }
}
