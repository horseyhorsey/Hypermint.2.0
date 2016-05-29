using Hs.Hypermint.HyperspinFile.Models;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.HyperspinFile.ViewModels
{
    public class HyperspinFilesViewModel : ViewModelBase
    {
        
        private IEventAggregator _eventAggregator;  

        private string columnHeader = "";
        private string groupBoxHeader = "Unused Files: ";
        public string GroupBoxHeader
        {
            get { return groupBoxHeader; }
            set { SetProperty(ref groupBoxHeader, value); }
        }

        private ICollectionView unusedMediaFiles;
        public ICollectionView UnusedMediaFiles
        {
            get { return unusedMediaFiles; }
            set { SetProperty(ref unusedMediaFiles, value); }
        }

        private ICollectionView filesForGame;
        public ICollectionView FilesForGame
        {
            get { return filesForGame; }
            set { SetProperty(ref filesForGame, value); }
        }

        private List<UnusedMediaFile> UnusedWheels;
        private List<UnusedMediaFile> UnusedArtwork1;
        private List<UnusedMediaFile> UnusedArtwork2;
        private List<UnusedMediaFile> UnusedArtwork3;
        private List<UnusedMediaFile> UnusedArtwork4;
        private List<UnusedMediaFile> UnusedThemes;
        private List<UnusedMediaFile> UnusedVideos;

        private string mediaTypeName = "Controls";
        private ISettingsRepo _settingsRepo;
        private IAuditer _auditRepo;
        private ISelectedService _selectedService;

        public string MediaTypeName
        {
            get { return mediaTypeName; }
            set { SetProperty(ref mediaTypeName, value); }
        }

        public HyperspinFilesViewModel(IEventAggregator ea, ISettingsRepo settings, IAuditer auditRepo, 
            ISelectedService selectedService)
        {
            _eventAggregator = ea;
            _settingsRepo = settings;
            _auditRepo = auditRepo;
            _selectedService = selectedService;  

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe((x) =>
            {
                SetCurrentName(x);                

                if (FilesForGame != null)
                    FilesForGame.CurrentChanged += FilesForGame_CurrentChanged;

            });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe((x) =>
            {
                UnusedMediaFiles = null;
                FilesForGame = null;
            });

            _eventAggregator.GetEvent<AuditHyperSpinEndEvent>().Subscribe(BuildUnusedMediaList);
        }

        private void BuildUnusedMediaList(string obj)
        {
            UnusedWheels = new List<UnusedMediaFile>();
            BuildList(ref UnusedWheels, Images.Wheels);

            UnusedArtwork1 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork1, Images.Artwork1);

            UnusedArtwork2 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork2, Images.Artwork2);

            UnusedArtwork3 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork3, Images.Artwork3);

            UnusedArtwork4 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork4, Images.Artwork4);

            UnusedThemes = new List<UnusedMediaFile>();
            BuildList(ref UnusedThemes, Root.Themes);

            UnusedVideos = new List<UnusedMediaFile>();
            BuildList(ref UnusedVideos, Root.Video);

        }

        private void BuildList(ref List<UnusedMediaFile> mediaList, string mediaPath)
        {
            var pathToScan = Path.Combine(
                _settingsRepo.HypermintSettings.HsPath,
                Root.Media, _selectedService.CurrentSystem,
                mediaPath);

            var files = Directory.GetFiles(pathToScan);

            foreach (string file in files)
            {
                var fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                var fileNameExt = Path.GetExtension(file);

                try
                {
                    if (fileNameNoExt.ToLower() != "thumbs" && !file.ToLower().Contains("default.zip"))
                    {
                        var fileMatchedToGame = _auditRepo.AuditsGameList.Where(x => x.RomName == fileNameNoExt).Any();

                        if (!fileMatchedToGame)
                            mediaList.Add(new UnusedMediaFile()
                            {
                                FileName = fileNameNoExt,
                                Extension = fileNameExt
                            });
                    }
                }
                catch (Exception e)
                {

                }

            }
        }

        private void SetCurrentName(string[] romAndColumn)
        {
            FilesForGame = null;

            //if (_selectedService.IsMainMenu())
            var rom = romAndColumn[0];
            //var rom = _selectedService.CurrentRomname;

            if (romAndColumn[1] != columnHeader)
            {
                columnHeader = romAndColumn[1];

                #region unusedswitch
                switch (columnHeader)
                {
                    case "Wheel":
                        UnusedMediaFiles = new ListCollectionView(UnusedWheels);
                        break;
                    case "Artwork1":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork1);
                        break;
                    case "Artwork2":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork2);
                        break;
                    case "Artwork3":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork3);
                        break;
                    case "Artwork4":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork4);
                        break;
                    case "Theme":
                        UnusedMediaFiles = new ListCollectionView(UnusedThemes);
                        break;
                    case "Videos":
                        UnusedMediaFiles = new ListCollectionView(UnusedVideos);
                        break;
                    default:
                        UnusedMediaFiles = null;
                        break;
                }
                #endregion

                GroupBoxHeader = "Hyperspin files for: " + columnHeader;
            }

            switch (columnHeader)
            {
                case "Wheel":
                    GetHyperspinFilesForGame(Images.Wheels, rom + "*.*");
                    break;
                case "Artwork1":
                    GetHyperspinFilesForGame(Images.Artwork1, rom + "*.*");
                    break;
                case "Artwork2":
                    GetHyperspinFilesForGame(Images.Artwork2, rom + "*.*");
                    break;
                case "Artwork3":
                    GetHyperspinFilesForGame(Images.Artwork3, rom + "*.*");
                    break;
                case "Artwork4":
                    GetHyperspinFilesForGame(Images.Artwork4, rom + "*.*");
                    break;
                case "Theme":
                    GetHyperspinFilesForGame(Root.Themes, rom + "*.*");
                    break;
                case "Videos":
                    GetHyperspinFilesForGame(Root.Video, rom + "*.*");
                    break;
                case "Letters":
                    GetHyperspinFilesForMenu(Images.Letters);
                    break;
                case "GenreBg":
                    GetHyperspinFilesForMenu(Images.GenreBackgrounds);
                    break;
                case "GenreWheel":
                    GetHyperspinFilesForMenu(Images.GenreWheel);
                    break;
                case "Wheel Sounds":
                    GetHyperspinFilesForMenu(Sound.WheelSounds);
                    break;
                case "Pointer":
                    GetHyperspinFilesForMenu(Images.Pointer);
                    break;
                case "Special":
                    GetHyperspinFilesForMenu(Images.Special);
                    break;
                default:
                    break;
            }
        }

        private void GetHyperspinLetters(string letters)
        {
            string pathToScan = "";

            if (_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
                Root.Media, _selectedService.CurrentRomname,
                letters);
            else
                pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
                Root.Media, _selectedService.CurrentSystem,
                letters);

            var mediaFiles = new List<MediaFile>();

            foreach (var item in Directory.EnumerateFiles(pathToScan, "*.*"))
            {
                mediaFiles.Add(new MediaFile
                {
                    FileName = item
                });
            }

            FilesForGame = new ListCollectionView(mediaFiles);

        }

        private void GetHyperspinFilesForGame(string mediaPath, string filter = "*.*")
        {
            var mainMenu = _selectedService.CurrentSystem.ToLower().Contains("main menu");
            string selected = "";

            selected = _selectedService.CurrentSystem;

            var pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
            Root.Media, selected, mediaPath);

            var mediaFiles = new List<MediaFile>();

            foreach (var item in Directory.EnumerateFiles(pathToScan, filter))
            {
                mediaFiles.Add(new MediaFile
                {
                    FileName = item
                }); 
            } 

            FilesForGame = new ListCollectionView(mediaFiles);

        }

        private void GetHyperspinFilesForMenu(string mediaPath, string filter = "*.*")
        {
            var selected = _selectedService.CurrentRomname;

            var pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
            Root.Media, selected, mediaPath);

            var mediaFiles = new List<MediaFile>();

            foreach (var item in Directory.EnumerateFiles(pathToScan, filter))
            {
                mediaFiles.Add(new MediaFile
                {
                    FileName = item
                });
            }

            FilesForGame = new ListCollectionView(mediaFiles);

        }

        private void FilesForGame_CurrentChanged(object sender, EventArgs e)
        {
            var selectedFile = FilesForGame.CurrentItem as MediaFile;            

            if (selectedFile != null)
                _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(selectedFile.FileName);
        }
    }
}
