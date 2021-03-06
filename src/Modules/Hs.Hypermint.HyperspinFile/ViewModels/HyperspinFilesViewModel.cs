﻿using GongSolutions.Wpf.DragDrop;
using Hs.Hypermint.HyperspinFile.Models;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Extensions;
using Hypermint.Base.Helpers;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Ionic.Zip;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Hs.Hypermint.HyperspinFile.ViewModels
{
    public class HyperspinFilesViewModel : ViewModelBase, IDropTarget
    {

        #region Fields
        private ISettingsHypermint _settingsRepo;
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
        private IImageEditService _imageEdit;
        private IRegionManager _regionManager;
        private ITrashMaster _trashMaster;
        private IHyperspinManager _hyperspinManager;
        #endregion

        #region Constructors
        public HyperspinFilesViewModel(IEventAggregator ea, ISettingsHypermint settings,
             IRegionManager regionManager, ITrashMaster trashMaster,
            ISelectedService selectedService, IImageEditService imageEdit, IHyperspinManager hyperspinManager)
        {
            _eventAggregator = ea;
            _settingsRepo = settings;

            _selectedService = selectedService;
            _imageEdit = imageEdit;
            _regionManager = regionManager;
            _trashMaster = trashMaster;
            _hyperspinManager = hyperspinManager;


            CurrentMediaFiles = new ObservableCollection<MediaFile>();
            FilesForGame = new ListCollectionView(CurrentMediaFiles);
            FilesForGame.CurrentChanged += FilesForGame_CurrentChanged;

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe((x) =>
            {
                try
                {
                    string activeViewName = RegionHelper.GetCurrentViewName(_regionManager);

                    //if (!activeViewName.Contains("HsMediaAuditView") || !activeViewName.Contains("DatabaseDetailsView"))
                    SetCurrentName(x);

                    if (FilesForGame != null)
                    {
                        if (FilesForGame.CurrentItem == null)
                            _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                        else
                            FilesForGame_CurrentChanged(null, null);
                    }
                }
                catch (Exception ex)
                {

                }

            }, ThreadOption.UIThread);

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe((x) =>
            {
                UnusedMediaFiles = null;
                CurrentMediaFiles.Clear();
            });

            //_eventAggregator.GetEvent<AuditHyperSpinEndEvent>().Subscribe(BuildUnusedMediaList);

            RemoveFileCommand = new DelegateCommand(() =>
            {
                var currentFile = FilesForGame.CurrentItem as MediaFile;

                if (currentFile != null)
                {
                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");

                    _trashMaster.HsFileToTrash(currentFile.FileName, GetSystemFolderName(columnHeader), columnHeader, _selectedService.CurrentRomname);

                    try
                    {
                        SetCurrentName(new string[] { _selectedService.CurrentRomname, columnHeader });

                        _eventAggregator.GetEvent<RefreshHsAuditEvent>().Publish("");
                    }

                    catch (Exception) { }

                }
            });

            OpenFolderCommand = new DelegateCommand(OpenFolder);

            OpenTrashFolderCommand = new DelegateCommand(() =>
            {
                var sys = GetSystemFolderName(columnHeader);
                var result = new DirectoryInfo(_trashMaster.GetHsTrashPath(sys, columnHeader)).Open();
            });
        }

        #endregion

        #region Properties
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
        public string MediaTypeName
        {
            get { return mediaTypeName; }
            set { SetProperty(ref mediaTypeName, value); }
        }

        #endregion

        #region Commands
        public DelegateCommand RemoveFileCommand { get; private set; }
        public DelegateCommand OpenFolderCommand { get; private set; }
        public DelegateCommand OpenTrashFolderCommand { get; private set; }
        public ObservableCollection<MediaFile> CurrentMediaFiles { get; private set; }
        #endregion

        #region Public Methods
        public void DroppedFile(string[] filelist, string selectedColumn)
        {
            int i;
            string Filename, ext, filename3;

            for (i = 0; i < filelist.Length; i++)
            {
                Filename = System.IO.Path.GetFileName(filelist[i]);
                ext = System.IO.Path.GetExtension(filelist[i]);
                filename3 = System.IO.Path.GetFileNameWithoutExtension(filelist[i]);

                switch (selectedColumn)
                {
                    case "Wheel":
                    case "Artwork1":
                    case "Artwork2":
                    case "Artwork3":
                    case "Artwork4":
                    case "Backgrounds":
                    case "Videos":
                        wheel_drop(filelist[i], selectedColumn, _selectedService.CurrentRomname);
                        break;
                    case "Theme":
                        ThemeDrop(filelist[i], selectedColumn, _selectedService.CurrentRomname);
                        break;
                    case "Letters":
                    case "GenreBg":
                    case "GenreWheel":
                    case "Special":
                    case "Wheel Sounds":
                    case "Sound Start":
                    case "Sound End":
                        wheel_drop(filelist[i], selectedColumn, Path.GetFileNameWithoutExtension(filelist[i]));
                        break;
                    case "Pointer":
                        wheel_drop(filelist[i], selectedColumn, "Pointer");
                        break;
                    case "Sound Click":
                        wheel_drop(filelist[i], selectedColumn, "Wheel Click");
                        break;
                    default:
                        break;
                }
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {

            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null;
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".*");
            }) ? DragDropEffects.Copy : DragDropEffects.None;

            DroppedFile(dragFileList.ToArray(), columnHeader);

            SetCurrentName(new string[] { _selectedService.CurrentRomname, columnHeader });

            if (FilesForGame != null)
            {
                if (FilesForGame.CurrentItem == null)
                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                else
                    FilesForGame_CurrentChanged(null, null);
            }

        }
        #endregion        

        #region Support Methods
        [Obsolete("Use helpers to get unused files")]
        private void BuildUnusedMediaList(string obj)
        {
            //UnusedWheels = new List<UnusedMediaFile>();
            //BuildList(ref UnusedWheels, Images.Wheels);

            //UnusedArtwork1 = new List<UnusedMediaFile>();
            //BuildList(ref UnusedArtwork1, Images.Artwork1);

            //UnusedArtwork2 = new List<UnusedMediaFile>();
            //BuildList(ref UnusedArtwork2, Images.Artwork2);

            //UnusedArtwork3 = new List<UnusedMediaFile>();
            //BuildList(ref UnusedArtwork3, Images.Artwork3);

            //UnusedArtwork4 = new List<UnusedMediaFile>();
            //BuildList(ref UnusedArtwork4, Images.Artwork4);

            //UnusedThemes = new List<UnusedMediaFile>();
            //BuildList(ref UnusedThemes, Root.Themes);

            //UnusedVideos = new List<UnusedMediaFile>();
            //BuildList(ref UnusedVideos, Root.Video);

        }

        [Obsolete("Use helpers to get unused files")]
        private void BuildList(ref List<UnusedMediaFile> mediaList, string mediaPath)
        {
            //var pathToScan = Path.Combine(
            //    _settingsRepo.HypermintSettings.HsPath,
            //    Root.Media, _selectedService.CurrentSystem,
            //    mediaPath);

            //var files = Directory.GetFiles(pathToScan);

            //foreach (string file in files)
            //{
            //    var fileNameNoExt = Path.GetFileNameWithoutExtension(file);
            //    var fileNameExt = Path.GetExtension(file);

            //    try
            //    {
            //        if (fileNameNoExt.ToLower() != "thumbs" && !file.ToLower().Contains("default.zip"))
            //        {
            //            var fileMatchedToGame = _auditRepo.AuditsGameList.Where(x => x.RomName == fileNameNoExt).Any();

            //            if (!fileMatchedToGame)
            //                mediaList.Add(new UnusedMediaFile()
            //                {
            //                    FileName = fileNameNoExt,
            //                    Extension = fileNameExt
            //                });
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }

            //}
        }

        private void SetCurrentName(string[] romAndColumn)
        {
            string rom = "";

            if (romAndColumn[0] == "")
                rom = _selectedService.CurrentRomname;
            else
                rom = romAndColumn[0];

            if (romAndColumn[1] != columnHeader)
            {
                columnHeader = romAndColumn[1];
            }

            switch (romAndColumn[1])
            {
                case "Wheel":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveWheel =
                        GetHyperspinFilesForGame(Images.Wheels, rom + "*.*");
                    break;
                case "Artwork1":
                    GetHyperspinFilesForGame(Images.Artwork1, rom + "*.*");
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveArt1 =
                        GetHyperspinFilesForGame(Images.Artwork1, rom + "*.*");
                    break;
                case "Artwork2":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveArt2 =
                        GetHyperspinFilesForGame(Images.Artwork2, rom + "*.*");
                    break;
                case "Artwork3":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveArt3 =
                        GetHyperspinFilesForGame(Images.Artwork3, rom + "*.*");
                    break;
                case "Artwork4":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveArt4 =
                        GetHyperspinFilesForGame(Images.Artwork4, rom + "*.*");
                    break;
                case "Theme":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveTheme =
                        GetHyperspinFilesForGame(Root.Themes, rom + "*.*");
                    break;
                case "Videos":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveVideo =
                        GetHyperspinFilesForGame(Root.Video, rom + "*.*");
                    break;
                case "Backgrounds":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveBackground =
                        GetHyperspinFilesForGame(Images.Backgrounds, rom + "*.*");
                    break;
                case "MusicBg":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveBGMusic =
                        GetHyperspinFilesForGame(Sound.BackgroundMusic, rom + "*.*");
                    break;
                case "Wheel Sounds":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveS_Wheel =
                    GetHyperspinFilesForMenu(Sound.WheelSounds, rom);
                    break;
                case "Letters":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveLetters =
                        GetHyperspinFilesForMenu(Images.Letters, rom);
                    break;
                case "Special":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveSpecial =
                    GetHyperspinFilesForMenu(Images.Special, rom);
                    break;
                case "GenreBg":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveGenreBG =
                    GetHyperspinFilesForMenu(Images.GenreBackgrounds, rom);
                    break;
                case "GenreWheel":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveGenreWheel =
                    GetHyperspinFilesForMenu(Images.GenreWheel, rom);
                    break;
                case "Pointer":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HavePointer =
                    GetHyperspinFilesForMenu(Images.Pointer, rom);
                    break;
                case "Sound Start":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveS_Start =
                    GetHyperspinFilesForMenu(Sound.SystemStart, rom);
                    break;
                case "Sound End":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveS_Exit =
                    GetHyperspinFilesForMenu(Sound.SystemExit, rom);
                    break;
                case "Sound Click":
                    _hyperspinManager.CurrentSystemsGames.Where(x => x.RomName == rom).Single().HaveWheelClick =
                    GetHyperspinFilesForMenu("Sound", rom, "Wheel Click*.*");
                    break;
                default:
                    break;
            }

            GroupBoxHeader = "Hyperspin files for: " + columnHeader;
        }

        private bool GetHyperspinFilesForGame(string mediaPath, string filter = "*.*")
        {
            var mainMenu = _selectedService.CurrentSystem.ToLower().Contains("main menu");
            string selected = "";

            selected = _selectedService.CurrentSystem;

            var pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
            Root.Media, selected, mediaPath);

            CurrentMediaFiles.Clear();

            if (Directory.Exists(pathToScan))
            {
                foreach (var item in Directory.EnumerateFiles(pathToScan, filter))
                {
                    CurrentMediaFiles.Add(new MediaFile
                    {
                        Name = Path.GetFileNameWithoutExtension(item),
                        FileName = Path.GetFullPath(item),
                        Extension = Path.GetExtension(item)
                    });
                }
            }            

            if (CurrentMediaFiles.Count == 0)
                return false;
            else
            {
                FilesForGame.MoveCurrentToFirst();
                return true;
            }                

        }

        private bool GetHyperspinFilesForMenu(string mediaPath, string romName, string filter = "*.*")
        {

            var pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath, Root.Media, romName, mediaPath);

            if (!Directory.Exists(pathToScan)) Directory.CreateDirectory(pathToScan);

            CurrentMediaFiles.Clear();

            foreach (var item in Directory.EnumerateFiles(pathToScan, filter))
            {
                CurrentMediaFiles.Add(new MediaFile
                {
                    Name = Path.GetFileNameWithoutExtension(item),
                    FileName = Path.GetFullPath(item),
                    Extension = Path.GetExtension(item)
                });
            }

            if (CurrentMediaFiles.Count == 0)
                return false;
            else
            {
                FilesForGame.MoveCurrentToFirst();
                return true;
            }

        }

        private void FilesForGame_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedFile = FilesForGame.CurrentItem as MediaFile;

                if (selectedFile != null)
                {
                    GroupBoxHeader = "Hyperspin " + columnHeader
                        + " : " + selectedFile.Name + selectedFile.Extension;
                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(selectedFile.FileName);
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message + ex.StackTrace);
            }

        }

        private string GetHsMediaPathDirectory(
            string hsPath, string systemName, string mediaType)
        {
            var hsSystemMediaPath = Path.Combine(hsPath, Root.Media);

            switch (mediaType)
            {
                case "Wheel":
                    if (_selectedService.IsMainMenu())
                        systemName = "Main Menu";
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Wheels);
                case "Artwork1":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Artwork1);
                case "Artwork2":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Artwork2);
                case "Artwork3":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Artwork3);
                case "Artwork4":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Artwork4);
                case "Backgrounds":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Backgrounds);
                case "Videos":
                    if (_selectedService.IsMainMenu())
                        systemName = "Main Menu";
                    return Path.Combine(hsSystemMediaPath, systemName, Root.Video);
                case "Theme":
                    if (_selectedService.IsMainMenu())
                        systemName = "Main Menu";
                    return Path.Combine(hsSystemMediaPath, systemName, Root.Themes);
                case "Letters":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Letters);
                case "Special":
                    if (_selectedService.IsMainMenu() && _selectedService.CurrentRomname == "Main Menu")
                        systemName = "Main Menu";
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Special);
                case "Sound Click":
                    return Path.Combine(hsSystemMediaPath, systemName, Root.Sound);
                case "Wheel Sounds":
                    return Path.Combine(hsSystemMediaPath, systemName, Sound.WheelSounds);
                case "MusicBg":
                    return Path.Combine(hsSystemMediaPath, systemName, Sound.BackgroundMusic);
                case "Sound End":
                    return Path.Combine(hsSystemMediaPath, systemName, Sound.SystemExit);
                case "Sound Start":
                    return Path.Combine(hsSystemMediaPath, systemName, Sound.SystemStart);
                case "GenreBg":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.GenreBackgrounds);
                case "GenreWheel":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.GenreWheel);
                case "Pointer":
                    return Path.Combine(hsSystemMediaPath, systemName, Images.Pointer);
                default:
                    return "";
            }
        }

        private void FileDrop(string file, string selectedColumn, string romName)
        {
            string ext = Path.GetExtension(file);
            string path = Path.GetDirectoryName(file);

            string name = _selectedService.CurrentSystem;

            if (_selectedService.IsMainMenu())
                name = _selectedService.CurrentRomname;

            string hsMediaPath = GetHsMediaPathDirectory(
                _settingsRepo.HypermintSettings.HsPath,
                 name,
                 columnHeader);

            if (string.IsNullOrWhiteSpace(hsMediaPath)) return;

            if (!Directory.Exists(hsMediaPath))
            {
                Directory.CreateDirectory(hsMediaPath);
            }


            name = Path.GetFileNameWithoutExtension(file);
            string fullPath = Path.Combine(hsMediaPath, name + ext);

            int i = 0;
            while (File.Exists(fullPath))
            {
                fullPath = Path.Combine(hsMediaPath, name + " " + i + ext);
                i++;
            }

            File.Copy(file, fullPath);
        }

        private void wheel_drop(string file, string selectedColumn, string romName)
        {
            string ext = Path.GetExtension(file);
            string path = Path.GetDirectoryName(file);

            string name = _selectedService.CurrentSystem;

            if (_selectedService.IsMainMenu())
                name = _selectedService.CurrentRomname;

            string hsMediaPath = GetHsMediaPathDirectory(
                _settingsRepo.HypermintSettings.HsPath,
                 name,
                 columnHeader);

            if (string.IsNullOrWhiteSpace(hsMediaPath)) return;

            string fullPath = "";

            try
            {
                if (ext == ".bmp" || ext == ".gif")
                    fullPath = Path.Combine(hsMediaPath, romName + ".png");
                else
                    fullPath = Path.Combine(hsMediaPath, romName + ext);

                if (ext == ".bmp" || ext == ".gif")
                    path = fullPath + romName + ext;

                if (file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".gif")
                    || file.ToLower().EndsWith(".jpeg") || file.ToLower().EndsWith(".bmp"))
                {
                    int i = 0;
                    while (File.Exists(fullPath))
                    {
                        fullPath = Path.Combine(hsMediaPath, romName + " " + i + ".png");
                        i++;
                    }

                    ConvertImageToPng(file, fullPath);

                    _eventAggregator.GetEvent<HsAuditUpdateEvent>().Publish(new string[] {
                        romName, selectedColumn
                    });

                    return;
                }
                else if (file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".mp4"))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    int i = 0;
                    while (File.Exists(fullPath))
                    {
                        fullPath = Path.Combine(hsMediaPath, romName + " " + i + ext);
                        i++;
                    }

                    File.Copy(file, fullPath);
                }

            }
            catch (Exception)
            {

            }

        }

        private void ThemeDrop(string file, string selectedColumn, string romName)
        {
            string ext = Path.GetExtension(file);
            string path = Path.GetDirectoryName(file);
            string name = _selectedService.CurrentSystem;

            if (_selectedService.IsMainMenu())
                name = _selectedService.CurrentRomname;

            string hsMediaPath = GetHsMediaPathDirectory(
                _settingsRepo.HypermintSettings.HsPath,
                 name,
                 columnHeader);

            string fullPath = "";

            // if extension is a zip just rename & copy over for now
            if (ext == ".zip")
            {
                fullPath = Path.Combine(hsMediaPath, romName + ext);

                if (!Directory.Exists(hsMediaPath))
                    Directory.CreateDirectory(hsMediaPath);

                int i = 0;
                while (File.Exists(fullPath))
                {
                    fullPath = Path.Combine(hsMediaPath, romName + " " + i + ".zip");
                    i++;
                }

                File.Copy(file, fullPath);

                return;
            }
            else if (ext == ".png" || ext == ".jpg")
            {
                fullPath = Path.Combine(hsMediaPath, romName + ".zip");

                if (!Directory.Exists(hsMediaPath))
                    Directory.CreateDirectory(hsMediaPath);

                int i = 0;
                while (File.Exists(fullPath))
                {
                    fullPath = Path.Combine(hsMediaPath, romName + " " + i + ".zip");
                    i++;
                }


                QuickThemeDrop(file, fullPath);
            }

        }

        private void QuickThemeDrop(string imageFile, string newThemeFile)
        {
            //// Quick theme file creator
            //// 
            //// If image format is dragged into theme folder then create a basic theme with image
            //// renamed to Background.png, info.txt & Theme.xml
            //// romname + ".zip"
            using (System.Drawing.Image im = System.Drawing.Image.FromFile(imageFile))
            {
                System.Drawing.Image im2 = System.Drawing.Image.FromFile(imageFile);

                im2 = _imageEdit.ResizeImageEdit(im, new System.Drawing.Size(1024, 768));

                im2.Save("Background.png", ImageFormat.Png);
            }

            try
            {
                if (!File.Exists("Theme.xml"))
                    return;

                using (var zip = new ZipFile())
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
                    zip.AddFile("Background.png");
                    zip.AddFile("Theme.xml");
                    zip.AddFile("Info.txt");

                    zip.Save(newThemeFile);

                    File.Delete("Background.png");
                }
            }
            catch (Exception)
            {

            }
        }

        private static void ConvertImageToPng(string file, string path)
        {
            using (System.Drawing.Image im = System.Drawing.Image.FromFile(file))
            {
                im.Save(Path.Combine(path), ImageFormat.Png);
            }
        }

        private void OpenFolder()
        {
            string name = _selectedService.CurrentSystem;

            if (_selectedService.IsMainMenu())
                name = _selectedService.CurrentRomname;

            string hsMediaPath = GetHsMediaPathDirectory(
                _settingsRepo.HypermintSettings.HsPath,
                 name,
                 columnHeader);

            var result = new DirectoryInfo(hsMediaPath).Open();
        }

        private string GetSystemFolderName(string mediaType)
        {
            string sysFolderName = _selectedService.CurrentSystem;

            if (_selectedService.IsMainMenu())
            {
                if (mediaType != "Wheel" && mediaType != "Theme" && mediaType != "Videos")
                    sysFolderName = _selectedService.CurrentRomname;
            }

            return sysFolderName;
        }

        #endregion

    }
}


