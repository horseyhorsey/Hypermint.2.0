using GongSolutions.Wpf.DragDrop;
using Hs.Hypermint.HyperspinFile.Models;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Helpers;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Ionic.Zip;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
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
        #region Constructors
        public HyperspinFilesViewModel(IEventAggregator ea, ISettingsRepo settings,
             IRegionManager regionManager,
            IFolderExplore folderExplore, ITrashMaster trashMaster,
            ISelectedService selectedService, IImageEditService imageEdit)
        {
            _eventAggregator = ea;
            _settingsRepo = settings;

#warning 69 need to reenable
            //IAuditer auditRepo,
            //_auditRepo = auditRepo;

            _selectedService = selectedService;
            _imageEdit = imageEdit;
            _regionManager = regionManager;
            _folderExplore = folderExplore;
            _trashMaster = trashMaster;

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe((x) =>
            {
                string activeViewName = RegionHelper.GetCurrentViewName(_regionManager);

                if (!activeViewName.Contains("HsMediaAuditView") || !activeViewName.Contains("DatabaseDetailsView"))

                    SetCurrentName(x);

                if (FilesForGame != null)
                {
                    if (FilesForGame.CurrentItem == null)
                        _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                    else
                        FilesForGame_CurrentChanged(null, null);
                }

            });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe((x) =>
            {
                UnusedMediaFiles = null;
                FilesForGame = null;
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

                _folderExplore.OpenFolder(_trashMaster.GetHsTrashPath(sys, columnHeader));

            });
        }

        #endregion

        #region Fields
        private ISettingsRepo _settingsRepo;
        private IAuditer _auditRepo;
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
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
        private IImageEditService _imageEdit;
        private IRegionManager _regionManager;
        private IFolderExplore _folderExplore;
        private ITrashMaster _trashMaster;

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
                    case "Letters":
                    case "GenreBg":
                    case "Pointer":
                    case "GenreWheel":
                        wheel_drop(filelist[i], selectedColumn, _selectedService.CurrentRomname);
                        break;
                    case "Theme":
                        ThemeDrop(filelist[i], selectedColumn, _selectedService.CurrentRomname);
                        break;
                    case "Special":
                    case "Wheel Sounds":
                    case "Sound Click":
                        FileDrop(filelist[i], selectedColumn, _selectedService.CurrentRomname);
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
                FilesForGame.CurrentChanged += FilesForGame_CurrentChanged;

                if (FilesForGame.CurrentItem == null)
                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                else
                    FilesForGame_CurrentChanged(null, null);
            }

        }
        #endregion        

        #region Support Methods
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
            string rom = "";

            if (romAndColumn[0] == "")
                rom = _selectedService.CurrentRomname;
            else
                rom = romAndColumn[0];

            if (romAndColumn[1] != columnHeader)
            {
                columnHeader = romAndColumn[1];

                #region unusedswitch
                //switch (columnHeader)
                //{
                //    case "Wheel":
                //        UnusedMediaFiles = new ListCollectionView(UnusedWheels);
                //        break;
                //    case "Artwork1":
                //        UnusedMediaFiles = new ListCollectionView(UnusedArtwork1);
                //        break;
                //    case "Artwork2":
                //        UnusedMediaFiles = new ListCollectionView(UnusedArtwork2);
                //        break;
                //    case "Artwork3":
                //        UnusedMediaFiles = new ListCollectionView(UnusedArtwork3);
                //        break;
                //    case "Artwork4":
                //        UnusedMediaFiles = new ListCollectionView(UnusedArtwork4);
                //        break;
                //    case "Theme":
                //        UnusedMediaFiles = new ListCollectionView(UnusedThemes);
                //        break;
                //    case "Videos":
                //        UnusedMediaFiles = new ListCollectionView(UnusedVideos);
                //        break;
                //    default:
                //        UnusedMediaFiles = null;
                //        break;
                //}
                #endregion

            }

            switch (romAndColumn[1])
            {
                case "Wheel":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveWheel =
                        GetHyperspinFilesForGame(Images.Wheels, rom + "*.*");
                    break;
                case "Artwork1":
                    GetHyperspinFilesForGame(Images.Artwork1, rom + "*.*");
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveArt1 =
                        GetHyperspinFilesForGame(Images.Artwork1, rom + "*.*");
                    break;
                case "Artwork2":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveArt2 =
                        GetHyperspinFilesForGame(Images.Artwork2, rom + "*.*");
                    break;
                case "Artwork3":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveArt3 =
                        GetHyperspinFilesForGame(Images.Artwork3, rom + "*.*");
                    break;
                case "Artwork4":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveArt4 =
                        GetHyperspinFilesForGame(Images.Artwork4, rom + "*.*");
                    break;
                case "Theme":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveTheme =
                        GetHyperspinFilesForGame(Root.Themes, rom + "*.*");
                    break;
                case "Videos":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveVideo =
                        GetHyperspinFilesForGame(Root.Video, rom + "*.*");
                    break;
                case "Backgrounds":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveBackground =
                        GetHyperspinFilesForGame(Images.Backgrounds, rom + "*.*");
                    break;
                case "MusicBg":
                    _auditRepo.AuditsGameList.Where(x => x.RomName == rom).Single().HaveBGMusic =
                        GetHyperspinFilesForGame(Sound.BackgroundMusic, rom + "*.*");
                    break;
                case "Letters":
                    _auditRepo.AuditsMenuList.Where(x => x.RomName == rom).Single().HaveLetters =
                        GetHyperspinFilesForMenu(Images.Letters);
                    break;
                case "GenreBg":
                    _auditRepo.AuditsMenuList.Where(x => x.RomName == rom).Single().HaveGenreBG =
                    GetHyperspinFilesForMenu(Images.GenreBackgrounds);
                    break;
                case "GenreWheel":
                    _auditRepo.AuditsMenuList.Where(x => x.RomName == rom).Single().HaveGenreWheel =
                    GetHyperspinFilesForMenu(Images.GenreWheel);
                    break;
                case "Wheel Sounds":
                    _auditRepo.AuditsMenuList.Where(x => x.RomName == rom).Single().HaveS_Wheel =
                    GetHyperspinFilesForMenu(Sound.WheelSounds);
                    break;
                case "Pointer":
                    _auditRepo.AuditsMenuList.Where(x => x.RomName == rom).Single().HavePointer =
                    GetHyperspinFilesForMenu(Images.Pointer);
                    break;
                case "Special":
                    _auditRepo.AuditsMenuList.Where(x => x.RomName == rom).Single().HaveSpecial =
                    GetHyperspinFilesForMenu(Images.Special);
                    break;
                default:
                    break;
            }

            GroupBoxHeader = "Hyperspin files for: " + columnHeader;
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
                    Name = Path.GetFileNameWithoutExtension(item),
                    FileName = Path.GetFullPath(item),
                    Extension = Path.GetExtension(item)
                });
            }

            FilesForGame = new ListCollectionView(mediaFiles);

            FilesForGame.CurrentChanged += FilesForGame_CurrentChanged;

        }

        private bool GetHyperspinFilesForGame(string mediaPath, string filter = "*.*")
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
                    Name = Path.GetFileNameWithoutExtension(item),
                    FileName = Path.GetFullPath(item),
                    Extension = Path.GetExtension(item)
                });
            }

            FilesForGame = new ListCollectionView(mediaFiles);

            FilesForGame.CurrentChanged += FilesForGame_CurrentChanged;

            FilesForGame.MoveCurrentToLast();

            if (mediaFiles.Count == 0)
                return false;
            else
                return true;

        }

        private bool GetHyperspinFilesForMenu(string mediaPath, string filter = "*.*")
        {
            FilesForGame = null;

            var selected = _selectedService.CurrentRomname;

            var pathToScan = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
            Root.Media, selected, mediaPath);

            if (!Directory.Exists(pathToScan)) Directory.CreateDirectory(pathToScan);

            var mediaFiles = new List<MediaFile>();

            foreach (var item in Directory.EnumerateFiles(pathToScan, filter))
            {
                mediaFiles.Add(new MediaFile
                {
                    Name = Path.GetFileNameWithoutExtension(item),
                    FileName = Path.GetFullPath(item),
                    Extension = Path.GetExtension(item)
                });
            }

            FilesForGame = new ListCollectionView(mediaFiles);

            FilesForGame.CurrentChanged += FilesForGame_CurrentChanged;

            if (mediaFiles.Count == 0)
                return false;
            else
                return true;

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

            _folderExplore.OpenFolder(hsMediaPath);
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

