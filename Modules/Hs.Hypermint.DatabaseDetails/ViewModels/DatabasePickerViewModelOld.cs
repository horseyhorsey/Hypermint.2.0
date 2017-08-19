using Frontends.Models.Hyperspin;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabasePickerViewModelOld : ViewModelBase
    {
        #region Constructors
        public DatabasePickerViewModelOld(IEventAggregator eventAggregator, ISettingsHypermint settingsRepo,
                IFolderExplore folderService, ISelectedService selectedService, IHyperspinManager hyperspinManager)
        {
            _settingsRepo = settingsRepo;
            _folderExploreService = folderService;
            _selectedService = selectedService;
            _eventAggregator = eventAggregator;
            _hyperspinManager = hyperspinManager;

            GenreDatabases = new ListCollectionView(_hyperspinManager.DatabasesCurrentGenres);

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(GetSystemDatabases);
            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(PopulateGenres);

            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);

            SystemDatabases.CurrentChanged += SystemDatabases_CurrentChanged;
        } 
        #endregion

        #region Fields
        private ISettingsHypermint _settingsRepo;
        private IFolderExplore _folderExploreService;
        private ISelectedService _selectedService;       
        private IEventAggregator _eventAggregator;
        private IHyperspinManager _hyperspinManager;
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

        #region Support Methods        

        [Obsolete("Use helpers to get data")]
        private void GetSystemDatabases(string systemName)
        {
            //var xmlsInDirectory = new List<string>();            

            ////Dont set if we're in the main menu
            //if (systemName.Contains("Main Menu"))
            //    return;

            ////Set path and return if not found.
            //var pathToScan = _settingsRepo.HypermintSettings.HsPath +
            //    "\\" +
            //    Root.Databases + "\\" +
            //    _selectedService.CurrentSystem;
            //if (!_fileFolderCheck.DirectoryExists(pathToScan)) return;            

            //foreach (var xmlFile in _fileFolderCheck.GetFiles(pathToScan, "*.xml"))
            //{
            //    var dbFileName = _fileFolderCheck.GetFileNameNoExt(xmlFile);

            //    if (dbFileName.Contains(systemName))
            //    {
            //        try
            //        {
            //            if (dbFileName.ToLower() != "genre")
            //            {
            //                xmlsInDirectory.Add(dbFileName);
            //            }
            //        }
            //        catch (Exception) { }
            //    }
            //}

            //SystemDatabases = new ListCollectionView(xmlsInDirectory);

            //SystemDatabases.MoveCurrentTo(systemName);      
        }

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

        [Obsolete("Use helpers to get data")]
        private void PopulateGenres(string systemName)
        {
            //SystemDatabases.CurrentChanged -= SystemDatabases_CurrentChanged;

            //try
            //{
            //    var hsPath = _settingsRepo.HypermintSettings.HsPath;
            //    var system = _selectedService.CurrentSystem;
                
            //    var genrePath = Path.Combine(hsPath, Root.Databases, system, "Genre.xml");

            //    if (_fileFolderCheck.FileExists(genrePath))
            //        _genreRepo.PopulateGenres(genrePath);
            //    else { _genreRepo.GenreList.Clear(); }

            //    GenreDatabases = new ListCollectionView(_genreRepo.GenreList);
            //}
            //catch (Exception) { }

            //SystemDatabases.CurrentChanged += SystemDatabases_CurrentChanged;

        }

        /// <summary>
        /// Handles the CurrentChanged event of the System Databases.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SystemDatabases_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                var systemDatabaseName = (HyperspinFile)SystemDatabases.CurrentItem;

                _eventAggregator.GetEvent<SystemDatabaseChanged>().Publish(systemDatabaseName.FileName);
            }
            catch (Exception) { }
        }

        #endregion        

    }
}
