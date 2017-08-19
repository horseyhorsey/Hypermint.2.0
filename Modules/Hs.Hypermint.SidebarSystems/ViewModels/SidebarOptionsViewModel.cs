using Hypermint.Base;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SidebarOptionsViewModel : ViewModelBase
    {
        private IHyperspinManager _hyperSpinManager;
        private IHyperspinXmlDataProvider _dataProvider;
        private IDialogCoordinator _dialogService;
        private IEventAggregator _eventAggregator;

        public SidebarOptionsViewModel(IHyperspinManager hyperSpinManager,
            IHyperspinXmlDataProvider dataProvider, 
            IEventAggregator ea, IDialogCoordinator dialogService)
        {
            _hyperSpinManager = hyperSpinManager;
            _dataProvider = dataProvider;
            _dialogService = dialogService;
            _eventAggregator = ea;

            AddSystemCommand = new DelegateCommand<string>(async x =>
            {
                await OnAddSystem();
            });

            SaveNewSystemCommand = new DelegateCommand(() =>
            {
                OnSaveNewSystem();
            });
        }

        #region Commands
        public ICommand AddSystemCommand { get; private set; }
        public ICommand SaveNewSystemCommand { get; private set; }
        #endregion

        CustomDialog customDialog;

        #region Properties

        private bool reOrderSystems;
        /// <summary>
        /// Gets or sets a value indicating whether [re order systems].
        /// </summary>
        public bool ReOrderSystems
        {
            get { return reOrderSystems; }
            set
            {
                SetProperty(ref reOrderSystems, value);

                _eventAggregator.GetEvent<ReorderingSystemsEvent>().Publish(ReOrderSystems);
            }
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Opens a custom dialog and adds the system asynchronous 
        /// </summary>
        /// <returns></returns>
        private async Task AddSystemAsync()
        {
            try
            {
                customDialog = new CustomDialog() { Title = "Add new system" };

                customDialog.Content = new Dialog.AddSystemDialog { DataContext = new AddSystemDialogViewModel(_dialogService, customDialog, _hyperSpinManager) };

                await _dialogService.ShowMetroDialogAsync(this, customDialog);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnAddSystem()
        {
            await AddSystemAsync();
        }

        private void OnSaveNewSystem()
        {
            //if (string.IsNullOrWhiteSpace(NewSystemName))
            //{
            //    return;
            //}

            //bool createFromExisting = false;

            //if (!string.IsNullOrEmpty(PickedDatabaseXml))
            //    createFromExisting = true;

            //SaveNewSystem(createFromExisting);
        }

        /// <summary>
        /// Saves a new system to the main menu
        /// </summary>
        /// <param name="createFromExistingDb"></param>
        private void SaveNewSystem(string systemName, bool createFromExistingDb)
        {
            _hyperSpinManager.CreateSystem(systemName, createFromExistingDb);
            //Add system to the list
            //_mainMenuRepo.Systems.Add(new MainMenu()
            //{
            //Enabled = 1,
            //Name = NewSystemName
            //});


            //Set the settings file for this system
            //var settingsFile = _settingsRepo.HypermintSettings.HsPath + "\\Settings\\" + NewSystemName + ".ini";

            ////Create settings file for system if not already existing.
            //if (!File.Exists(settingsFile))
            //{
            //    //Load templated hyperspin settings ini from embedded resource
            //    var assembly = Assembly.GetExecutingAssembly();
            //    var resourceName = "Hs.Hypermint.SidebarSystems.systemSettings.ini";

            //    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //    using (var reader = new StreamReader(stream))
            //    using (var textFile = File.CreateText(settingsFile))
            //    {
            //        string line = "";
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            textFile.WriteLine(line);
            //        }
            //    }
            //}

            //CreateMediaDirectorysForNewSystem(_settingsRepo.HypermintSettings.HsPath, NewSystemName);

            ////Create Database file or directory
            //string dbDir = _settingsRepo.HypermintSettings.HsPath + "\\Databases\\";

            //if (!Directory.Exists(dbDir + NewSystemName))
            //{
            //    Directory.CreateDirectory(dbDir + NewSystemName);
            //}

            //if (!createFromExistingDb) //Create a blank database with new system name.
            //{
            //    File.Create(dbDir + NewSystemName + "\\" + NewSystemName + ".xml");
            //}
            //else // Create a new database from an existing hyperspin xml
            //{
            //    if (!File.Exists(dbDir + NewSystemName + "\\" + NewSystemName + ".xml"))
            //    {
            //        File.Copy(PickedDatabaseXml, dbDir + NewSystemName + "\\" + NewSystemName + ".xml");
            //    }
            //}

            //_dialogService.HideMetroDialogAsync(this, customDialog);
        }

        #endregion
    }
}
