﻿using Hypermint.Base;
using Prism.Events;
using System.IO;
using System.Windows.Media;
using System;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Prism.Commands;
using Hypermint.Base.Helpers;
using Prism.Regions;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class MediaPaneViewModel : ViewModelBase
    {
        #region Constructors
        public MediaPaneViewModel(IEventAggregator eventAggregator, ISelectedService selectedService,
            ISettingsHypermint settingsRepo, IPdfService pdfService, IRegionManager rm)
        {
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;
            _settingsRepo = settingsRepo;
            _pdfService = pdfService;
            _regionManager = rm;

            PagePdfCommand = new DelegateCommand<string>((x) =>
            {
                PagePdf(x);
            });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(
                (x) =>
                {
                    WheelSource = null;
                    VideoSource = null;
                    IsVideoSource = false;

                    SetImage(x);
                });

            _eventAggregator.GetEvent<SetMediaFileRlEvent>().Subscribe((x) =>
            {
                string activeViewName = RegionHelper.GetCurrentViewName(_regionManager);

                if (!activeViewName.Contains("HsMediaAuditView") || !activeViewName.Contains("DatabaseDetailsView"))
                {
                    IsVideoSource = false;
                    SetMediaFromFileType(x);
                }
            });

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(SetMediaForGameHs);
            _eventAggregator.GetEvent<PreviewGeneratedEvent>().Subscribe(SetMediaFromFileType);

            ImageEditCommand = new DelegateCommand(() =>
            {
                if (!File.Exists(currentImagePath)) return;

                _eventAggregator.GetEvent<NavigateRequestEvent>().Publish("CreateImageView");

                _eventAggregator.GetEvent<ImageEditSourceEvent>().Publish(currentImagePath);
            });

            OpenVideoEditorCommand = new DelegateCommand(() =>
            {
                if (!File.Exists(VideoSource.LocalPath)) return;                

                _eventAggregator.GetEvent<NavigateRequestEvent>().Publish("VideoEditView");
                _eventAggregator.GetEvent<VideoSourceEvent>().Publish(VideoSource.LocalPath);
            });

            ChangeMediaCommand = new DelegateCommand(() =>
            {
                SetMediaForGameHs(new string[] { _selectedService.CurrentRomname, "Videos" });
            });
          
        }

        #endregion

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

        private string textSource;
        public string TextSource
        {
            get { return textSource; }
            set { SetProperty(ref textSource, value); }
        }

        private bool isTextSource = false;
        public bool IsTextSource
        {
            get { return isTextSource; }
            set { SetProperty(ref isTextSource, value); }
        }

        private bool isSwfSource = false;
        public bool IsSwfSource
        {
            get { return isSwfSource; }
            set { SetProperty(ref isSwfSource, value); }
        }

        private bool isImageSource = false;
        public bool IsImageSource
        {
            get { return isImageSource; }
            set { SetProperty(ref isImageSource, value); }
        }

        private bool isPdf;
        public bool IsPdf
        {
            get { return isPdf; }
            set { SetProperty(ref isPdf, value); }
        }

        private bool isVideoSource;
        public bool IsVideoSource
        {
            get { return isVideoSource; }
            set { SetProperty(ref isVideoSource, value); }
        }

        private int pdfPageCount;
        public int PdfPageCount
        {
            get { return pdfPageCount; }
            set { SetProperty(ref pdfPageCount, value); }
        }

        private int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set { SetProperty(ref currentPage, value); }
        }

        private string mediaPaneHeader = "Media View";
        public string MediaPaneHeader
        {
            get { return mediaPaneHeader; }
            set { SetProperty(ref mediaPaneHeader, value); }
        }

        private string swfFileSource;
        public string SwfFileSource
        {
            get { return swfFileSource; }
            set { SetProperty(ref swfFileSource, value); }
        }
        public string currentImagePath { get; set; }

        //timelineSlider.Maximum

        #endregion

        #region Services
        private IEventAggregator _eventAggregator;
        private ISelectedService _selectedService;
        private ISettingsHypermint _settingsRepo;
        private IPdfService _pdfService;
        private string currentPdfFile;
        private IRegionManager _regionManager;
        #endregion

        #region Commands
        public DelegateCommand<string> PagePdfCommand { get; private set; }
        public DelegateCommand ImageEditCommand { get; private set; }
        public DelegateCommand ChangeMediaCommand { get; private set; }
        public ICommand OpenVideoEditorCommand { get; set; }
        
        #endregion

        #region Support Methods

        private string GetMediaPath(string mediaType)
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
        private void PagePdf(string direction)
        {
            if (direction == "forward")
            {
                if (CurrentPage < PdfPageCount)
                {
                    CurrentPage++;
                }
            }
            else
            {
                if (CurrentPage > 1)
                {
                    CurrentPage--;
                }
            }

            SetPdfImage(currentPdfFile, CurrentPage - 1);
        }
        private void SetMediaForGameHs(string[] selectedOptions)
        {
            WheelSource = null;
            VideoSource = null;
            IsVideoSource = false;
            IsImageSource = false;
            IsPdf = false;

            var hsPath = _settingsRepo.HypermintSettings.HsPath;
            var romName = selectedOptions[0];

            _selectedService.CurrentRomname = romName;

            MediaPaneHeader = "Media View: " + selectedOptions[1];

            var mediaTypePath = Images.Wheels;

            if (!string.IsNullOrEmpty(selectedOptions[1]))
            {
                mediaTypePath = GetMediaPath(selectedOptions[1]);
            }

            //Set the image path TODO: Adjust for Multi-System
            var imagePath = Path.Combine(hsPath, Root.Media, _selectedService.CurrentSystem, mediaTypePath, romName + ".png");

            if (selectedOptions[1] == "Videos")
            {
                SetVideo(imagePath);
                return;
            }

            if (selectedOptions[1].Contains("Sound") || selectedOptions[1] == "MusicBg")
            {
                if (selectedOptions[1] == "Wheel Sounds")
                {

                    return;
                }
                else
                {
                    SetSound(imagePath);
                    return;
                }
            }

            if (!File.Exists(imagePath))
            {
                WheelSource = _selectedService.SystemImage;
                IsImageSource = true;
            }
            else
            {
                IsImageSource = true;
                currentImagePath = imagePath;
                _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(imagePath));
                WheelSource = _selectedService.GameImage;

                MediaPaneHeader += " | " + Path.GetFileName(imagePath) + " W:" + Math.Round(WheelSource.Width) + " H:" + Math.Round(WheelSource.Height);
            }

        }
        private void SetImageWheelPreview(string imagePath)
        {
            WheelSource = null;
            VideoSource = null;
            IsVideoSource = false;

            if (File.Exists(imagePath))
            {
                IsImageSource = true;

                var fullpath = Path.GetFullPath(imagePath);

                currentImagePath = fullpath;

                _selectedService.GameImage =
                    SelectedService.SetBitmapFromUri(new Uri(currentImagePath));
                WheelSource = _selectedService.GameImage;

                MediaPaneHeader = "Media View | " + currentImagePath + " W:" + Math.Round(WheelSource.Width) + " H:" + Math.Round(WheelSource.Height);
            }
        }
        private void SetSound(string soundPath)
        {
            var soundsPath = soundPath.Replace(".png", ".mp3");

            if (File.Exists(soundsPath))
            {
                SetVideoSource(soundsPath);
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
                    if (File.Exists(pathToImage))
                    {
                        IsImageSource = true;
                        currentImagePath = pathToImage;
                        MediaPaneHeader += " | " + Path.GetFileName(pathToImage);
                        _selectedService.GameImage = SelectedService.SetBitmapFromUri(new Uri(pathToImage));
                        WheelSource = _selectedService.GameImage;
                        IsVideoSource = false;
                    }
                }
                else
                {
                    SetVideoSource(videoPath);
                }

            }
            else
            {
                SetVideoSource(videoPath);
            }

        }
        private void SetVideoSource(string videoPath)
        {
            try
            {
                VideoSource = new Uri(videoPath);
                MediaPaneHeader = "Media View | " + Path.GetFileName(videoPath);
                IsVideoSource = true;
            }
            catch (Exception ex)
            {
                MediaPaneHeader = "Media View | " + ex.Message;
            }
        }
        private void SetImage(string obj)
        {
            WheelSource = _selectedService.SystemImage;
        }
        private void SetText(string file)
        {
            using (var sr = new StreamReader(file))
            {
                TextSource = sr.ReadToEnd();

                MediaPaneHeader = "Media View | " + file;

            }
        }
        private void SetPdfPageCount(string file)
        {
            if (!File.Exists(file)) return;

            PdfPageCount = _pdfService.GetNumberOfPdfPages(file);
        }
        private void SetPdfImage(string file, int pageNumber = 0)
        {
            if (!File.Exists(file)) return;

            currentPdfFile = file;

            var gsPath = _settingsRepo.HypermintSettings.GhostscriptPath;

            if (!Directory.Exists(gsPath)) return;

            WheelSource = _pdfService.GetPage(gsPath, file, pageNumber);

            MediaPaneHeader = "Media View | " + file;

        }
        private void SetMediaFromFileType(string file)
        {
            IsTextSource = false;
            IsPdf = false;
            IsSwfSource = false;
            IsVideoSource = false;
            IsImageSource = false;
            WheelSource = null;
            VideoSource = null;
            SwfFileSource = null;

            MediaPaneHeader = "Media View | ";

            if (file == "") return;

            var extension = Path.GetExtension(file);

            switch (extension.ToLower())
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                    SetImageWheelPreview(file);
                    break;
                case ".avi":
                case ".flv":
                case ".mp4":
                case ".mpg":
                    SetVideo(file);
                    break;
                case ".mp3":
                case ".wav":
                    SetSound(file);
                    break;
                case ".txt":
                case ".ini":
                    IsTextSource = true;
                    SetText(file);
                    break;
                case ".pdf":
                    IsPdf = true;
                    CurrentPage = 1;
                    SetPdfPageCount(file);
                    SetPdfImage(file);
                    break;
                case ".swf":
                    IsSwfSource = true;
                    SwfFileSource = file;
                    break;
                default:
                    MediaPaneHeader = "Media View | " + file;
                    break;
            }

        }

        #endregion

    }
}
