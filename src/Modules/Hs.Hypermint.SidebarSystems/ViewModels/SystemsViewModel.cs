using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Hypermint.Base.Constants;
using Hypermint.Base.Base;
using System.Diagnostics;
using Hypermint.Base.Services;
using System.Windows.Media.Imaging;
using System;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using Prism.Commands;
using Hypermint.Base.Events;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using Hs.Hypermint.SidebarSystems.Dialog;
using System.Reflection;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SystemsViewModel : ViewModelBase, IDropTarget
    {
        #region Properties
        private ICollectionView _systemItems;
        public ICollectionView SystemItems
        {
            get { return _systemItems; }
            set { SetProperty(ref _systemItems, value); }
        }

        private bool systemListEnabled = true;
        public bool SystemListEnabled
        {
            get { return systemListEnabled; }
            set { SetProperty(ref systemListEnabled, value); }
        }

        private string _mainMenuXmlPath;

        private bool reOrderSystems;
        public bool ReOrderSystems
        {
            get { return reOrderSystems; }
            set { SetProperty(ref reOrderSystems, value); }
        }

        private string systemsHeader = "Systems";
        public string SystemsHeader
        {
            get { return systemsHeader; }
            set { SetProperty(ref systemsHeader, value); }
        }

        private int systemCount;
        public int SystemsCount
        {
            get { return systemCount; }
            set { SetProperty(ref systemCount, value); }
        }

        private string pickedDatabaseXml;
        public string PickedDatabaseXml
        {
            get { return pickedDatabaseXml; }
            set { SetProperty(ref pickedDatabaseXml, value); }
        }

        private string shortDbName;
        public string ShortDbName
        {
            get { return shortDbName; }
            set { SetProperty(ref shortDbName, value); }
        }

        private string newSystemName;
        public string NewSystemName
        {
            get { return newSystemName; }
            set { SetProperty(ref newSystemName, value); }
        }

        #endregion

        #region Services
        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;
        IEventAggregator _eventAggregator;
        ISelectedService _selectedService;
        IDialogCoordinator _dialogService;
        CustomDialog customDialog;
        private IFileFolderService _fileFolderService;
        #endregion

        #region Commands
        public DelegateCommand SaveMainMenuCommand { get; private set; }
        public DelegateCommand<string> AddSystemCommand { get; private set; }
        public DelegateCommand CloseDialogCommand { get; private set; }
        public DelegateCommand SelectDatabaseCommand { get; private set; }
        public DelegateCommand SaveNewSystemCommand { get; private set; } 

        #endregion

        public SystemsViewModel()
        {
            _eventAggregator.GetEvent<AddNewSystemEvent>().Publish("SystemsView");
        }

        public SystemsViewModel(IMainMenuRepo main, 
            IEventAggregator eventAggregator,
            ISettingsRepo settings,
            IFileFolderService fileService,
            IDialogCoordinator dialogService,
            ISelectedService selectedService)
        {
            _mainMenuRepo = main;
            _eventAggregator = eventAggregator;

            _settingsRepo = settings;
            _settingsRepo.LoadHypermintSettings();

            _selectedService = selectedService;
            _dialogService = dialogService;
            _fileFolderService = fileService;            

            // Setup the main menu database to read in all systems
            _mainMenuXmlPath = "";

            _mainMenuXmlPath = Path.Combine(
                    _settingsRepo.HypermintSettings.HsPath, Root.Databases,
                    @"Main Menu\Main Menu.xml");

            UpdateSystems(_mainMenuXmlPath);

            _eventAggregator.GetEvent<MainMenuSelectedEvent>().Subscribe(UpdateSystems);
            _eventAggregator.GetEvent<SystemFilteredEvent>().Subscribe(FilterSystemsByText);

            SaveMainMenuCommand = new DelegateCommand(SaveMainMenu);            

            AddSystemCommand = new DelegateCommand<string>(async x =>
            {
                if (!Directory.Exists(_settingsRepo.HypermintSettings.HsPath)) return;

                await AddSystemAsync();
            });

            CloseDialogCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);
            });

            SelectDatabaseCommand = new DelegateCommand(() =>
            {
                if (!Directory.Exists(_settingsRepo.HypermintSettings.HsPath)) return;

                PickedDatabaseXml = 
                    _fileFolderService.setFileDialog(_settingsRepo.HypermintSettings.HsPath + "\\Databases");

                ShortDbName = Path.GetFileNameWithoutExtension(PickedDatabaseXml);
            });

            SaveNewSystemCommand = new DelegateCommand(() => 
            {
                if (string.IsNullOrWhiteSpace(NewSystemName)) return;

                bool createFromExisting = false;

                if (!string.IsNullOrEmpty(PickedDatabaseXml))
                    createFromExisting = true;

                SaveNewSystem(createFromExisting);

            });

        }

        private void SaveNewSystem(bool createFromExistingDb)
        {
            _mainMenuRepo.Systems.Add(new MainMenu()
            {
                Enabled = 1,
                Name = NewSystemName
            });

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Hs.Hypermint.SidebarSystems.systemSettings.ini";

            //Create settings file for system if not already existing.
            var settingsFile = _settingsRepo.HypermintSettings.HsPath + "\\Settings\\" + NewSystemName + ".ini";
            if (!File.Exists(settingsFile))
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                using (var textFile = File.CreateText(settingsFile))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        textFile.WriteLine(line);
                    }
                }
            }

            //Create media dirs
            CreateMediaDirectorysForNewSystem(_settingsRepo.HypermintSettings.HsPath, NewSystemName);

            //Create Database file or directory
            string dbDir = _settingsRepo.HypermintSettings.HsPath + "\\Databases\\";
            if (!createFromExistingDb)
            {
                if (!Directory.Exists(dbDir + NewSystemName))
                {
                    Directory.CreateDirectory(dbDir + NewSystemName);
                    File.Create(dbDir + NewSystemName + "\\" + NewSystemName + ".xml");
                }
            }
            else
            {
                if (!Directory.Exists(dbDir + NewSystemName))
                {
                    Directory.CreateDirectory(dbDir + NewSystemName);                    
                }

                if (!File.Exists(dbDir + NewSystemName + "\\" + NewSystemName + ".xml"))
                {
                    File.Copy(PickedDatabaseXml, dbDir + NewSystemName + "\\" + NewSystemName + ".xml");
                }               

            }

            _dialogService.HideMetroDialogAsync(this, customDialog);
        }

        private void CreateMediaDirectorysForNewSystem(string hsPath, string systemName)
        {
            var newSystemMediaPath = Path.Combine(hsPath, Root.Media, systemName);

            CreateDefaultHyperspinFolders(newSystemMediaPath);
        }

        private void CreateDefaultHyperspinFolders(string hyperSpinSystemMediaDirectory)
        {
            for (int i = 1; i < 5; i++)
            {
                Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\Images\\Artwork" + i);
            }
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Backgrounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.GenreBackgrounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.GenreWheel);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Letters);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Pointer);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Special);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Wheels);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Root.Themes);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.BackgroundMusic);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.SystemExit);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.SystemStart);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.WheelSounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Root.Video);
        }

        private async Task AddSystemAsync()
        {
            customDialog = new CustomDialog() { Title = "Add new system" };

            customDialog.Content = new AddSystemDialog { DataContext = this };

            await _dialogService.ShowMetroDialogAsync(this, customDialog);
        }

        private void SaveMainMenu()
        {
            if (_selectedService.CurrentMainMenu == null) return;

            try
            {
                _eventAggregator.GetEvent<SaveMainMenuEvent>()
                    .Publish(_selectedService.CurrentMainMenu);
            }
            catch (Exception) { }

        }

        private void UpdateSystems(string mainMenuXml)
        {            
            if (File.Exists(mainMenuXml))
            {
                _mainMenuXmlPath = mainMenuXml;
                _mainMenuRepo.BuildMainMenuItems(mainMenuXml, _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\");
                SystemItems = new ListCollectionView(_mainMenuRepo.Systems);

                //Subscribe here again?? 
                //##    Existing unsubscribes when the system list is changed.
                SystemItems.CurrentChanged += SystemItems_CurrentChanged;

                SystemsCount = _mainMenuRepo.Systems.Count - 1;
                SystemsHeader = "Systems: " + SystemsCount;

                _eventAggregator.GetEvent<SystemsGenerated>().Publish("");
                
            }
        }

        /// <summary>
        /// Filter the systems list
        /// </summary>
        /// <param name="obj"></param>
        private void FilterSystemsByText(string filter)
        {
            //Unsubscribe when the filter is being set
            //Avoiding the systems databases loading on each filter change            
            SystemItems.CurrentChanged -= SystemItems_CurrentChanged;

            if (SystemItems != null)
            {
                ICollectionView cv;

                cv = CollectionViewSource.GetDefaultView(SystemItems);

                cv.Filter = o =>
                {
                    var m = o as MainMenu;

                    var textFiltered = m.Name.ToUpper().Contains(filter.ToUpper());
                    return textFiltered;
                };

            }

            SystemItems.CurrentChanged += SystemItems_CurrentChanged;

        }

        private void SystemItems_CurrentChanged(object sender, System.EventArgs e)
        {

            MainMenu system = SystemItems.CurrentItem as MainMenu;

            if (system != null)
            {

                _selectedService.CurrentSystem = system.Name;

                if (!ReOrderSystems)
                {
                    SetSystemImage();

                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                    this._eventAggregator.GetEvent<SystemSelectedEvent>().Publish(system.Name);
                }
            }
        }

        /// <summary>
        /// Set wheel image for the system
        /// </summary>
        /// <param name="path"></param>
        private void SetSystemImage()
        {
            var imagePath = _settingsRepo.HypermintSettings.HsPath +
                "\\Media\\Main Menu\\Images\\Wheel\\" +
                _selectedService.CurrentSystem + ".png";

            if (File.Exists(imagePath))
                _selectedService.SystemImage = setImage(imagePath);

        }

        private BitmapImage setImage(string imagePath)
        {
            Uri uriSource;
            uriSource = new Uri(imagePath);
            return new BitmapImage(uriSource);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as MainMenu;
            var targetItem = dropInfo.TargetItem as MainMenu;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }

        }

        public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as MainMenu;
            var targetItem = dropInfo.TargetItem as MainMenu;

            var AddInIndex = _mainMenuRepo.Systems.IndexOf(targetItem);

            if (AddInIndex == 0)
                AddInIndex = 1;

            _mainMenuRepo.Systems.Remove(sourceItem);
            _mainMenuRepo.Systems.Insert(AddInIndex, sourceItem);
        }
    }
}
