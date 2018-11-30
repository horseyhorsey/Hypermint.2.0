using Frontends.Models.Hyperspin;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Extensions;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabasePickerViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IHyperspinManager _hyperspinManager;
        private ISettingsHypermint _settingsRepo;
        private ISelectedService _selectedService;

        #region Constructors

        public DatabasePickerViewModel()
        {

        }

        public DatabasePickerViewModel( IEventAggregator eventAggregator, IHyperspinManager hyperspinManager, ISettingsHypermint settingsRepo, 
            ISelectedService selectedService)
        {
            _eventAggregator = eventAggregator;
            _hyperspinManager = hyperspinManager;
            _settingsRepo = settingsRepo;
            _selectedService = selectedService;

            this.SystemDatabases = new ListCollectionView(_hyperspinManager.DatabasesCurrentSystem);
            this.GenreDatabases = new ListCollectionView(_hyperspinManager.DatabasesCurrentGenres);            

            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);

            SystemDatabases.CurrentChanged += SystemDatabases_CurrentChanged;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(async (x) => await OnSystemChanged(x));
        }

        #endregion

        #region Properties

        public ICollectionView SystemDatabases { get; set; }
        public ICollectionView GenreDatabases { get; set; }

        #endregion

        #region Commands

        public ICommand OpenFolderCommand { get; set; }

        #endregion

        #region Support Methods

        [Obsolete]
        private void OpenFolder(string hyperspinDirType)
        {
            switch (hyperspinDirType)
            {
                case "Databases":
                    var pathToOpen = Path.Combine(_settingsRepo.HypermintSettings.HsPath,
                        Root.Databases, _selectedService.CurrentSystem);
                    var di = new DirectoryInfo(pathToOpen).Open();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when a [system is changed]. Gets the databases matching the system name.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <returns></returns>
        private async Task OnSystemChanged(string systemName)
        {
            try
            {
                await _hyperspinManager.GetSystemDatabases(systemName);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Handles the CurrentChanged event of the SystemDatabases control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void SystemDatabases_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                var dbFile = (HyperspinFile)SystemDatabases.CurrentItem;

                if (dbFile == null) return;
                //Post message that database was changed
                _eventAggregator.GetEvent<SystemDatabaseChanged>().Publish(dbFile.FileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Todo
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

        #endregion

    }
}
