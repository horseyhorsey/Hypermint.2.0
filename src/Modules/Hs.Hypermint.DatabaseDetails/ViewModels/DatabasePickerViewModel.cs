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
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabasePickerViewModel : ViewModelBase
    {
        public DatabasePickerViewModel(ISettingsRepo settingsRepo,
     IFolderExplore folderService, ISelectedService selectedService,
     IFileFolderChecker fileFolderCheck, IGenreRepo genreRepo,
     IEventAggregator eventAggregator)
        {
            _settingsRepo = settingsRepo;
            _folderExploreService = folderService;
            _selectedService = selectedService;
            _fileFolderCheck = fileFolderCheck;
            _genreRepo = genreRepo;
            _eventAggregator = eventAggregator;            

            if (_genreRepo.GenreList == null)
            {
                _genreRepo.GenreList = new List<string>();
                GenreDatabases = new ListCollectionView(_genreRepo.GenreList);
            }

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(GetSystemDatabases);

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(PopulateGenres);

            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);

            //SystemDatabases.CurrentChanged += SystemDatabases_CurrentChanged;
        }

        #region Services
        private ISettingsRepo _settingsRepo;
        private IFolderExplore _folderExploreService;
        private ISelectedService _selectedService;
        private IFileFolderChecker _fileFolderCheck;
        private IGenreRepo _genreRepo;
        private IEventAggregator _eventAggregator;
        #endregion

        #region Collections
        private ICollectionView systemDatabases;
        public ICollectionView SystemDatabases
        {
            get { return systemDatabases; }
            set { SetProperty(ref systemDatabases, value); }
        }

        private ICollectionView genreDatabases;        
        public ICollectionView GenreDatabases
        {
            get { return genreDatabases; }
            set { SetProperty(ref genreDatabases, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand<string> OpenFolderCommand { get; set; }
        #endregion

        #region Methods
        private void OpenFolder(string hyperspinDirType)
        {
            switch (hyperspinDirType)
            {
                case "Databases":
                    var pathToOpen = _settingsRepo.HypermintSettings.HsPath;
                    _folderExploreService.OpenFolder(pathToOpen + "\\" +
                        Root.Databases + "\\" +
                        _selectedService.CurrentSystem);
                    break;
                default:
                    break;
            }

        }

        private void GetSystemDatabases(string systemName)
        {
            var xmlsInDirectory = new List<string>();            

            //Dont set if we're in the main menu
            if (systemName.Contains("Main Menu"))
                return;

            //Set path and return if not found.
            var pathToScan = _settingsRepo.HypermintSettings.HsPath +
                "\\" +
                Root.Databases + "\\" +
                _selectedService.CurrentSystem;
            if (!_fileFolderCheck.DirectoryExists(pathToScan)) return;            

            foreach (var xmlFile in _fileFolderCheck.GetFiles(pathToScan, "*.xml"))
            {
                var dbFileName = _fileFolderCheck.GetFileNameNoExt(xmlFile);

                if (dbFileName.Contains(systemName))
                {
                    try
                    {
                        if (dbFileName.ToLower() != "genre")
                        {
                            xmlsInDirectory.Add(dbFileName);
                        }
                    }
                    catch (Exception) { }
                }
            }

            SystemDatabases = new ListCollectionView(xmlsInDirectory);

            SystemDatabases.MoveCurrentTo(systemName);      
        }

        private void PopulateGenres(string systemName)
        {
            SystemDatabases.CurrentChanged -= SystemDatabases_CurrentChanged;

            try
            {
                var hsPath = _settingsRepo.HypermintSettings.HsPath;
                var system = _selectedService.CurrentSystem;
                
                var genrePath = _fileFolderCheck.CombinePath(
                    new string[] { hsPath, Root.Databases, system, "Genre.xml" });

                if (_fileFolderCheck.FileExists(genrePath))
                    _genreRepo.PopulateGenres(genrePath);
                else { _genreRepo.GenreList.Clear(); }

                GenreDatabases = new ListCollectionView(_genreRepo.GenreList);
            }
            catch (Exception) { }

            SystemDatabases.CurrentChanged += SystemDatabases_CurrentChanged;

        }

        #endregion

        #region Events
        private void SystemDatabases_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                var systemDatabaseName = (string)SystemDatabases.CurrentItem;                

                _eventAggregator.GetEvent<SystemDatabaseChanged>().Publish(systemDatabaseName);
            }
            catch (Exception) { }


        }
        #endregion

    }
}
